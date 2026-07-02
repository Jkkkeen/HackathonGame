using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace FeatherDetective.Tests
{
    public sealed class MemoryPlaybackControllerTests
    {
        [Test]
        public void SelectEffectReturnsEffectMatchingFeatherSpecies()
        {
            var crow = new FakeMemoryEffect(BirdSpecies.Crow);
            var magpie = new FakeMemoryEffect(BirdSpecies.Magpie);

            var selected = MemoryPlaybackController.SelectEffectForTests(new IMemoryEffect[] { crow, magpie }, BirdSpecies.Magpie);

            Assert.That(selected, Is.EqualTo(magpie));
        }

        [Test]
        public void SelectEffectReturnsNullWhenSpeciesIsMissing()
        {
            var crow = new FakeMemoryEffect(BirdSpecies.Crow);

            var selected = MemoryPlaybackController.SelectEffectForTests(new IMemoryEffect[] { crow }, BirdSpecies.Sparrow);

            Assert.That(selected, Is.Null);
        }

        [Test]
        public void SelectEffectAcceptsEnumerableEffects()
        {
            var crow = new FakeMemoryEffect(BirdSpecies.Crow);
            var pigeon = new FakeMemoryEffect(BirdSpecies.Pigeon);

            var selected = MemoryPlaybackController.SelectEffectForTests(YieldEffects(crow, pigeon), BirdSpecies.Pigeon);

            Assert.That(selected, Is.EqualTo(pigeon));
        }

        [Test]
        public void ConfigureForBuilderAssignsContext()
        {
            var gameObject = new GameObject("Memory Playback Builder Test");
            var contextObject = new GameObject("Memory Context");
            try
            {
                var controller = gameObject.AddComponent<MemoryPlaybackController>();
                var context = contextObject.AddComponent<MemoryContext>();

                controller.ConfigureForBuilder(context);

                Assert.That(controller.Context, Is.EqualTo(context));
            }
            finally
            {
                Object.DestroyImmediate(gameObject);
                Object.DestroyImmediate(contextObject);
            }
        }

        [Test]
        public void ColorMemoryTargetPreservesConfiguredImportantColorAndRestoresOriginal()
        {
            var gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            try
            {
                var renderer = gameObject.GetComponent<Renderer>();
                renderer.material = new Material(Shader.Find("Standard"));
                renderer.material.color = new Color(0.1f, 0.8f, 0.2f, 0.5f);

                var target = gameObject.AddComponent<ColorMemoryTarget>();
                target.ConfigureForBuilder(Color.green);

                target.ApplyMagpieState(new[] { Color.green });

                Assert.That(renderer.material.color, Is.EqualTo(new Color(0.1f, 0.8f, 0.2f, 0.5f)));

                renderer.material.color = Color.black;
                target.Restore();

                Assert.That(renderer.material.color, Is.EqualTo(new Color(0.1f, 0.8f, 0.2f, 0.5f)));
            }
            finally
            {
                Object.DestroyImmediate(gameObject);
            }
        }

        [Test]
        public void ColorMemoryTargetGrayscalesNonImportantConfiguredColor()
        {
            var gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            try
            {
                var renderer = gameObject.GetComponent<Renderer>();
                renderer.material = new Material(Shader.Find("Standard"));
                renderer.material.color = new Color(0.2f, 0.4f, 0.8f, 0.25f);

                var target = gameObject.AddComponent<ColorMemoryTarget>();
                target.ConfigureForBuilder(Color.blue);

                target.ApplyMagpieState(new[] { Color.red });

                var color = renderer.material.color;
                Assert.That(color.r, Is.EqualTo(color.g).Within(0.001f));
                Assert.That(color.g, Is.EqualTo(color.b).Within(0.001f));
                Assert.That(color.a, Is.EqualTo(0.25f).Within(0.001f));
            }
            finally
            {
                Object.DestroyImmediate(gameObject);
            }
        }

        [UnityTest]
        public IEnumerator RouteLineRendererEnablesDuringAnimationAndHidesAfterPlayback()
        {
            var gameObject = new GameObject("Route Line Renderer Test");
            try
            {
                var lineRenderer = gameObject.AddComponent<LineRenderer>();
                lineRenderer.startWidth = 0.4f;
                lineRenderer.endWidth = 0.2f;
                var routeRenderer = gameObject.AddComponent<RouteLineRenderer>();
                var route = new[]
                {
                    Vector3.zero,
                    Vector3.one
                };

                yield return null;

                var animation = routeRenderer.AnimateRoute(route, 0f);
                Assert.That(animation.MoveNext(), Is.True);
                Assert.That(lineRenderer.enabled, Is.True);
                Assert.That(lineRenderer.positionCount, Is.EqualTo(2));

                while (animation.MoveNext())
                {
                    yield return animation.Current;
                }

                Assert.That(lineRenderer.enabled, Is.False);
            }
            finally
            {
                Object.DestroyImmediate(gameObject);
            }
        }

        [UnityTest]
        public IEnumerator PlayMemoryIgnoresRequestsWhileMemoryIsPlaying()
        {
            var gameObject = new GameObject("Memory Playback Controller Test");
            try
            {
                var controller = gameObject.AddComponent<MemoryPlaybackController>();
                var crowEffect = gameObject.AddComponent<BlockingMemoryEffect>();
                var pigeonEffect = gameObject.AddComponent<BlockingMemoryEffect>();
                crowEffect.Configure(BirdSpecies.Crow);
                pigeonEffect.Configure(BirdSpecies.Pigeon);

                var crowFeather = CreateFeather(BirdSpecies.Crow);
                var pigeonFeather = CreateFeather(BirdSpecies.Pigeon);

                yield return null;

                controller.PlayMemory(crowFeather);
                yield return null;
                controller.PlayMemory(pigeonFeather);
                yield return null;

                Assert.That(crowEffect.PlayCount, Is.EqualTo(1));
                Assert.That(crowEffect.IsPlaying, Is.True);
                Assert.That(pigeonEffect.PlayCount, Is.EqualTo(0));

                crowEffect.Release();
                yield return null;
            }
            finally
            {
                Object.DestroyImmediate(gameObject);
            }
        }

        [UnityTest]
        public IEnumerator PlayMemoryIgnoresNullRequestWithoutBlockingNextPlayback()
        {
            var gameObject = new GameObject("Memory Playback Null Test");
            try
            {
                var controller = gameObject.AddComponent<MemoryPlaybackController>();
                var crowEffect = gameObject.AddComponent<BlockingMemoryEffect>();
                crowEffect.Configure(BirdSpecies.Crow);

                var crowFeather = CreateFeather(BirdSpecies.Crow);

                yield return null;

                controller.PlayMemory(null);
                yield return null;
                controller.PlayMemory(crowFeather);
                yield return null;

                Assert.That(crowEffect.PlayCount, Is.EqualTo(1));
                Assert.That(crowEffect.IsPlaying, Is.True);

                crowEffect.Release();
                yield return null;
            }
            finally
            {
                Object.DestroyImmediate(gameObject);
            }
        }

        [Test]
        public void RestoreAfterMemoryRestoresPresentationState()
        {
            var contextObject = new GameObject("Memory Context Restore Test");
            var colorObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            var routeObject = new GameObject("Route Renderer");
            var rigObject = new GameObject("First Person Rig");
            try
            {
                var overlay = contextObject.AddComponent<CanvasGroup>();
                overlay.alpha = 0.2f;

                var audioSource = contextObject.AddComponent<AudioSource>();
                audioSource.spatialBlend = 0.3f;
                audioSource.panStereo = -0.4f;
                audioSource.loop = true;

                var renderer = colorObject.GetComponent<Renderer>();
                renderer.material = new Material(Shader.Find("Standard"));
                renderer.material.color = Color.yellow;
                var colorTarget = colorObject.AddComponent<ColorMemoryTarget>();
                colorTarget.ConfigureForBuilder(Color.yellow);

                var lineRenderer = routeObject.AddComponent<LineRenderer>();
                var routeRenderer = routeObject.AddComponent<RouteLineRenderer>();

                rigObject.transform.localPosition = new Vector3(1f, 2f, 3f);
                var rig = rigObject.AddComponent<FirstPersonMemoryRig>();
                rig.ConfigureForBuilder(null, null);

                var context = contextObject.AddComponent<MemoryContext>();
                context.ConfigureForBuilder(overlay, audioSource, new[] { colorTarget }, routeRenderer, rig, null);

                overlay.alpha = 0.9f;
                audioSource.spatialBlend = 1f;
                audioSource.panStereo = 0.8f;
                audioSource.loop = false;
                renderer.material.color = Color.black;
                lineRenderer.enabled = true;
                lineRenderer.positionCount = 2;
                rigObject.transform.localPosition = Vector3.zero;

                context.RestoreAfterMemory();

                Assert.That(overlay.alpha, Is.EqualTo(0.2f).Within(0.001f));
                Assert.That(audioSource.spatialBlend, Is.EqualTo(0.3f).Within(0.001f));
                Assert.That(audioSource.panStereo, Is.EqualTo(-0.4f).Within(0.001f));
                Assert.That(audioSource.loop, Is.True);
                Assert.That(renderer.material.color, Is.EqualTo(Color.yellow));
                Assert.That(lineRenderer.enabled, Is.False);
                Assert.That(lineRenderer.positionCount, Is.EqualTo(0));
                Assert.That(rigObject.transform.localPosition, Is.EqualTo(new Vector3(1f, 2f, 3f)));
            }
            finally
            {
                Object.DestroyImmediate(contextObject);
                Object.DestroyImmediate(colorObject);
                Object.DestroyImmediate(routeObject);
                Object.DestroyImmediate(rigObject);
            }
        }

        [UnityTest]
        public IEnumerator StopPlaybackRestoresContextAndAllowsNextPlayback()
        {
            var gameObject = new GameObject("Memory Playback Stop Test");
            var contextObject = new GameObject("Memory Context Stop Test");
            try
            {
                var overlay = contextObject.AddComponent<CanvasGroup>();
                overlay.alpha = 0.15f;
                var context = contextObject.AddComponent<MemoryContext>();
                context.ConfigureForBuilder(overlay, null, new ColorMemoryTarget[0], null, null, null);

                var controller = gameObject.AddComponent<MemoryPlaybackController>();
                var crowEffect = gameObject.AddComponent<BlockingMemoryEffect>();
                crowEffect.Configure(BirdSpecies.Crow);
                controller.ConfigureForBuilder(context);

                yield return null;

                controller.PlayMemory(CreateFeather(BirdSpecies.Crow));
                yield return null;

                overlay.alpha = 0.8f;
                controller.StopPlayback();

                Assert.That(overlay.alpha, Is.EqualTo(0.15f).Within(0.001f));

                controller.PlayMemory(CreateFeather(BirdSpecies.Crow));
                yield return null;

                Assert.That(crowEffect.PlayCount, Is.EqualTo(2));

                crowEffect.Release();
                yield return null;
            }
            finally
            {
                Object.DestroyImmediate(gameObject);
                Object.DestroyImmediate(contextObject);
            }
        }

        private static IEnumerable<IMemoryEffect> YieldEffects(params IMemoryEffect[] effects)
        {
            for (var i = 0; i < effects.Length; i++)
            {
                yield return effects[i];
            }
        }

        private static FeatherDefinition CreateFeather(BirdSpecies species)
        {
            var feather = ScriptableObject.CreateInstance<FeatherDefinition>();
            feather.ConfigureForTests(
                species.ToString(),
                species.ToString(),
                species,
                ClueRole.MainMystery,
                new DeductionSlotId[0],
                new Color[0],
                new Vector3[0],
                string.Empty);
            return feather;
        }

        private sealed class FakeMemoryEffect : IMemoryEffect
        {
            public FakeMemoryEffect(BirdSpecies species)
            {
                Species = species;
            }

            public BirdSpecies Species { get; }

            public IEnumerator Play(FeatherDefinition feather, MemoryContext context)
            {
                yield break;
            }
        }

        private sealed class BlockingMemoryEffect : MonoBehaviour, IMemoryEffect
        {
            private bool released;

            public BirdSpecies Species { get; private set; }
            public int PlayCount { get; private set; }
            public bool IsPlaying { get; private set; }

            public void Configure(BirdSpecies species)
            {
                Species = species;
            }

            public void Release()
            {
                released = true;
            }

            public IEnumerator Play(FeatherDefinition feather, MemoryContext context)
            {
                PlayCount++;
                IsPlaying = true;

                while (!released)
                {
                    yield return null;
                }

                IsPlaying = false;
            }
        }
    }
}
