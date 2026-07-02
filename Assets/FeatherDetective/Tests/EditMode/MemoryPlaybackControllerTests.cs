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
        public void RouteLineRendererAcceptsDurationWhenAnimatingRoute()
        {
            var gameObject = new GameObject("Route Line Renderer Test");
            try
            {
                var routeRenderer = gameObject.AddComponent<RouteLineRenderer>();
                var route = new[]
                {
                    Vector3.zero,
                    Vector3.one
                };

                var animation = routeRenderer.AnimateRoute(route, 0.5f);

                Assert.That(animation, Is.Not.Null);
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
