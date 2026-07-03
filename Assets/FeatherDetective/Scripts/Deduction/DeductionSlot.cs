using UnityEngine;

namespace FeatherDetective
{
    public sealed class DeductionSlot : MonoBehaviour
    {
        [SerializeField] private DeductionSlotId slotId;

        public DeductionSlotId SlotId => slotId;

        public void ConfigureForBuilder(DeductionSlotId newSlotId)
        {
            slotId = newSlotId;
        }
    }
}
