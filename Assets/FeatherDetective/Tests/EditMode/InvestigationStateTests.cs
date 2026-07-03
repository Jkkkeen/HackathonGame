using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace FeatherDetective.Tests
{
    public sealed class InvestigationStateTests
    {
        [Test]
        public void CollectStoresEachFeatherOnlyOnce()
        {
            var state = new InvestigationState();
            var feather = CreateFeather("crow-bench", BirdSpecies.Crow, DeductionSlotId.LastFeedingDay);

            Assert.That(state.Collect(feather), Is.True);
            Assert.That(state.Collect(feather), Is.False);
            Assert.That(state.HasCollected(feather), Is.True);
            Assert.That(state.CollectedCount, Is.EqualTo(1));
        }

        [Test]
        public void TryPlaceRequiresCollectedCompatibleFeather()
        {
            var state = new InvestigationState();
            var feather = CreateFeather("pigeon-route", BirdSpecies.Pigeon, DeductionSlotId.WhereHeWent);

            Assert.That(state.TryPlace(feather, DeductionSlotId.WhereHeWent), Is.False);

            state.Collect(feather);

            Assert.That(state.TryPlace(feather, DeductionSlotId.LastFeedingDay), Is.False);
            Assert.That(state.TryPlace(feather, DeductionSlotId.WhereHeWent), Is.True);
            Assert.That(state.GetPlacedFeatherId(DeductionSlotId.WhereHeWent), Is.EqualTo("pigeon-route"));
        }

        [Test]
        public void PlacementsDoesNotExposeMutableDictionary()
        {
            var state = new InvestigationState();
            var feather = CreateFeather("crow-bench", BirdSpecies.Crow, DeductionSlotId.LastFeedingDay);

            state.Collect(feather);
            state.TryPlace(feather, DeductionSlotId.LastFeedingDay);

            Assert.That(state.Placements, Is.Not.TypeOf<Dictionary<DeductionSlotId, string>>());
            Assert.That(state.GetPlacedFeatherId(DeductionSlotId.LastFeedingDay), Is.EqualTo("crow-bench"));
        }

        [Test]
        public void IsSolvedRequiresEverySolutionSlotToMatch()
        {
            var crow = CreateFeather("crow-bench", BirdSpecies.Crow, DeductionSlotId.LastFeedingDay);
            var pigeon = CreateFeather("pigeon-edge", BirdSpecies.Pigeon, DeductionSlotId.WhereHeWent);
            var solution = ScriptableObject.CreateInstance<DeductionSolution>();
            solution.ConfigureForTests(new[]
            {
                new DeductionAnswer(DeductionSlotId.LastFeedingDay, crow),
                new DeductionAnswer(DeductionSlotId.WhereHeWent, pigeon)
            });

            var state = new InvestigationState();
            state.Collect(crow);
            state.Collect(pigeon);
            state.TryPlace(crow, DeductionSlotId.LastFeedingDay);

            Assert.That(state.IsSolved(solution), Is.False);

            state.TryPlace(pigeon, DeductionSlotId.WhereHeWent);

            Assert.That(state.IsSolved(solution), Is.True);
        }

        [Test]
        public void IsSolvedRejectsAtmosphereFeatherPlacedIntoMainMysterySlot()
        {
            var required = CreateFeather("pigeon-edge", BirdSpecies.Pigeon, DeductionSlotId.WhatEvidenceMeans);
            var atmosphere = CreateFeather("sparrow-mood", BirdSpecies.Sparrow, DeductionSlotId.WhatEvidenceMeans, ClueRole.Atmosphere);
            var solution = ScriptableObject.CreateInstance<DeductionSolution>();
            solution.ConfigureForTests(new[]
            {
                new DeductionAnswer(DeductionSlotId.WhatEvidenceMeans, required)
            });

            var state = new InvestigationState();
            state.Collect(atmosphere);
            state.TryPlace(atmosphere, DeductionSlotId.WhatEvidenceMeans);

            Assert.That(state.IsSolved(solution), Is.False);
        }

        private static FeatherDefinition CreateFeather(string id, BirdSpecies species, DeductionSlotId slot)
        {
            return CreateFeather(id, species, slot, ClueRole.MainMystery);
        }

        private static FeatherDefinition CreateFeather(string id, BirdSpecies species, DeductionSlotId slot, ClueRole role)
        {
            var feather = ScriptableObject.CreateInstance<FeatherDefinition>();
            feather.ConfigureForTests(id, id, species, role, new[] { slot }, new Color[0], new Vector3[0], id);
            return feather;
        }
    }
}
