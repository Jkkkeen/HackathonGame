using UnityEngine;

namespace FeatherDetective
{
    public static class PerchSelector
    {
        public static PerchNode SelectBestReachable(PerchNode current, Vector3 birdPosition, Vector3 forward, float maxDistance, float minForwardDot)
        {
            if (current == null)
            {
                return null;
            }

            var bestScore = float.NegativeInfinity;
            PerchNode best = null;

            foreach (var perch in current.LinkedPerches)
            {
                if (perch == null)
                {
                    continue;
                }

                var offset = perch.LandingPosition - birdPosition;
                var distance = offset.magnitude;
                if (distance > maxDistance || distance <= 0.01f)
                {
                    continue;
                }

                var direction = offset.normalized;
                var dot = Vector3.Dot(forward.normalized, direction);
                if (dot < minForwardDot)
                {
                    continue;
                }

                var score = dot * 2f - distance * 0.05f;
                if (score > bestScore)
                {
                    bestScore = score;
                    best = perch;
                }
            }

            return best;
        }
    }
}
