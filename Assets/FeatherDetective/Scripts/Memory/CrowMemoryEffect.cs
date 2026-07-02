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
                yield return PlaySpatialCrowAudio(context.AudioSource);
            }

            yield return new WaitForSeconds(holdDuration);
            yield return FadeOverlay(context.DarkOverlay, context.DarkOverlay.alpha, previousAlpha, darkenDuration);
        }

        private static IEnumerator PlaySpatialCrowAudio(AudioSource audioSource)
        {
            var previousSpatialBlend = audioSource.spatialBlend;
            var previousPanStereo = audioSource.panStereo;
            var previousRolloffMode = audioSource.rolloffMode;
            var previousClip = audioSource.clip;
            var previousLoop = audioSource.loop;

            var clip = ProceduralAudio.CreateTone("Crow Memory", 220f, 0.25f, 0.12f);
            audioSource.spatialBlend = 1f;
            audioSource.panStereo = 0f;
            audioSource.rolloffMode = AudioRolloffMode.Linear;
            audioSource.PlayOneShot(clip);

            yield return new WaitForSeconds(clip.length);

            audioSource.spatialBlend = previousSpatialBlend;
            audioSource.panStereo = previousPanStereo;
            audioSource.rolloffMode = previousRolloffMode;
            audioSource.clip = previousClip;
            audioSource.loop = previousLoop;
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
