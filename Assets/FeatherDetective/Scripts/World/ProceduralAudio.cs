using UnityEngine;

namespace FeatherDetective
{
    public static class ProceduralAudio
    {
        private const int SampleRate = 44100;

        public static AudioClip CreateTone(string name, float frequency, float duration, float volume)
        {
            var sampleCount = Mathf.Max(1, Mathf.CeilToInt(SampleRate * Mathf.Max(0.01f, duration)));
            var samples = new float[sampleCount];

            for (var i = 0; i < sampleCount; i++)
            {
                var time = (float)i / SampleRate;
                samples[i] = Mathf.Sin(time * frequency * Mathf.PI * 2f) * volume;
            }

            var clip = AudioClip.Create(name, sampleCount, 1, SampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        public static AudioClip CreateSoftNoise(string name, float duration, float volume)
        {
            var sampleCount = Mathf.Max(1, Mathf.CeilToInt(SampleRate * Mathf.Max(0.01f, duration)));
            var samples = new float[sampleCount];

            for (var i = 0; i < sampleCount; i++)
            {
                samples[i] = Random.Range(-volume, volume);
            }

            var clip = AudioClip.Create(name, sampleCount, 1, SampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }
    }
}
