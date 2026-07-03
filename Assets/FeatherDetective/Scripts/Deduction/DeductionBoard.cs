using UnityEngine;

namespace FeatherDetective
{
    public sealed class DeductionBoard : MonoBehaviour, IInspectable
    {
        [SerializeField] private InvestigationRuntime runtime;
        [SerializeField] private DeductionSolution solution;
        [SerializeField] private EndingController endingController;
        [SerializeField] private DeductionSlot[] slots = new DeductionSlot[0];

        public string PromptText => "Place";

        public void ConfigureForBuilder(
            InvestigationRuntime newRuntime,
            DeductionSolution newSolution,
            EndingController newEndingController,
            DeductionSlot[] newSlots)
        {
            runtime = newRuntime;
            solution = newSolution;
            endingController = newEndingController;
            slots = newSlots ?? new DeductionSlot[0];
        }

        public void Inspect()
        {
            if (runtime == null || runtime.SelectedFeather == null || slots == null)
            {
                return;
            }

            for (var i = 0; i < slots.Length; i++)
            {
                var slot = slots[i];
                if (slot != null && runtime.SelectedFeather.CanPlaceInSlot(slot.SlotId))
                {
                    Place(slot.SlotId);
                    return;
                }
            }
        }

        public bool Place(DeductionSlotId slotId)
        {
            if (runtime == null)
            {
                return false;
            }

            var placed = runtime.TryPlaceSelected(slotId);
            if (placed && runtime.State.IsSolved(solution) && endingController != null)
            {
                endingController.PlayEnding();
            }

            return placed;
        }
    }
}
