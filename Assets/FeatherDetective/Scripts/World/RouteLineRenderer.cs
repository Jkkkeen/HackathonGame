using System.Collections;
using UnityEngine;

namespace FeatherDetective
{
    public sealed class RouteLineRenderer : MonoBehaviour
    {
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private float secondsPerSegment = 0.35f;

        private void Awake()
        {
            if (lineRenderer == null)
            {
                lineRenderer = GetComponent<LineRenderer>();
            }
        }

        public IEnumerator AnimateRoute(Vector3[] routePoints)
        {
            if (lineRenderer == null || routePoints == null || routePoints.Length == 0)
            {
                yield break;
            }

            lineRenderer.positionCount = 0;
            for (var i = 0; i < routePoints.Length; i++)
            {
                lineRenderer.positionCount = i + 1;
                lineRenderer.SetPosition(i, routePoints[i]);

                if (secondsPerSegment > 0f)
                {
                    yield return new WaitForSeconds(secondsPerSegment);
                }
            }
        }

        public void Clear()
        {
            if (lineRenderer != null)
            {
                lineRenderer.positionCount = 0;
            }
        }
    }
}
