using System.Collections;
using UnityEngine;

namespace FeatherDetective
{
    public sealed class AtmosphereController : MonoBehaviour
    {
        [SerializeField] private AudioSource windSource;
        [SerializeField] private AudioSource insectSource;
        [SerializeField] private Transform flockRoot;

        private float originalWindVolume;
        private float originalInsectVolume;
        private Vector3 originalFlockScale;

        private void Awake()
        {
            CaptureOriginalState();
            ConfigureSources();
        }

        public void ConfigureForBuilder(AudioSource newWindSource, AudioSource newInsectSource, Transform newFlockRoot)
        {
            windSource = newWindSource;
            insectSource = newInsectSource;
            flockRoot = newFlockRoot;

            CaptureOriginalState();
            ConfigureSources();
        }

        public IEnumerator ShiftSparrowAtmosphere(float duration)
        {
            CaptureOriginalState();

            var targetWindVolume = originalWindVolume * 0.3f;
            var targetInsectVolume = originalInsectVolume * 1.6f;
            var targetFlockScale = originalFlockScale * 1.35f;

            yield return ShiftAtmosphere(targetWindVolume, targetInsectVolume, targetFlockScale, duration);
            yield return ShiftAtmosphere(originalWindVolume, originalInsectVolume, originalFlockScale, duration);
        }

        public void Restore()
        {
            SetAtmosphere(originalWindVolume, originalInsectVolume, originalFlockScale);
        }

        private IEnumerator ShiftAtmosphere(float windVolume, float insectVolume, Vector3 flockScale, float duration)
        {
            var startWind = windSource != null ? windSource.volume : 0f;
            var startInsect = insectSource != null ? insectSource.volume : 0f;
            var startFlockScale = flockRoot != null ? flockRoot.localScale : Vector3.one;

            var elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                var t = duration <= 0f ? 1f : Mathf.Clamp01(elapsed / duration);
                SetAtmosphere(
                    Mathf.Lerp(startWind, windVolume, t),
                    Mathf.Lerp(startInsect, insectVolume, t),
                    Vector3.Lerp(startFlockScale, flockScale, t));
                yield return null;
            }

            SetAtmosphere(windVolume, insectVolume, flockScale);
        }

        private void SetAtmosphere(float windVolume, float insectVolume, Vector3 flockScale)
        {
            if (windSource != null)
            {
                windSource.volume = windVolume;
            }

            if (insectSource != null)
            {
                insectSource.volume = insectVolume;
            }

            if (flockRoot != null)
            {
                flockRoot.localScale = flockScale;
            }
        }

        private void CaptureOriginalState()
        {
            originalWindVolume = windSource != null ? windSource.volume : 0f;
            originalInsectVolume = insectSource != null ? insectSource.volume : 0f;
            originalFlockScale = flockRoot != null ? flockRoot.localScale : Vector3.one;
        }

        private void ConfigureSources()
        {
            ConfigureSource(windSource, ProceduralAudio.CreateSoftNoise("Memory Wind", 1f, 0.08f), true);
            ConfigureSource(insectSource, ProceduralAudio.CreateTone("Memory Insects", 880f, 0.5f, 0.02f), true);
        }

        private static void ConfigureSource(AudioSource source, AudioClip clip, bool loop)
        {
            if (source == null)
            {
                return;
            }

            if (source.clip == null)
            {
                source.clip = clip;
            }

            source.loop = loop;
            if (!source.isPlaying)
            {
                source.Play();
            }
        }
    }
}
