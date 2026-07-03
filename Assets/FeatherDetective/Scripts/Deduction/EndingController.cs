using UnityEngine;

namespace FeatherDetective
{
    public sealed class EndingController : MonoBehaviour
    {
        private const string EndingMessage = "He stayed at the hospital with his wife. The park was waiting with him.";

        [SerializeField] private EndingMessageUI endingMessage;
        [SerializeField] private Light sun;
        [SerializeField] private Color resolvedSkyColor = new Color(0.76f, 0.86f, 0.95f);

        public bool HasPlayed { get; private set; }

        public void ConfigureForBuilder(EndingMessageUI newEndingMessage, Light newSun)
        {
            endingMessage = newEndingMessage;
            sun = newSun;
        }

        public void PlayEnding()
        {
            if (HasPlayed)
            {
                return;
            }

            HasPlayed = true;
            RenderSettings.ambientLight = resolvedSkyColor;
            if (sun != null)
            {
                sun.intensity = 1.15f;
            }

            if (endingMessage != null)
            {
                endingMessage.Show(EndingMessage);
            }
        }
    }
}
