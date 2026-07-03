using UnityEngine;
using UnityEngine.UI;

namespace FeatherDetective
{
    public sealed class DeductionBoardUI : MonoBehaviour
    {
        private static readonly DeductionSlotId[] DisplaySlots =
        {
            DeductionSlotId.LastFeedingDay,
            DeductionSlotId.WhatChanged,
            DeductionSlotId.WhereHeWent,
            DeductionSlotId.WhyBirdsWaited,
            DeductionSlotId.WhatEvidenceMeans
        };

        [SerializeField] private Text label;

        public void ConfigureForBuilder(Text newLabel)
        {
            label = newLabel;
        }

        public void ShowBoardHint()
        {
            if (label != null)
            {
                label.text = "Nest board\nBring a feather close and press E.";
            }
        }

        public void ShowNoSelectedFeather(InvestigationRuntime runtime)
        {
            Refresh(runtime, "No feather selected. Press Tab after collecting feathers.");
        }

        public void ShowCannotPlace(InvestigationRuntime runtime, FeatherDefinition feather)
        {
            var featherName = feather != null ? feather.DisplayName : "This feather";
            Refresh(runtime, $"{featherName} does not fit an open clue slot.");
        }

        public void ShowPlaced(InvestigationRuntime runtime, DeductionSlotId slotId, FeatherDefinition feather)
        {
            var featherName = feather != null ? feather.DisplayName : "Selected feather";
            Refresh(runtime, $"Placed {featherName} in {FormatSlot(slotId)}.");
        }

        public void ShowSolved(InvestigationRuntime runtime)
        {
            Refresh(runtime, "The quiet answer settles into the nest.");
        }

        private void Refresh(InvestigationRuntime runtime, string status)
        {
            if (label == null)
            {
                return;
            }

            var text = "Nest board";
            if (!string.IsNullOrWhiteSpace(status))
            {
                text += $"\n{status}";
            }

            if (runtime != null)
            {
                for (var i = 0; i < DisplaySlots.Length; i++)
                {
                    var slotId = DisplaySlots[i];
                    var placedId = runtime.State.GetPlacedFeatherId(slotId);
                    var placedName = ResolveCollectedName(runtime, placedId);
                    text += $"\n{FormatSlot(slotId)}: {placedName}";
                }
            }

            label.text = text;
        }

        private static string ResolveCollectedName(InvestigationRuntime runtime, string featherId)
        {
            if (string.IsNullOrWhiteSpace(featherId))
            {
                return "-";
            }

            foreach (var feather in runtime.CollectedFeathers)
            {
                if (feather != null && feather.Id == featherId)
                {
                    return feather.DisplayName;
                }
            }

            return featherId;
        }

        private static string FormatSlot(DeductionSlotId slotId)
        {
            switch (slotId)
            {
                case DeductionSlotId.LastFeedingDay:
                    return "Last feeding day";
                case DeductionSlotId.WhatChanged:
                    return "What changed";
                case DeductionSlotId.WhereHeWent:
                    return "Where he went";
                case DeductionSlotId.WhyBirdsWaited:
                    return "Why birds waited";
                case DeductionSlotId.WhatEvidenceMeans:
                    return "What evidence means";
                default:
                    return slotId.ToString();
            }
        }
    }
}
