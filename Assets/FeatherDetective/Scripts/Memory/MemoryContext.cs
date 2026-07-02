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

        public CanvasGroup DarkOverlay => darkOverlay;
        public AudioSource AudioSource => audioSource;
        public ColorMemoryTarget[] ColorTargets => colorTargets;
        public RouteLineRenderer RouteRenderer => routeRenderer;
        public FirstPersonMemoryRig FirstPersonRig => firstPersonRig;
        public AtmosphereController AtmosphereController => atmosphereController;

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
        }
    }
}
