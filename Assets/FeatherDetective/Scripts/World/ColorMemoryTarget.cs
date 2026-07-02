using UnityEngine;

namespace FeatherDetective
{
    public sealed class ColorMemoryTarget : MonoBehaviour
    {
        [SerializeField] private Color memoryColor = Color.white;

        private Renderer cachedRenderer;
        private Color originalColor;
        private bool hasOriginalColor;
        private bool hasExplicitMemoryColor;

        public Color MemoryColor => memoryColor;

        private void Awake()
        {
            CaptureRendererState();
        }

        public void ConfigureForBuilder(Color newMemoryColor)
        {
            memoryColor = newMemoryColor;
            hasExplicitMemoryColor = true;
            CaptureRendererState(true);
        }

        public void ApplyMagpieState(Color[] importantColors)
        {
            if (cachedRenderer == null)
            {
                return;
            }

            if (IsImportantColor(importantColors))
            {
                cachedRenderer.material.color = originalColor;
                return;
            }

            var grayscale = originalColor.grayscale;
            cachedRenderer.material.color = new Color(grayscale, grayscale, grayscale, originalColor.a);
        }

        public void Restore()
        {
            if (cachedRenderer != null)
            {
                cachedRenderer.material.color = originalColor;
            }
        }

        private bool IsImportantColor(Color[] importantColors)
        {
            CaptureRendererState();

            if (importantColors == null)
            {
                return false;
            }

            var targetColor = (Vector4)memoryColor;
            for (var i = 0; i < importantColors.Length; i++)
            {
                if (Vector4.Distance(targetColor, importantColors[i]) < 0.08f)
                {
                    return true;
                }
            }

            return false;
        }

        private void CaptureRendererState(bool forceOriginalColor = false)
        {
            if (cachedRenderer == null)
            {
                cachedRenderer = GetComponent<Renderer>();
            }

            if (cachedRenderer == null)
            {
                if (!hasOriginalColor)
                {
                    originalColor = Color.white;
                    hasOriginalColor = true;
                }

                return;
            }

            if (!hasOriginalColor || forceOriginalColor)
            {
                originalColor = cachedRenderer.material.color;
                hasOriginalColor = true;
            }

            if (!hasExplicitMemoryColor)
            {
                memoryColor = originalColor;
            }
        }
    }
}
