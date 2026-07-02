using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FeatherDetective
{
    public sealed class MemoryPlaybackController : MonoBehaviour
    {
        [SerializeField] private MemoryContext context;

        private readonly List<IMemoryEffect> effects = new List<IMemoryEffect>();
        private Coroutine activeRoutine;

        public MemoryContext Context => context;

        private void Awake()
        {
            if (context == null)
            {
                context = GetComponent<MemoryContext>();
            }

            RefreshEffects();
        }

        public void PlayMemory(FeatherDefinition feather)
        {
            if (activeRoutine != null)
            {
                return;
            }

            activeRoutine = StartCoroutine(PlayRoutine(feather));
        }

        public IEnumerator PlayRoutine(FeatherDefinition feather)
        {
            if (feather == null)
            {
                activeRoutine = null;
                yield break;
            }

            if (effects.Count == 0)
            {
                RefreshEffects();
            }

            var effect = SelectEffect(effects, feather.Species);
            if (effect == null)
            {
                activeRoutine = null;
                yield break;
            }

            yield return effect.Play(feather, context);
            activeRoutine = null;
        }

        public static IMemoryEffect SelectEffectForTests(IEnumerable<IMemoryEffect> availableEffects, BirdSpecies species)
        {
            return SelectEffect(availableEffects, species);
        }

        private void RefreshEffects()
        {
            effects.Clear();

            var behaviours = GetComponents<MonoBehaviour>();
            for (var i = 0; i < behaviours.Length; i++)
            {
                if (behaviours[i] is IMemoryEffect effect)
                {
                    effects.Add(effect);
                }
            }
        }

        private static IMemoryEffect SelectEffect(IEnumerable<IMemoryEffect> availableEffects, BirdSpecies species)
        {
            if (availableEffects == null)
            {
                return null;
            }

            foreach (var effect in availableEffects)
            {
                if (effect != null && effect.Species == species)
                {
                    return effect;
                }
            }

            return null;
        }
    }
}
