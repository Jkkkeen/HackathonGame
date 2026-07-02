using System.Collections;
using NUnit.Framework;
using UnityEngine;

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
    }
}
