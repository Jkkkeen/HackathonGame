using UnityEngine;
using UnityEngine.UI;

namespace FeatherDetective
{
    public sealed class DeductionBoardUI : MonoBehaviour
    {
        [SerializeField] private Text label;

        public void ConfigureForBuilder(Text newLabel)
        {
            label = newLabel;
        }

        public void ShowBoardHint()
        {
            if (label != null)
            {
                label.text = "Nest board";
            }
        }
    }
}
