using UnityEngine;

namespace FeatherDetective
{
    public sealed class FeatherPickup : MonoBehaviour, IInspectable
    {
        [SerializeField] private FeatherDefinition definition;
        [SerializeField] private InvestigationRuntime runtime;
        [SerializeField] private GameObject collectedVisual;

        public FeatherDefinition Definition => definition;
        public string PromptText => "Inspect";

        public void ConfigureForBuilder(FeatherDefinition newDefinition, InvestigationRuntime newRuntime)
        {
            definition = newDefinition;
            runtime = newRuntime;
        }

        public void Inspect()
        {
            if (definition == null || runtime == null)
            {
                return;
            }

            var collected = runtime.CollectFeather(definition);
            if (collected && collectedVisual != null)
            {
                collectedVisual.SetActive(false);
            }
        }
    }
}
