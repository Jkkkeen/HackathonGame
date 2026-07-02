using System.Collections;
using UnityEngine;

namespace FeatherDetective
{
    public sealed class FirstPersonMemoryRig : MonoBehaviour
    {
        [SerializeField] private Camera memoryCamera;
        [SerializeField] private AudioSource breathSource;
        [SerializeField] private float bobHeight = 0.08f;
        [SerializeField] private float bobFrequency = 2f;

        private Vector3 originalLocalPosition;
        private bool hasOriginalPosition;

        private void Awake()
        {
            CaptureOriginalPosition();
        }

        public void ConfigureForBuilder(Camera newMemoryCamera, AudioSource newImpactAudioSource)
        {
            memoryCamera = newMemoryCamera;
            breathSource = newImpactAudioSource;

            if (memoryCamera != null)
            {
                memoryCamera.enabled = false;
            }
        }

        public IEnumerator PlayRhythm(float duration)
        {
            CaptureOriginalPosition();

            if (memoryCamera != null)
            {
                memoryCamera.enabled = true;
            }

            var elapsed = 0f;
            var nextPulseTime = 0f;
            var pulseInterval = bobFrequency > 0f ? 1f / bobFrequency : duration;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                if (breathSource != null && elapsed >= nextPulseTime)
                {
                    breathSource.Play();
                    nextPulseTime += pulseInterval;
                }

                var offset = Mathf.Sin(elapsed * Mathf.PI * 2f * bobFrequency) * bobHeight;
                transform.localPosition = originalLocalPosition + Vector3.up * offset;
                yield return null;
            }

            Restore();
        }

        public void Restore()
        {
            if (hasOriginalPosition)
            {
                transform.localPosition = originalLocalPosition;
            }

            if (breathSource != null)
            {
                breathSource.Stop();
            }

            if (memoryCamera != null)
            {
                memoryCamera.enabled = false;
            }
        }

        private void CaptureOriginalPosition()
        {
            if (hasOriginalPosition)
            {
                return;
            }

            originalLocalPosition = transform.localPosition;
            hasOriginalPosition = true;

            if (memoryCamera != null)
            {
                memoryCamera.enabled = false;
            }
        }
    }
}
