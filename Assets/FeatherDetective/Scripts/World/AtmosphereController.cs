using System.Collections;
using UnityEngine;

namespace FeatherDetective
{
    public sealed class AtmosphereController : MonoBehaviour
    {
        [SerializeField] private AudioSource windSource;
        [SerializeField] private AudioSource insectSource;
        [SerializeField] private AudioSource flockSource;
        [SerializeField] private float shiftDuration = 2f;

        private float originalWindVolume;
        private float originalInsectVolume;
        private float originalFlockVolume;
        private bool configured;

        private void Awake()
        {
            Configure();
        }

        public void Configure()
        {
            if (configured)
            {
                return;
            }

            originalWindVolume = windSource != null ? windSource.volume : 0f;
            originalInsectVolume = insectSource != null ? insectSource.volume : 0f;
            originalFlockVolume = flockSource != null ? flockSource.volume : 0f;

            ConfigureSource(windSource, ProceduralAudio.CreateSoftNoise("Memory Wind", 1f, 0.08f), true);
            ConfigureSource(insectSource, ProceduralAudio.CreateTone("Memory Insects", 880f, 0.5f, 0.02f), true);
            ConfigureSource(flockSource, ProceduralAudio.CreateTone("Memory Flock", 440f, 0.8f, 0.03f), true);

            configured = true;
        }

        public IEnumerator ShiftForSparrow()
        {
            Configure();

            yield return FadeAtmosphere(originalWindVolume * 0.3f, originalInsectVolume * 1.6f, originalFlockVolume * 0.2f, shiftDuration);
            yield return new WaitForSeconds(0.5f);
            yield return FadeAtmosphere(originalWindVolume, originalInsectVolume, originalFlockVolume, shiftDuration);
        }

        private IEnumerator FadeAtmosphere(float windVolume, float insectVolume, float flockVolume, float duration)
        {
            var startWind = windSource != null ? windSource.volume : 0f;
            var startInsect = insectSource != null ? insectSource.volume : 0f;
            var startFlock = flockSource != null ? flockSource.volume : 0f;

            var elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                var t = duration <= 0f ? 1f : Mathf.Clamp01(elapsed / duration);
                SetVolumes(
                    Mathf.Lerp(startWind, windVolume, t),
                    Mathf.Lerp(startInsect, insectVolume, t),
                    Mathf.Lerp(startFlock, flockVolume, t));
                yield return null;
            }

            SetVolumes(windVolume, insectVolume, flockVolume);
        }

        private void SetVolumes(float windVolume, float insectVolume, float flockVolume)
        {
            if (windSource != null)
            {
                windSource.volume = windVolume;
            }

            if (insectSource != null)
            {
                insectSource.volume = insectVolume;
            }

            if (flockSource != null)
            {
                flockSource.volume = flockVolume;
            }
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
        }
    }
}
