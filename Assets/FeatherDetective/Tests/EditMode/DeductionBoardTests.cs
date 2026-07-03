using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace FeatherDetective.Tests
{
    public sealed class DeductionBoardTests
    {
        [Test]
        public void InspectPlacesSelectedFeatherIntoCompatibleSlotAndTriggersEndingWhenSolved()
        {
            var feather = CreateFeather("pigeon-edge", BirdSpecies.Pigeon, DeductionSlotId.WhatEvidenceMeans);
            var solution = ScriptableObject.CreateInstance<DeductionSolution>();
            solution.ConfigureForTests(new[]
            {
                new DeductionAnswer(DeductionSlotId.WhatEvidenceMeans, feather)
            });

            var runtimeObject = new GameObject("Investigation Runtime");
            var boardObject = new GameObject("Deduction Board");
            var endingObject = new GameObject("Ending Controller");
            var slotObject = new GameObject("Deduction Slot");

            try
            {
                var runtime = runtimeObject.AddComponent<InvestigationRuntime>();
                var board = boardObject.AddComponent<DeductionBoard>();
                var ending = endingObject.AddComponent<EndingController>();
                var slot = slotObject.AddComponent<DeductionSlot>();

                slot.ConfigureForBuilder(DeductionSlotId.WhatEvidenceMeans);
                board.ConfigureForBuilder(runtime, solution, ending, new[] { slot });

                runtime.CollectFeather(feather);
                board.Inspect();

                Assert.That(runtime.State.GetPlacedFeatherId(DeductionSlotId.WhatEvidenceMeans), Is.EqualTo("pigeon-edge"));
                Assert.That(ending.HasPlayed, Is.True);
            }
            finally
            {
                Object.DestroyImmediate(runtimeObject);
                Object.DestroyImmediate(boardObject);
                Object.DestroyImmediate(endingObject);
                Object.DestroyImmediate(slotObject);
            }
        }

        [Test]
        public void RuntimeCollectsFeatherOnlyOnceAndCyclesSelection()
        {
            var crow = CreateFeather("crow-bench", BirdSpecies.Crow, DeductionSlotId.LastFeedingDay);
            var pigeon = CreateFeather("pigeon-edge", BirdSpecies.Pigeon, DeductionSlotId.WhatEvidenceMeans);
            var runtimeObject = new GameObject("Investigation Runtime");

            try
            {
                var runtime = runtimeObject.AddComponent<InvestigationRuntime>();

                Assert.That(runtime.CollectFeather(crow), Is.True);
                Assert.That(runtime.CollectFeather(crow), Is.False);
                Assert.That(runtime.CollectFeather(pigeon), Is.True);
                Assert.That(runtime.CollectedFeathers, Has.Count.EqualTo(2));
                Assert.That(runtime.SelectedFeather, Is.EqualTo(pigeon));

                runtime.SelectNextCollectedFeather();

                Assert.That(runtime.SelectedFeather, Is.EqualTo(crow));

                Assert.That(runtime.CollectFeather(pigeon), Is.False);
                Assert.That(runtime.CollectedFeathers, Has.Count.EqualTo(2));
                Assert.That(runtime.SelectedFeather, Is.EqualTo(pigeon));
            }
            finally
            {
                Object.DestroyImmediate(runtimeObject);
            }
        }

        [Test]
        public void CollectFeatherNullDoesNotClearSelectionOrAddToCollection()
        {
            var crow = CreateFeather("crow-bench", BirdSpecies.Crow, DeductionSlotId.LastFeedingDay);
            var runtimeObject = new GameObject("Investigation Runtime");
            var uiObject = new GameObject("Feather Inventory UI");
            var labelObject = new GameObject("Feather Inventory Label", typeof(RectTransform), typeof(CanvasRenderer), typeof(UnityEngine.UI.Text));

            try
            {
                var runtime = runtimeObject.AddComponent<InvestigationRuntime>();
                var ui = uiObject.AddComponent<FeatherInventoryUI>();
                var label = labelObject.GetComponent<UnityEngine.UI.Text>();
                label.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                ui.ConfigureForBuilder(label);
                runtime.ConfigureForBuilder(null, ui);

                Assert.That(runtime.CollectFeather(crow), Is.True);
                Assert.That(runtime.SelectedFeather, Is.EqualTo(crow));
                Assert.That(runtime.CollectedFeathers, Has.Count.EqualTo(1));
                Assert.That(label.text, Is.EqualTo("Feathers 1/12\nSelected: crow-bench"));

                label.text = "sentinel";

                Assert.That(runtime.CollectFeather(null), Is.False);
                Assert.That(runtime.SelectedFeather, Is.EqualTo(crow));
                Assert.That(runtime.CollectedFeathers, Has.Count.EqualTo(1));
                Assert.That(label.text, Is.EqualTo("sentinel"));
            }
            finally
            {
                Object.DestroyImmediate(runtimeObject);
                Object.DestroyImmediate(uiObject);
                Object.DestroyImmediate(labelObject);
            }
        }

        [Test]
        public void FeatherPickupWithoutDefinitionDoesNotDisableCollectedVisual()
        {
            var pickupObject = new GameObject("Feather Pickup");
            var runtimeObject = new GameObject("Investigation Runtime");
            var visualObject = new GameObject("Feather Visual");

            try
            {
                var pickup = pickupObject.AddComponent<FeatherPickup>();
                var runtime = runtimeObject.AddComponent<InvestigationRuntime>();
                pickup.ConfigureForBuilder(null, runtime);

                var serializedPickup = new SerializedObject(pickup);
                serializedPickup.FindProperty("collectedVisual").objectReferenceValue = visualObject;
                serializedPickup.ApplyModifiedPropertiesWithoutUndo();

                pickup.Inspect();

                Assert.That(visualObject.activeSelf, Is.True);
            }
            finally
            {
                Object.DestroyImmediate(pickupObject);
                Object.DestroyImmediate(runtimeObject);
                Object.DestroyImmediate(visualObject);
            }
        }

        private static FeatherDefinition CreateFeather(string id, BirdSpecies species, DeductionSlotId slot)
        {
            var feather = ScriptableObject.CreateInstance<FeatherDefinition>();
            feather.ConfigureForTests(id, id, species, ClueRole.MainMystery, new[] { slot }, new Color[0], new Vector3[0], id);
            return feather;
        }
    }
}
