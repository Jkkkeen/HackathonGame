using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FeatherDetective.Tests
{
    public sealed class PrototypeSceneBuilderTests
    {
        [Test]
        public void BuildSceneCreatesExpectedSceneObjectsAndSerializedWiring()
        {
            try
            {
                PrototypeSceneBuilder.BuildInOpenSceneForTests();

                Assert.That(Object.FindObjectsOfType<FeatherPickup>().Length, Is.EqualTo(12));
                Assert.That(Object.FindObjectsOfType<PerchNode>().Length, Is.GreaterThanOrEqualTo(8));
                Assert.That(Object.FindObjectOfType<BirdPlayerController>(), Is.Not.Null);

                var board = Object.FindObjectOfType<DeductionBoard>();
                Assert.That(board, Is.Not.Null);

                var boardInteraction = GameObject.Find("NestBoardInteraction");
                Assert.That(boardInteraction, Is.Not.Null);
                var boardCollider = boardInteraction.GetComponent<BoxCollider>();
                Assert.That(boardCollider, Is.Not.Null);
                Assert.That(boardCollider.isTrigger, Is.True);
                Assert.That(boardCollider.size.x, Is.GreaterThanOrEqualTo(4f));
                Assert.That(boardCollider.size.y, Is.GreaterThanOrEqualTo(1.5f));
                Assert.That(boardCollider.size.z, Is.GreaterThanOrEqualTo(3f));

                var player = Object.FindObjectOfType<BirdPlayerController>();
                var serializedPlayer = new SerializedObject(player);
                Assert.That(serializedPlayer.FindProperty("hopSpeed").floatValue, Is.GreaterThanOrEqualTo(6f));
                var groundedGrace = serializedPlayer.FindProperty("groundedGraceDuration");
                Assert.That(groundedGrace, Is.Not.Null);
                Assert.That(groundedGrace.floatValue, Is.GreaterThanOrEqualTo(0.1f));

                var serializedBoard = new SerializedObject(board);
                var solution = serializedBoard.FindProperty("solution").objectReferenceValue as DeductionSolution;
                Assert.That(solution, Is.Not.Null);
                Assert.That(EditorUtility.IsPersistent(solution), Is.True);
                Assert.That(AssetDatabase.GetAssetPath(solution), Is.EqualTo("Assets/FeatherDetective/Data/DeductionSolution.asset"));
                Assert.That(solution.RequiredSlots, Has.Length.EqualTo(5));
                Assert.That(solution.RequiredSlots[0].SlotId, Is.EqualTo(DeductionSlotId.LastFeedingDay));
                Assert.That(solution.RequiredSlots[0].Feather.Id, Is.EqualTo("crow-bench"));
                Assert.That(solution.RequiredSlots[1].SlotId, Is.EqualTo(DeductionSlotId.WhatChanged));
                Assert.That(solution.RequiredSlots[1].Feather.Id, Is.EqualTo("magpie-bench"));
                Assert.That(solution.RequiredSlots[2].SlotId, Is.EqualTo(DeductionSlotId.WhereHeWent));
                Assert.That(solution.RequiredSlots[2].Feather.Id, Is.EqualTo("magpie-notice"));
                Assert.That(solution.RequiredSlots[3].SlotId, Is.EqualTo(DeductionSlotId.WhyBirdsWaited));
                Assert.That(solution.RequiredSlots[3].Feather.Id, Is.EqualTo("sparrow-trees"));
                Assert.That(solution.RequiredSlots[4].SlotId, Is.EqualTo(DeductionSlotId.WhatEvidenceMeans));
                Assert.That(solution.RequiredSlots[4].Feather.Id, Is.EqualTo("pigeon-edge"));

                Assert.That(Object.FindObjectsOfType<AudioListener>().Length, Is.EqualTo(1));

                foreach (var pickup in Object.FindObjectsOfType<FeatherPickup>())
                {
                    var serializedPickup = new SerializedObject(pickup);
                    var collectedVisual = serializedPickup.FindProperty("collectedVisual").objectReferenceValue as GameObject;
                    Assert.That(collectedVisual, Is.EqualTo(pickup.gameObject), $"{pickup.name} should hide its own visual when collected.");
                }
            }
            finally
            {
                EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            }
        }

        [Test]
        public void BuildSceneUsesShadersCompatibleWithActiveRenderPipeline()
        {
            try
            {
                PrototypeSceneBuilder.BuildInOpenSceneForTests();

                foreach (var sceneRenderer in Object.FindObjectsOfType<Renderer>())
                {
                    foreach (var material in sceneRenderer.sharedMaterials)
                    {
                        if (material == null || material.shader == null)
                        {
                            continue;
                        }

                        Assert.That(
                            material.shader.name,
                            Is.Not.EqualTo("Hidden/InternalErrorShader"),
                            $"{sceneRenderer.name} uses Unity's internal error shader.");

                        Assert.That(
                            material.shader.name,
                            Does.Not.StartWith("Universal Render Pipeline/"),
                            $"{sceneRenderer.name} uses {material.shader.name}, which renders magenta when the project is not running URP.");
                    }
                }
            }
            finally
            {
                EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            }
        }
    }
}
