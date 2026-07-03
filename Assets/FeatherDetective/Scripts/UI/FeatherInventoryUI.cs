using UnityEngine;
using UnityEngine.UI;

namespace FeatherDetective
{
    public sealed class FeatherInventoryUI : MonoBehaviour
    {
        [SerializeField] private Text label;
        [SerializeField] private int totalFeathers = 12;

        public void ConfigureForBuilder(Text newLabel)
        {
            label = newLabel;
        }

        public void ConfigureForBuilder(Text newLabel, int newTotalFeathers)
        {
            label = newLabel;
            totalFeathers = newTotalFeathers;
        }

        public void Refresh(InvestigationRuntime runtime)
        {
            if (label == null || runtime == null)
            {
                return;
            }

            var selected = runtime.SelectedFeather != null ? runtime.SelectedFeather.DisplayName : "None";
            var selectedIndex = GetSelectedIndex(runtime);
            var selectedCount = runtime.CollectedFeathers.Count;
            label.text = $"Feathers {runtime.State.CollectedCount}/{totalFeathers}\nSelected {selectedIndex}/{selectedCount}: {selected}";
        }

        private static int GetSelectedIndex(InvestigationRuntime runtime)
        {
            if (runtime.SelectedFeather == null)
            {
                return 0;
            }

            for (var i = 0; i < runtime.CollectedFeathers.Count; i++)
            {
                if (runtime.CollectedFeathers[i] == runtime.SelectedFeather)
                {
                    return i + 1;
                }
            }

            return 0;
        }
    }
}
