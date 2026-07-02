using UnityEngine;

namespace FeatherDetective
{
    public sealed class MemoryContext : MonoBehaviour
    {
        [SerializeField] private CanvasGroup darkOverlay;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private ColorMemoryTarget[] colorTargets = new ColorMemoryTarget[0];
        [SerializeField] private RouteLineRenderer routeRenderer;
        [SerializeField] private FirstPersonMemoryRig firstPersonRig;
        [SerializeField] private AtmosphereController atmosphereController;

        private float defaultOverlayAlpha;
        private float defaultAudioSpatialBlend;
        private float defaultAudioPanStereo;
        private AudioRolloffMode defaultAudioRolloffMode;
        private AudioClip defaultAudioClip;
        private bool defaultAudioLoop;
        private bool hasDefaults;

        public CanvasGroup DarkOverlay => darkOverlay;
        public AudioSource AudioSource => audioSource;
        public ColorMemoryTarget[] ColorTargets => colorTargets;
        public RouteLineRenderer RouteRenderer => routeRenderer;
        public FirstPersonMemoryRig FirstPersonRig => firstPersonRig;
        public AtmosphereController AtmosphereController => atmosphereController;

        private void Awake()
        {
            CaptureDefaults();
        }

        public void ConfigureForBuilder(
            CanvasGroup newDarkOverlay,
            AudioSource newAudioSource,
            ColorMemoryTarget[] newColorTargets,
            RouteLineRenderer newRouteRenderer,
            FirstPersonMemoryRig newFirstPersonRig,
            AtmosphereController newAtmosphereController)
        {
            darkOverlay = newDarkOverlay;
            audioSource = newAudioSource;
            colorTargets = newColorTargets ?? new ColorMemoryTarget[0];
            routeRenderer = newRouteRenderer;
            firstPersonRig = newFirstPersonRig;
            atmosphereController = newAtmosphereController;
            CaptureDefaults(true);
        }

        public void RestoreAfterMemory()
        {
            CaptureDefaults(false);

            if (darkOverlay != null)
            {
                darkOverlay.alpha = defaultOverlayAlpha;
            }

            if (audioSource != null)
            {
                audioSource.Stop();
                audioSource.spatialBlend = defaultAudioSpatialBlend;
                audioSource.panStereo = defaultAudioPanStereo;
                audioSource.rolloffMode = defaultAudioRolloffMode;
                audioSource.clip = defaultAudioClip;
                audioSource.loop = defaultAudioLoop;
            }

            if (colorTargets != null)
            {
                for (var i = 0; i < colorTargets.Length; i++)
                {
                    if (colorTargets[i] != null)
                    {
                        colorTargets[i].Restore();
                    }
                }
            }

            if (routeRenderer != null)
            {
                routeRenderer.Clear();
            }

            if (firstPersonRig != null)
            {
                firstPersonRig.Restore();
            }

            if (atmosphereController != null)
            {
                atmosphereController.Restore();
            }
        }

        private void CaptureDefaults(bool force = false)
        {
            if (hasDefaults && !force)
            {
                return;
            }

            defaultOverlayAlpha = darkOverlay != null ? darkOverlay.alpha : 0f;
            defaultAudioSpatialBlend = audioSource != null ? audioSource.spatialBlend : 0f;
            defaultAudioPanStereo = audioSource != null ? audioSource.panStereo : 0f;
            defaultAudioRolloffMode = audioSource != null ? audioSource.rolloffMode : AudioRolloffMode.Logarithmic;
            defaultAudioClip = audioSource != null ? audioSource.clip : null;
            defaultAudioLoop = audioSource != null && audioSource.loop;
            hasDefaults = true;
        }
    }
}
