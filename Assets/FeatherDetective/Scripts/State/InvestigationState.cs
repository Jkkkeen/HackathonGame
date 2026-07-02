using System.Collections.Generic;

namespace FeatherDetective
{
    public sealed class InvestigationState
    {
        private readonly HashSet<string> collectedIds = new HashSet<string>();
        private readonly Dictionary<DeductionSlotId, string> placements = new Dictionary<DeductionSlotId, string>();

        public int CollectedCount => collectedIds.Count;
        public IReadOnlyDictionary<DeductionSlotId, string> Placements => placements;

        public bool Collect(FeatherDefinition feather)
        {
            if (feather == null || string.IsNullOrWhiteSpace(feather.Id))
            {
                return false;
            }

            return collectedIds.Add(feather.Id);
        }

        public bool HasCollected(FeatherDefinition feather)
        {
            return feather != null && collectedIds.Contains(feather.Id);
        }

        public bool TryPlace(FeatherDefinition feather, DeductionSlotId slotId)
        {
            if (!HasCollected(feather) || !feather.CanPlaceInSlot(slotId))
            {
                return false;
            }

            placements[slotId] = feather.Id;
            return true;
        }

        public string GetPlacedFeatherId(DeductionSlotId slotId)
        {
            return placements.TryGetValue(slotId, out var featherId) ? featherId : string.Empty;
        }

        public bool IsSolved(DeductionSolution solution)
        {
            if (solution == null || solution.RequiredSlots.Length == 0)
            {
                return false;
            }

            foreach (var answer in solution.RequiredSlots)
            {
                if (answer.Feather == null)
                {
                    return false;
                }

                if (!placements.TryGetValue(answer.SlotId, out var placedId) || placedId != answer.Feather.Id)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
