using System.Collections.Generic;
using UnityEngine;

namespace FeatherDetective
{
    public sealed class InvestigationRuntime : MonoBehaviour
    {
        [SerializeField] private MemoryPlaybackController memoryPlayback;
        [SerializeField] private FeatherInventoryUI inventoryUI;

        private readonly List<FeatherDefinition> collectedFeathers = new List<FeatherDefinition>();

        public InvestigationState State { get; } = new InvestigationState();
        public FeatherDefinition SelectedFeather { get; private set; }
        public IReadOnlyList<FeatherDefinition> CollectedFeathers => collectedFeathers;

        public void ConfigureForBuilder(MemoryPlaybackController newMemoryPlayback, FeatherInventoryUI newInventoryUI)
        {
            memoryPlayback = newMemoryPlayback;
            inventoryUI = newInventoryUI;
            RefreshInventory();
        }

        public bool CollectFeather(FeatherDefinition feather)
        {
            if (feather == null)
            {
                return false;
            }

            var collected = State.Collect(feather);
            SelectedFeather = feather;

            if (collected)
            {
                collectedFeathers.Add(feather);
            }

            RefreshInventory();

            if (memoryPlayback != null)
            {
                memoryPlayback.PlayMemory(feather);
            }

            return collected;
        }

        public bool TryPlaceSelected(DeductionSlotId slotId)
        {
            return SelectedFeather != null && State.TryPlace(SelectedFeather, slotId);
        }

        public void SelectNextCollectedFeather()
        {
            if (collectedFeathers.Count == 0)
            {
                SelectedFeather = null;
                RefreshInventory();
                return;
            }

            var currentIndex = SelectedFeather != null ? collectedFeathers.IndexOf(SelectedFeather) : -1;
            var nextIndex = (currentIndex + 1) % collectedFeathers.Count;
            SelectedFeather = collectedFeathers[nextIndex];
            RefreshInventory();
        }

        private void RefreshInventory()
        {
            if (inventoryUI != null)
            {
                inventoryUI.Refresh(this);
            }
        }
    }
}
