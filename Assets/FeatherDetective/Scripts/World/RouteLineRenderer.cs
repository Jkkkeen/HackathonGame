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

        public IEnumerator AnimateRoute(Vector3[] points)
        {
            if (lineRenderer == null || points == null || points.Length < 2)
            {
                yield break;
            }

            var originalStartWidth = lineRenderer.startWidth;
            var originalEndWidth = lineRenderer.endWidth;

            lineRenderer.enabled = true;
            lineRenderer.positionCount = points.Length;
            lineRenderer.SetPositions(points);
            lineRenderer.startWidth = 0f;
            lineRenderer.endWidth = 0f;

            var elapsed = 0f;
            var duration = secondsPerSegment * (points.Length - 1);
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                var t = duration <= 0f ? 1f : Mathf.Clamp01(elapsed / duration);
                lineRenderer.startWidth = Mathf.Lerp(0f, originalStartWidth, t);
                lineRenderer.endWidth = Mathf.Lerp(0f, originalEndWidth, t);
                yield return null;
            }

            lineRenderer.startWidth = originalStartWidth;
            lineRenderer.endWidth = originalEndWidth;

            if (secondsPerSegment > 0f)
            {
                yield return new WaitForSeconds(Mathf.Min(0.2f, secondsPerSegment));
            }

            lineRenderer.enabled = false;
        }

        public void Clear()
        {
            if (lineRenderer != null)
            {
                lineRenderer.positionCount = 0;
                lineRenderer.enabled = false;
            }
        }
    }
}
