using UnityEngine;

namespace FeatherDetective
{
    public sealed class FixedThirdPersonCamera : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset = new Vector3(0f, 8f, -9f);
        [SerializeField] private float followSharpness = 8f;

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }

        private void LateUpdate()
        {
            if (target == null)
            {
                return;
            }

            var desired = target.position + offset;
            transform.position = Vector3.Lerp(transform.position, desired, 1f - Mathf.Exp(-followSharpness * Time.deltaTime));
            transform.rotation = Quaternion.Euler(48f, 0f, 0f);
        }
    }
}
