using System;
using UnityEngine;

namespace FeatherDetective
{
    [Serializable]
    public struct DeductionAnswer
    {
        [SerializeField] private DeductionSlotId slotId;
        [SerializeField] private FeatherDefinition feather;

        public DeductionAnswer(DeductionSlotId slotId, FeatherDefinition feather)
        {
            this.slotId = slotId;
            this.feather = feather;
        }

        public DeductionSlotId SlotId => slotId;
        public FeatherDefinition Feather => feather;
    }
}
