using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FeatherDetective.Tests
{
    public sealed class PrototypeSceneBuilderTests
    {
        [Test]
        public void BuildSceneCreatesTwelveFeathersAndAtLeastEightPerches()
        {
            PrototypeSceneBuilder.BuildInOpenSceneForTests();

            Assert.That(Object.FindObjectsOfType<FeatherPickup>().Length, Is.EqualTo(12));
            Assert.That(Object.FindObjectsOfType<PerchNode>().Length, Is.GreaterThanOrEqualTo(8));
            Assert.That(Object.FindObjectOfType<BirdPlayerController>(), Is.Not.Null);
            Assert.That(Object.FindObjectOfType<DeductionBoard>(), Is.Not.Null);

            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        }
    }
}
