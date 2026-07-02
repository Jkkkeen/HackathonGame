using System;
using UnityEngine;

namespace FeatherDetective
{
    [CreateAssetMenu(menuName = "Feather Detective/Feather Definition")]
    public sealed class FeatherDefinition : ScriptableObject
    {
        [SerializeField] private string id = string.Empty;
        [SerializeField] private string displayName = string.Empty;
        [SerializeField] private BirdSpecies species;
        [SerializeField] private ClueRole role;
        [SerializeField] private DeductionSlotId[] compatibleSlots = Array.Empty<DeductionSlotId>();
        [SerializeField] private Color[] highlightColors = Array.Empty<Color>();
        [SerializeField] private Vector3[] routePoints = Array.Empty<Vector3>();
        [SerializeField] private string memorySummary = string.Empty;

        public string Id => id;
        public string DisplayName => displayName;
        public BirdSpecies Species => species;
        public ClueRole Role => role;
        public DeductionSlotId[] CompatibleSlots => compatibleSlots;
        public Color[] HighlightColors => highlightColors;
        public Vector3[] RoutePoints => routePoints;
        public string MemorySummary => memorySummary;

        public bool CanPlaceInSlot(DeductionSlotId slotId)
        {
            return Array.IndexOf(compatibleSlots, slotId) >= 0;
        }

        public void ConfigureForTests(
            string newId,
            string newDisplayName,
            BirdSpecies newSpecies,
            ClueRole newRole,
            DeductionSlotId[] newCompatibleSlots,
            Color[] newHighlightColors,
            Vector3[] newRoutePoints,
            string newMemorySummary)
        {
            id = newId;
            displayName = newDisplayName;
            species = newSpecies;
            role = newRole;
            compatibleSlots = newCompatibleSlots ?? Array.Empty<DeductionSlotId>();
            highlightColors = newHighlightColors ?? Array.Empty<Color>();
            routePoints = newRoutePoints ?? Array.Empty<Vector3>();
            memorySummary = newMemorySummary;
        }
    }
}
