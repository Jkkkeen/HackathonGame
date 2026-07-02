using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

namespace FeatherDetective.Tests
{
    public sealed class PerchSelectorTests
    {
        private readonly List<GameObject> createdObjects = new List<GameObject>();

        [TearDown]
        public void TearDown()
        {
            for (var i = createdObjects.Count - 1; i >= 0; i--)
            {
                if (createdObjects[i] != null)
                {
                    Object.DestroyImmediate(createdObjects[i]);
                }
            }

            createdObjects.Clear();
        }

        [Test]
        public void SelectBestReachablePerchPrefersForwardLinkedPerch()
        {
            var originObject = CreateGameObject("origin");
            var forwardObject = CreateGameObject("forward");
            var sideObject = CreateGameObject("side");

            var origin = originObject.AddComponent<PerchNode>();
            var forward = forwardObject.AddComponent<PerchNode>();
            var side = sideObject.AddComponent<PerchNode>();

            originObject.transform.position = Vector3.zero;
            forwardObject.transform.position = new Vector3(0f, 0f, 4f);
            sideObject.transform.position = new Vector3(4f, 0f, 0f);
            origin.ConfigureForTests(new[] { forward, side });

            var selected = PerchSelector.SelectBestReachable(origin, Vector3.zero, Vector3.forward, 6f, 0.25f);

            Assert.That(selected, Is.EqualTo(forward));
        }

        [Test]
        public void SelectBestReachablePerchReturnsNullWhenNoLinkedPerchMatchesDistance()
        {
            var originObject = CreateGameObject("origin");
            var farObject = CreateGameObject("far");

            var origin = originObject.AddComponent<PerchNode>();
            var far = farObject.AddComponent<PerchNode>();

            farObject.transform.position = new Vector3(0f, 0f, 20f);
            origin.ConfigureForTests(new[] { far });

            var selected = PerchSelector.SelectBestReachable(origin, Vector3.zero, Vector3.forward, 6f, 0.25f);

            Assert.That(selected, Is.Null);
        }

        [Test]
        public void TryGetInspectableFindsInspectableOnParentOfCollider()
        {
            var parentObject = CreateGameObject("inspectable");
            var childObject = CreateGameObject("child collider");
            childObject.transform.SetParent(parentObject.transform);
            var expected = parentObject.AddComponent<InspectableStub>();
            var collider = childObject.AddComponent<BoxCollider>();

            var method = typeof(BirdPlayerController).GetMethod("TryGetInspectable", BindingFlags.NonPublic | BindingFlags.Static);
            Assert.That(method, Is.Not.Null);

            var parameters = new object[] { collider, null };
            var found = (bool)method.Invoke(null, parameters);

            Assert.That(found, Is.True);
            Assert.That(parameters[1], Is.SameAs(expected));
        }

        private GameObject CreateGameObject(string name)
        {
            var gameObject = new GameObject(name);
            createdObjects.Add(gameObject);
            return gameObject;
        }

        private sealed class InspectableStub : MonoBehaviour, IInspectable
        {
            public string PromptText => "Inspect";

            public void Inspect()
            {
            }
        }
    }
}
