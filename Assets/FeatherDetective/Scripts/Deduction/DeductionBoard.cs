using UnityEngine;

namespace FeatherDetective
{
    public sealed class DeductionBoard : MonoBehaviour, IInspectable
    {
        [SerializeField] private InvestigationRuntime runtime;
        [SerializeField] private DeductionSolution solution;
        [SerializeField] private EndingController endingController;
        [SerializeField] private DeductionBoardUI boardUI;
        [SerializeField] private DeductionSlot[] slots = new DeductionSlot[0];

        public string PromptText => "Place feather";

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

        public void ConfigureForBuilder(
            InvestigationRuntime newRuntime,
            DeductionSolution newSolution,
            EndingController newEndingController,
            DeductionSlot[] newSlots,
            DeductionBoardUI newBoardUI)
        {
            ConfigureForBuilder(newRuntime, newSolution, newEndingController, newSlots);
            boardUI = newBoardUI;
        }

        public void Inspect()
        {
            if (runtime == null || slots == null)
            {
                return;
            }

            var selected = runtime.SelectedFeather;
            if (selected == null)
            {
                boardUI?.ShowNoSelectedFeather(runtime);
                return;
            }

            for (var i = 0; i < slots.Length; i++)
            {
                var slot = slots[i];
                if (slot != null && selected.CanPlaceInSlot(slot.SlotId))
                {
                    Place(slot.SlotId);
                    return;
                }
            }

            boardUI?.ShowCannotPlace(runtime, selected);
        }

        public bool Place(DeductionSlotId slotId)
        {
            if (runtime == null)
            {
                return false;
            }

            var selected = runtime.SelectedFeather;
            if (selected == null)
            {
                boardUI?.ShowNoSelectedFeather(runtime);
                return false;
            }

            var placed = runtime.TryPlaceSelected(slotId);
            if (!placed)
            {
                boardUI?.ShowCannotPlace(runtime, selected);
                return false;
            }

            boardUI?.ShowPlaced(runtime, slotId, selected);
            if (placed && runtime.State.IsSolved(solution) && endingController != null)
            {
                boardUI?.ShowSolved(runtime);
                endingController.PlayEnding();
            }

            return true;
        }
    }
}
