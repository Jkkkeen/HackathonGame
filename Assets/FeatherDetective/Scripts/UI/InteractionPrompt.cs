using UnityEngine;
using UnityEngine.UI;

namespace FeatherDetective
{
    public sealed class InteractionPrompt : MonoBehaviour
    {
        [SerializeField] private Text label;

        public void ConfigureForBuilder(Text newLabel)
        {
            label = newLabel;
            Hide();
        }

        public void Show(string text)
        {
            if (label == null)
            {
                return;
            }

            label.text = text;
            label.enabled = true;
        }

        public void Hide()
        {
            if (label != null)
            {
                label.enabled = false;
            }
        }
    }
}
