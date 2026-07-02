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

        private static FeatherDefinition CreateFeather(string id, BirdSpecies species, DeductionSlotId slot)
        {
            var feather = ScriptableObject.CreateInstance<FeatherDefinition>();
            feather.ConfigureForTests(id, id, species, ClueRole.MainMystery, new[] { slot }, new Color[0], new Vector3[0], id);
            return feather;
        }
    }
}
