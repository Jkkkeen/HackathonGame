using UnityEngine;

namespace FeatherDetective
{
    public sealed class ColorMemoryTarget : MonoBehaviour
    {
        [SerializeField] private Renderer targetRenderer;

        private Color originalColor = Color.white;
        private bool hasOriginalColor;

        private void Awake()
        {
            if (targetRenderer == null)
            {
                targetRenderer = GetComponent<Renderer>();
            }

            CaptureOriginalColor();
        }

        public void ApplyMagpieState(Color color)
        {
            CaptureOriginalColor();

            if (targetRenderer != null)
            {
                targetRenderer.material.color = color;
            }
        }

        public void Restore()
        {
            if (targetRenderer != null && hasOriginalColor)
            {
                targetRenderer.material.color = originalColor;
            }
        }

        private void CaptureOriginalColor()
        {
            if (hasOriginalColor || targetRenderer == null)
            {
                return;
            }

            originalColor = targetRenderer.material.color;
            hasOriginalColor = true;
        }
    }
}
