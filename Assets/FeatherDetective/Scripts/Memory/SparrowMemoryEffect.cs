using System.Collections;
using UnityEngine;

namespace FeatherDetective
{
    public sealed class SparrowMemoryEffect : MonoBehaviour, IMemoryEffect
    {
        [SerializeField] private float holdDuration = 0.8f;

        public BirdSpecies Species => BirdSpecies.Sparrow;

        public IEnumerator Play(FeatherDefinition feather, MemoryContext context)
        {
            if (context == null)
            {
                yield break;
            }

            if (context.AudioSource != null)
            {
                context.AudioSource.PlayOneShot(ProceduralAudio.CreateTone("Sparrow Memory", 1040f, 0.2f, 0.05f));
            }

            if (context.AtmosphereController != null)
            {
                yield return context.AtmosphereController.ShiftSparrowAtmosphere(holdDuration);
            }
            else
            {
                yield return new WaitForSeconds(holdDuration);
            }
        }
    }
}
