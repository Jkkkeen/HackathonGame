using System;
using UnityEngine;

namespace FeatherDetective
{
    [CreateAssetMenu(menuName = "Feather Detective/Deduction Solution")]
    public sealed class DeductionSolution : ScriptableObject
    {
        [SerializeField] private DeductionAnswer[] requiredAnswers = Array.Empty<DeductionAnswer>();

        public DeductionAnswer[] RequiredSlots => requiredAnswers;

        public bool IsRequiredAnswer(DeductionSlotId slotId, FeatherDefinition feather)
        {
            if (feather == null)
            {
                return false;
            }

            foreach (var answer in requiredAnswers)
            {
                if (answer.SlotId == slotId && answer.Feather != null && answer.Feather.Id == feather.Id)
                {
                    return true;
                }
            }

            return false;
        }

        public void ConfigureForTests(DeductionAnswer[] answers)
        {
            requiredAnswers = answers ?? Array.Empty<DeductionAnswer>();
        }
    }
}
