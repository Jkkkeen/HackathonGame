using System.Collections;
using UnityEngine;

namespace FeatherDetective
{
    public sealed class WoodpeckerMemoryEffect : MonoBehaviour, IMemoryEffect
    {
        [SerializeField] private float duration = 1.8f;

        public BirdSpecies Species => BirdSpecies.Woodpecker;

        public IEnumerator Play(FeatherDefinition feather, MemoryContext context)
        {
            if (context == null || context.FirstPersonRig == null)
            {
                yield break;
            }

            if (context.AudioSource != null)
            {
                context.AudioSource.PlayOneShot(ProceduralAudio.CreateTone("Woodpecker Memory", 120f, 0.15f, 0.12f));
            }

            yield return context.FirstPersonRig.PlayRhythm(duration);
        }
    }
}
