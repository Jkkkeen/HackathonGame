using UnityEngine;
using UnityEngine.UI;

namespace FeatherDetective
{
    public sealed class EndingMessageUI : MonoBehaviour
    {
        [SerializeField] private Text label;
        [SerializeField] private CanvasGroup group;

        public void ConfigureForBuilder(Text newLabel, CanvasGroup newGroup)
        {
            label = newLabel;
            group = newGroup;
            if (group != null)
            {
                group.alpha = 0f;
            }
        }

        public void Show(string message)
        {
            if (label != null)
            {
                label.text = message;
            }

            if (group != null)
            {
                group.alpha = 1f;
            }
        }
    }
}
