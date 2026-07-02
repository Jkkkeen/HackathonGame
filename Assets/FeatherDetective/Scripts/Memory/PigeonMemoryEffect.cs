using System.Collections;
using UnityEngine;

namespace FeatherDetective
{
    public sealed class PigeonMemoryEffect : MonoBehaviour, IMemoryEffect
    {
        public BirdSpecies Species => BirdSpecies.Pigeon;

        public IEnumerator Play(FeatherDefinition feather, MemoryContext context)
        {
            if (feather == null || context == null || context.RouteRenderer == null)
            {
                yield break;
            }

            if (context.AudioSource != null)
            {
                context.AudioSource.PlayOneShot(ProceduralAudio.CreateTone("Pigeon Memory", 330f, 0.3f, 0.07f));
            }

            yield return context.RouteRenderer.AnimateRoute(feather.RoutePoints);
        }
    }
}
