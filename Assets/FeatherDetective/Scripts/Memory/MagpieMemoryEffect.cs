using System.Collections;
using UnityEngine;

namespace FeatherDetective
{
    public sealed class MagpieMemoryEffect : MonoBehaviour, IMemoryEffect
    {
        [SerializeField] private float colorHoldDuration = 1.5f;

        public BirdSpecies Species => BirdSpecies.Magpie;

        public IEnumerator Play(FeatherDefinition feather, MemoryContext context)
        {
            if (feather == null || context == null)
            {
                yield break;
            }

            var targets = context.ColorTargets;
            var colors = feather.HighlightColors;

            if (targets != null && colors != null && colors.Length > 0)
            {
                for (var i = 0; i < targets.Length; i++)
                {
                    if (targets[i] != null)
                    {
                        targets[i].ApplyMagpieState(colors[i % colors.Length]);
                    }
                }
            }

            if (context.AudioSource != null)
            {
                context.AudioSource.PlayOneShot(ProceduralAudio.CreateTone("Magpie Memory", 660f, 0.2f, 0.08f));
            }

            yield return new WaitForSeconds(colorHoldDuration);

            if (targets == null)
            {
                yield break;
            }

            for (var i = 0; i < targets.Length; i++)
            {
                if (targets[i] != null)
                {
                    targets[i].Restore();
                }
            }
        }
    }
}
