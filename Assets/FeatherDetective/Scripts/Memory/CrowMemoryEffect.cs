using System.Collections;
using UnityEngine;

namespace FeatherDetective
{
    public sealed class CrowMemoryEffect : MonoBehaviour, IMemoryEffect
    {
        [SerializeField] private float darkenDuration = 0.4f;
        [SerializeField] private float holdDuration = 0.7f;

        public BirdSpecies Species => BirdSpecies.Crow;

        public IEnumerator Play(FeatherDefinition feather, MemoryContext context)
        {
            if (context == null || context.DarkOverlay == null)
            {
                yield break;
            }

            var previousAlpha = context.DarkOverlay.alpha;
            yield return FadeOverlay(context.DarkOverlay, context.DarkOverlay.alpha, 0.85f, darkenDuration);

            if (context.AudioSource != null)
            {
                context.AudioSource.spatialBlend = 1f;
                context.AudioSource.panStereo = 0f;
                context.AudioSource.rolloffMode = AudioRolloffMode.Linear;
                context.AudioSource.PlayOneShot(ProceduralAudio.CreateTone("Crow Memory", 220f, 0.25f, 0.12f));
            }

            yield return new WaitForSeconds(holdDuration);
            yield return FadeOverlay(context.DarkOverlay, context.DarkOverlay.alpha, previousAlpha, darkenDuration);
        }

        private static IEnumerator FadeOverlay(CanvasGroup overlay, float from, float to, float duration)
        {
            var elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                overlay.alpha = Mathf.Lerp(from, to, duration <= 0f ? 1f : Mathf.Clamp01(elapsed / duration));
                yield return null;
            }

            overlay.alpha = to;
        }
    }
}
