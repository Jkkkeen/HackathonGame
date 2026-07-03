using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

namespace FeatherDetective.Tests
{
    public sealed class FeatherInventoryUITests
    {
        [Test]
        public void RefreshUsesConfiguredTotalFeathers()
        {
            var runtimeObject = new GameObject("Investigation Runtime");
            var uiObject = new GameObject("Feather Inventory UI");
            var labelObject = new GameObject("Feather Inventory Label", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));

            try
            {
                var runtime = runtimeObject.AddComponent<InvestigationRuntime>();
                var ui = uiObject.AddComponent<FeatherInventoryUI>();
                var label = labelObject.GetComponent<Text>();
                label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

                ui.ConfigureForBuilder(label, 24);

                var feather = ScriptableObject.CreateInstance<FeatherDefinition>();
                feather.ConfigureForTests(
                    "crow-bench",
                    "Crow Feather",
                    BirdSpecies.Crow,
                    ClueRole.MainMystery,
                    new[] { DeductionSlotId.LastFeedingDay },
                    new Color[0],
                    new Vector3[0],
                    string.Empty);

                runtime.CollectFeather(feather);
                ui.Refresh(runtime);

                Assert.That(label.text, Is.EqualTo("Feathers 1/24\nSelected: Crow Feather"));
            }
            finally
            {
                Object.DestroyImmediate(runtimeObject);
                Object.DestroyImmediate(uiObject);
                Object.DestroyImmediate(labelObject);
            }
        }
    }
}
