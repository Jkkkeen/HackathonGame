using NUnit.Framework;
using UnityEngine;

namespace FeatherDetective.Tests
{
    public sealed class FeatherDefinitionTests
    {
        [Test]
        public void FeatherDefinitionStoresSensoryIdentityAndCompatibleSlots()
        {
            var feather = ScriptableObject.CreateInstance<FeatherDefinition>();
            feather.ConfigureForTests(
                "crow-bench",
                "Feather by the feeding bench",
                BirdSpecies.Crow,
                ClueRole.MainMystery,
                new[] { DeductionSlotId.LastFeedingDay, DeductionSlotId.WhatChanged },
                new[] { new Color(0.1f, 0.2f, 0.3f) },
                new[] { new Vector3(0f, 0f, 0f), new Vector3(1f, 0f, 2f) },
                "Hospital siren, wheels, and a soft worried voice."
            );

            Assert.That(feather.Id, Is.EqualTo("crow-bench"));
            Assert.That(feather.DisplayName, Is.EqualTo("Feather by the feeding bench"));
            Assert.That(feather.Species, Is.EqualTo(BirdSpecies.Crow));
            Assert.That(feather.Role, Is.EqualTo(ClueRole.MainMystery));
            Assert.That(feather.CanPlaceInSlot(DeductionSlotId.LastFeedingDay), Is.True);
            Assert.That(feather.CanPlaceInSlot(DeductionSlotId.WhereHeWent), Is.False);
            Assert.That(feather.HighlightColors, Has.Length.EqualTo(1));
            Assert.That(feather.RoutePoints, Has.Length.EqualTo(2));
            Assert.That(feather.MemorySummary, Does.Contain("Hospital siren"));
        }

        [Test]
        public void DeductionSolutionIsSatisfiedOnlyByExactRequiredFeatherIds()
        {
            var crow = CreateFeather("crow-bench", BirdSpecies.Crow, DeductionSlotId.LastFeedingDay);
            var pigeon = CreateFeather("pigeon-park-edge", BirdSpecies.Pigeon, DeductionSlotId.WhereHeWent);
            var solution = ScriptableObject.CreateInstance<DeductionSolution>();
            solution.ConfigureForTests(new[]
            {
                new DeductionAnswer(DeductionSlotId.LastFeedingDay, crow),
                new DeductionAnswer(DeductionSlotId.WhereHeWent, pigeon)
            });

            Assert.That(solution.RequiredSlots, Has.Length.EqualTo(2));
            Assert.That(solution.IsRequiredAnswer(DeductionSlotId.LastFeedingDay, crow), Is.True);
            Assert.That(solution.IsRequiredAnswer(DeductionSlotId.WhereHeWent, crow), Is.False);
        }

        private static FeatherDefinition CreateFeather(string id, BirdSpecies species, DeductionSlotId slot)
        {
            var feather = ScriptableObject.CreateInstance<FeatherDefinition>();
            feather.ConfigureForTests(id, id, species, ClueRole.MainMystery, new[] { slot }, new Color[0], new Vector3[0], id);
            return feather;
        }
    }
}
