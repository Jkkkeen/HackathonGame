using System.Collections;
using UnityEngine;

namespace FeatherDetective
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class BirdPlayerController : MonoBehaviour
    {
        [SerializeField] private float walkSpeed = 3.5f;
        [SerializeField] private float hopSpeed = 4.8f;
        [SerializeField] private float gravity = -14f;
        [SerializeField] private float glideDuration = 0.8f;
        [SerializeField] private float glideRange = 7f;
        [SerializeField] private Transform visualRoot;
        [SerializeField] private InteractionPrompt prompt;

        private CharacterController controller;
        private Vector3 velocity;
        private PerchNode currentPerch;
        private IInspectable focusedInspectable;
        private bool gliding;

        public void ConfigureForBuilder(InteractionPrompt newPrompt, PerchNode startPerch)
        {
            prompt = newPrompt;
            currentPerch = startPerch;
            if (startPerch != null)
            {
                transform.position = startPerch.LandingPosition;
            }
        }

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
        }

        private void Update()
        {
            if (gliding)
            {
                return;
            }

            var input = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
            if (input.sqrMagnitude > 1f)
            {
                input.Normalize();
            }

            var move = input * walkSpeed;
            controller.Move(move * Time.deltaTime);

            if (input.sqrMagnitude > 0.001f)
            {
                transform.rotation = Quaternion.LookRotation(input, Vector3.up);
            }

            if (controller.isGrounded && velocity.y < 0f)
            {
                velocity.y = -1f;
            }

            if (controller.isGrounded && Input.GetKeyDown(KeyCode.Space))
            {
                velocity.y = hopSpeed;
            }

            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);

            if (Input.GetKeyDown(KeyCode.E) && focusedInspectable != null)
            {
                focusedInspectable.Inspect();
            }

            if (Input.GetKeyDown(KeyCode.G))
            {
                var target = PerchSelector.SelectBestReachable(currentPerch, transform.position, transform.forward, glideRange, 0.1f);
                if (target != null)
                {
                    StartCoroutine(GlideTo(target));
                }
            }
        }

        private IEnumerator GlideTo(PerchNode target)
        {
            gliding = true;
            var start = transform.position;
            var end = target.LandingPosition;
            var elapsed = 0f;

            while (elapsed < glideDuration)
            {
                elapsed += Time.deltaTime;
                var t = Mathf.Clamp01(elapsed / glideDuration);
                var arc = Mathf.Sin(t * Mathf.PI) * 0.8f;
                transform.position = Vector3.Lerp(start, end, t) + Vector3.up * arc;
                yield return null;
            }

            transform.position = end;
            currentPerch = target;
            velocity = Vector3.zero;
            gliding = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out PerchNode perch))
            {
                currentPerch = perch;
            }

            if (TryGetInspectable(other, out var inspectable))
            {
                focusedInspectable = inspectable;
                prompt?.Show(inspectable.PromptText);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out PerchNode perch) && ReferenceEquals(perch, currentPerch))
            {
                currentPerch = null;
            }

            if (TryGetInspectable(other, out var inspectable) && ReferenceEquals(inspectable, focusedInspectable))
            {
                focusedInspectable = null;
                prompt?.Hide();
            }
        }

        private static bool TryGetInspectable(Collider other, out IInspectable inspectable)
        {
            var behaviours = other.GetComponentsInParent<MonoBehaviour>();
            foreach (var behaviour in behaviours)
            {
                if (behaviour is IInspectable candidate)
                {
                    inspectable = candidate;
                    return true;
                }
            }

            inspectable = null;
            return false;
        }
    }
}
