using NUnit.Framework;
using UnityEngine;

namespace FeatherDetective.Tests
{
    public sealed class PerchSelectorTests
    {
        [Test]
        public void SelectBestReachablePerchPrefersForwardLinkedPerch()
        {
            var originObject = new GameObject("origin");
            var forwardObject = new GameObject("forward");
            var sideObject = new GameObject("side");

            var origin = originObject.AddComponent<PerchNode>();
            var forward = forwardObject.AddComponent<PerchNode>();
            var side = sideObject.AddComponent<PerchNode>();

            originObject.transform.position = Vector3.zero;
            forwardObject.transform.position = new Vector3(0f, 0f, 4f);
            sideObject.transform.position = new Vector3(4f, 0f, 0f);
            origin.ConfigureForTests(new[] { forward, side });

            var selected = PerchSelector.SelectBestReachable(origin, Vector3.zero, Vector3.forward, 6f, 0.25f);

            Assert.That(selected, Is.EqualTo(forward));

            Object.DestroyImmediate(originObject);
            Object.DestroyImmediate(forwardObject);
            Object.DestroyImmediate(sideObject);
        }

        [Test]
        public void SelectBestReachablePerchReturnsNullWhenNoLinkedPerchMatchesDistance()
        {
            var originObject = new GameObject("origin");
            var farObject = new GameObject("far");

            var origin = originObject.AddComponent<PerchNode>();
            var far = farObject.AddComponent<PerchNode>();

            farObject.transform.position = new Vector3(0f, 0f, 20f);
            origin.ConfigureForTests(new[] { far });

            var selected = PerchSelector.SelectBestReachable(origin, Vector3.zero, Vector3.forward, 6f, 0.25f);

            Assert.That(selected, Is.Null);

            Object.DestroyImmediate(originObject);
            Object.DestroyImmediate(farObject);
        }
    }
}
