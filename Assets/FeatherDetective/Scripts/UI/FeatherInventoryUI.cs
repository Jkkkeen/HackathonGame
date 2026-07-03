using UnityEngine;
using UnityEngine.UI;

namespace FeatherDetective
{
    public sealed class FeatherInventoryUI : MonoBehaviour
    {
        [SerializeField] private Text label;

        public void ConfigureForBuilder(Text newLabel)
        {
            label = newLabel;
        }

        public void Refresh(InvestigationRuntime runtime)
        {
            if (label == null || runtime == null)
            {
                return;
            }

            var selected = runtime.SelectedFeather != null ? runtime.SelectedFeather.DisplayName : "None";
            label.text = $"Feathers {runtime.State.CollectedCount}/12\nSelected: {selected}";
        }
    }
}
