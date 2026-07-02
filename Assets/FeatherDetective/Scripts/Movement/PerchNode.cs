using System.Collections.Generic;
using UnityEngine;

namespace FeatherDetective
{
    public sealed class PerchNode : MonoBehaviour
    {
        [SerializeField] private Transform landingPoint;
        [SerializeField] private List<PerchNode> linkedPerches = new List<PerchNode>();

        public Vector3 LandingPosition => landingPoint != null ? landingPoint.position : transform.position;
        public IReadOnlyList<PerchNode> LinkedPerches => linkedPerches;

        public void ConfigureForTests(IEnumerable<PerchNode> links)
        {
            linkedPerches.Clear();
            linkedPerches.AddRange(links);
        }
    }
}
