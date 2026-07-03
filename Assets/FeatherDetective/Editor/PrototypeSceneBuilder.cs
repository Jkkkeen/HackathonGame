using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace FeatherDetective
{
    public static class PrototypeSceneBuilder
    {
        private const string ScenePath = "Assets/FeatherDetective/Scenes/ParkPrototype.unity";
        private const string DataPath = "Assets/FeatherDetective/Data/Feathers";

        [MenuItem("Feather Detective/Build Prototype Scene")]
        public static void BuildPrototypeScene()
        {
            BuildInOpenSceneForTests();
            EnsureAssetFolder(Path.GetDirectoryName(ScenePath)?.Replace("\\", "/") ?? "Assets/FeatherDetective/Scenes");
            EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), ScenePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static void BuildInOpenSceneForTests()
        {
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            EnsureAssetFolder("Assets/FeatherDetective/Data");
            EnsureAssetFolder(DataPath);

            var materials = CreateMaterials();
            var feathers = CreateFeatherDefinitions();
            var solution = CreateSolution(feathers);
            var sun = CreateLighting();
            var colorTargets = CreateParkGeometry(materials);
            var runtimeBundle = CreateRuntime(materials, colorTargets);
            var perches = CreatePerches();

            CreatePlayer(runtimeBundle.Prompt, perches[0], materials);
            CreateFeathers(feathers, runtimeBundle.Runtime, materials);
            CreateNestBoard(runtimeBundle.Runtime, runtimeBundle.EndingUi, solution, sun, materials);
            CreateCamera();
            AssetDatabase.SaveAssets();
        }

        private static Material[] CreateMaterials()
        {
            return new[]
            {
                CreateMaterial("Grass", new Color(0.34f, 0.68f, 0.36f)),
                CreateMaterial("Path", new Color(0.68f, 0.62f, 0.53f)),
                CreateMaterial("Wood", new Color(0.55f, 0.35f, 0.22f)),
                CreateMaterial("Leaf", new Color(0.20f, 0.55f, 0.28f)),
                CreateMaterial("Feather", new Color(0.95f, 0.88f, 0.66f)),
                CreateMaterial("HospitalBlue", new Color(0.36f, 0.62f, 0.95f)),
                CreateMaterial("BraceletRed", new Color(0.84f, 0.28f, 0.28f)),
                CreateMaterial("NoticeYellow", new Color(0.95f, 0.81f, 0.28f))
            };
        }

        private static Material CreateMaterial(string name, Color color)
        {
            var shader = Shader.Find("Universal Render Pipeline/Lit")
                ?? Shader.Find("Standard")
                ?? Shader.Find("Sprites/Default");
            var material = new Material(shader)
            {
                name = name,
                color = color
            };
            return material;
        }

        private static FeatherDefinition[] CreateFeatherDefinitions()
        {
            return new[]
            {
                CreateFeather("crow-bench", "Bench sound feather", BirdSpecies.Crow, ClueRole.MainMystery, DeductionSlotId.LastFeedingDay, "Hospital siren, wheels, and a soft worried voice.", new Vector3[0], Color.yellow),
                CreateFeather("magpie-bench", "Bench color feather", BirdSpecies.Magpie, ClueRole.MainMystery, DeductionSlotId.WhatChanged, "Red bracelet, blue thermos, yellow seed bag.", new Vector3[0], new Color(0.36f, 0.62f, 0.95f)),
                CreateFeather("pigeon-path", "Path route feather", BirdSpecies.Pigeon, ClueRole.MainMystery, DeductionSlotId.WhereHeWent, "Route away from the park.", new[] { new Vector3(-5f, 0.04f, -2f), new Vector3(0f, 0.04f, 1f), new Vector3(7f, 0.04f, 5f) }, Color.white),
                CreateFeather("woodpecker-sign", "Sign rhythm feather", BirdSpecies.Woodpecker, ClueRole.MainMystery, DeductionSlotId.WhatChanged, "Cane taps and hurried packing rhythm.", new Vector3[0], Color.white),
                CreateFeather("sparrow-trees", "Tree atmosphere feather", BirdSpecies.Sparrow, ClueRole.MainMystery, DeductionSlotId.WhyBirdsWaited, "Wind rises while the flock waits.", new Vector3[0], Color.white),
                CreateFeather("crow-fountain", "Fountain sound feather", BirdSpecies.Crow, ClueRole.MainMystery, DeductionSlotId.WhatEvidenceMeans, "Phone ringing near fountain water.", new Vector3[0], Color.white),
                CreateFeather("magpie-notice", "Notice color feather", BirdSpecies.Magpie, ClueRole.MainMystery, DeductionSlotId.WhereHeWent, "Blue-white hospital sign color.", new Vector3[0], new Color(0.36f, 0.62f, 0.95f)),
                CreateFeather("pigeon-edge", "Park edge route feather", BirdSpecies.Pigeon, ClueRole.MainMystery, DeductionSlotId.WhatEvidenceMeans, "Repeated trips between home and hospital direction.", new[] { new Vector3(7f, 0.04f, 5f), new Vector3(4f, 0.04f, -4f), new Vector3(7f, 0.04f, 5f) }, Color.white),
                CreateFeather("sparrow-flowers", "Flower atmosphere feather", BirdSpecies.Sparrow, ClueRole.Atmosphere, DeductionSlotId.WhatChanged, "The park quiets after feeding stops.", new Vector3[0], Color.white),
                CreateFeather("woodpecker-tree", "Tree rhythm feather", BirdSpecies.Woodpecker, ClueRole.Atmosphere, DeductionSlotId.WhatChanged, "Ordinary maintenance knocks.", new Vector3[0], Color.white),
                CreateFeather("magpie-roof", "Roof color feather", BirdSpecies.Magpie, ClueRole.Atmosphere, DeductionSlotId.WhatEvidenceMeans, "Bright picnic colors on a roof edge.", new Vector3[0], new Color(0.84f, 0.28f, 0.28f)),
                CreateFeather("crow-water", "Water sound feather", BirdSpecies.Crow, ClueRole.Atmosphere, DeductionSlotId.LastFeedingDay, "Water, shoes, and children laughing.", new Vector3[0], Color.white)
            };
        }

        private static FeatherDefinition CreateFeather(
            string id,
            string displayName,
            BirdSpecies species,
            ClueRole role,
            DeductionSlotId slot,
            string summary,
            Vector3[] routePoints,
            Color highlightColor)
        {
            var assetPath = $"{DataPath}/{id}.asset";
            var feather = AssetDatabase.LoadAssetAtPath<FeatherDefinition>(assetPath);
            if (feather == null)
            {
                feather = ScriptableObject.CreateInstance<FeatherDefinition>();
                AssetDatabase.CreateAsset(feather, assetPath);
            }

            feather.ConfigureForTests(
                id,
                displayName,
                species,
                role,
                new[] { slot },
                new[] { highlightColor },
                routePoints,
                summary);
            EditorUtility.SetDirty(feather);
            return feather;
        }

        private static DeductionSolution CreateSolution(FeatherDefinition[] feathers)
        {
            var solution = ScriptableObject.CreateInstance<DeductionSolution>();
            solution.ConfigureForTests(new[]
            {
                new DeductionAnswer(DeductionSlotId.LastFeedingDay, feathers[0]),
                new DeductionAnswer(DeductionSlotId.WhatChanged, feathers[1]),
                new DeductionAnswer(DeductionSlotId.WhereHeWent, feathers[6]),
                new DeductionAnswer(DeductionSlotId.WhyBirdsWaited, feathers[4]),
                new DeductionAnswer(DeductionSlotId.WhatEvidenceMeans, feathers[7])
            });
            return solution;
        }

        private static Light CreateLighting()
        {
            RenderSettings.ambientLight = new Color(0.62f, 0.72f, 0.76f);

            var sunObject = new GameObject("Sun");
            sunObject.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

            var sun = sunObject.AddComponent<Light>();
            sun.type = LightType.Directional;
            sun.intensity = 1f;
            sun.shadows = LightShadows.Soft;
            sun.color = new Color(1f, 0.97f, 0.9f);
            return sun;
        }

        private static RuntimeBundle CreateRuntime(Material[] materials, ColorMemoryTarget[] colorTargets)
        {
            var systems = new GameObject("GameSystems");

            var canvas = CreateCanvas();
            var overlay = CreateOverlay(canvas.transform);
            var promptText = CreateText("PromptText", canvas.transform, "Inspect", new Vector2(0f, -220f), new Vector2(0.5f, 0.5f), 24, TextAnchor.MiddleCenter);
            var inventoryText = CreateText("InventoryText", canvas.transform, "Feathers 0/12", new Vector2(24f, -24f), new Vector2(0f, 1f), 20, TextAnchor.UpperLeft);
            var boardText = CreateText("BoardText", canvas.transform, "Nest board", new Vector2(0f, -24f), new Vector2(0.5f, 1f), 20, TextAnchor.UpperCenter);
            var endingGroup = CreateEndingGroup(canvas.transform, out var endingText);

            var prompt = systems.AddComponent<InteractionPrompt>();
            prompt.ConfigureForBuilder(promptText);

            var inventory = systems.AddComponent<FeatherInventoryUI>();
            inventory.ConfigureForBuilder(inventoryText, 12);

            var boardUi = systems.AddComponent<DeductionBoardUI>();
            boardUi.ConfigureForBuilder(boardText);
            boardUi.ShowBoardHint();

            var endingUi = systems.AddComponent<EndingMessageUI>();
            endingUi.ConfigureForBuilder(endingText, endingGroup);

            var memoryAudio = systems.AddComponent<AudioSource>();
            memoryAudio.playOnAwake = false;
            memoryAudio.spatialBlend = 0.75f;

            var routeObject = new GameObject("PigeonRouteLine");
            var routeLine = routeObject.AddComponent<LineRenderer>();
            routeLine.sharedMaterial = materials[4];
            routeLine.useWorldSpace = true;
            routeLine.startWidth = 0.08f;
            routeLine.endWidth = 0.08f;
            var routeRenderer = routeObject.AddComponent<RouteLineRenderer>();

            var rhythmObject = new GameObject("WoodpeckerMemoryRig");
            var rhythmCameraObject = new GameObject("WoodpeckerMemoryCamera");
            rhythmCameraObject.transform.SetParent(rhythmObject.transform, false);
            var rhythmCamera = rhythmCameraObject.AddComponent<Camera>();
            rhythmCamera.enabled = false;
            var rhythmAudio = rhythmObject.AddComponent<AudioSource>();
            rhythmAudio.playOnAwake = false;
            var firstPersonRig = rhythmObject.AddComponent<FirstPersonMemoryRig>();
            firstPersonRig.ConfigureForBuilder(rhythmCamera, rhythmAudio);

            var atmosphereObject = new GameObject("Atmosphere");
            var windSource = atmosphereObject.AddComponent<AudioSource>();
            var insectSource = atmosphereObject.AddComponent<AudioSource>();
            windSource.loop = true;
            insectSource.loop = true;
            windSource.volume = 0.2f;
            insectSource.volume = 0.35f;

            var flockRoot = new GameObject("FlockSilhouettes").transform;
            flockRoot.SetParent(atmosphereObject.transform, false);
            flockRoot.localScale = Vector3.one;
            var atmosphere = atmosphereObject.AddComponent<AtmosphereController>();
            atmosphere.ConfigureForBuilder(windSource, insectSource, flockRoot);

            var context = systems.AddComponent<MemoryContext>();
            context.ConfigureForBuilder(overlay, memoryAudio, colorTargets, routeRenderer, firstPersonRig, atmosphere);

            var playback = systems.AddComponent<MemoryPlaybackController>();
            systems.AddComponent<CrowMemoryEffect>();
            systems.AddComponent<MagpieMemoryEffect>();
            systems.AddComponent<PigeonMemoryEffect>();
            systems.AddComponent<WoodpeckerMemoryEffect>();
            systems.AddComponent<SparrowMemoryEffect>();
            playback.ConfigureForBuilder(context);

            var runtime = systems.AddComponent<InvestigationRuntime>();
            runtime.ConfigureForBuilder(playback, inventory);

            return new RuntimeBundle(runtime, prompt, endingUi);
        }

        private static Canvas CreateCanvas()
        {
            var canvasObject = new GameObject("PrototypeCanvas");
            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasObject.AddComponent<GraphicRaycaster>();
            return canvas;
        }

        private static CanvasGroup CreateOverlay(Transform parent)
        {
            var overlayObject = new GameObject("MemoryDarkOverlay");
            overlayObject.transform.SetParent(parent, false);

            var image = overlayObject.AddComponent<Image>();
            image.color = Color.black;

            var rect = image.rectTransform;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            var group = overlayObject.AddComponent<CanvasGroup>();
            group.alpha = 0f;
            group.blocksRaycasts = false;
            return group;
        }

        private static CanvasGroup CreateEndingGroup(Transform parent, out Text endingText)
        {
            var groupObject = new GameObject("EndingMessage");
            groupObject.transform.SetParent(parent, false);

            var group = groupObject.AddComponent<CanvasGroup>();
            group.alpha = 0f;

            endingText = CreateText("EndingText", groupObject.transform, string.Empty, Vector2.zero, new Vector2(0.5f, 0.5f), 26, TextAnchor.MiddleCenter);
            endingText.color = new Color(0.12f, 0.16f, 0.18f);
            return group;
        }

        private static Text CreateText(
            string name,
            Transform parent,
            string value,
            Vector2 anchoredPosition,
            Vector2 anchor,
            int size,
            TextAnchor alignment)
        {
            var textObject = new GameObject(name);
            textObject.transform.SetParent(parent, false);

            var text = textObject.AddComponent<Text>();
            text.text = value;
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = size;
            text.alignment = alignment;
            text.color = new Color(0.08f, 0.1f, 0.12f);

            var rect = text.rectTransform;
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = anchor;
            rect.sizeDelta = new Vector2(520f, 120f);
            rect.anchoredPosition = anchoredPosition;

            return text;
        }

        private static ColorMemoryTarget[] CreateParkGeometry(Material[] materials)
        {
            var targets = new List<ColorMemoryTarget>();

            AddPrimitive("Grass", PrimitiveType.Cube, new Vector3(0f, -0.05f, 0f), new Vector3(18f, 0.1f, 14f), materials[0]);
            AddPrimitive("CurvedPath", PrimitiveType.Cube, new Vector3(0f, 0f, 0f), new Vector3(15f, 0.08f, 2.2f), materials[1]);
            AddPrimitive("FeedingBench", PrimitiveType.Cube, new Vector3(-4f, 0.45f, -2f), new Vector3(2.6f, 0.35f, 0.7f), materials[2]);
            AddPrimitive("Fountain", PrimitiveType.Cylinder, new Vector3(2.5f, 0.35f, -1.5f), new Vector3(1.4f, 0.35f, 1.4f), materials[5]);
            AddPrimitive("LowRoof", PrimitiveType.Cube, new Vector3(6f, 1.8f, -4.5f), new Vector3(3.5f, 0.25f, 2f), materials[2]);

            var sign = AddPrimitive("HospitalDirectionSign", PrimitiveType.Cube, new Vector3(5.5f, 1.2f, 3.5f), new Vector3(1.4f, 0.8f, 0.08f), materials[5]);
            targets.Add(ConfigureColorTarget(sign, new Color(0.36f, 0.62f, 0.95f)));

            var bag = AddPrimitive("YellowSeedBag", PrimitiveType.Cube, new Vector3(-3.3f, 0.8f, -1.5f), new Vector3(0.45f, 0.45f, 0.25f), materials[7]);
            targets.Add(ConfigureColorTarget(bag, new Color(0.95f, 0.81f, 0.28f)));

            var bracelet = AddPrimitive("RedBracelet", PrimitiveType.Cylinder, new Vector3(-4.5f, 0.72f, -1.35f), new Vector3(0.12f, 0.02f, 0.12f), materials[6]);
            targets.Add(ConfigureColorTarget(bracelet, new Color(0.84f, 0.28f, 0.28f)));

            var thermos = AddPrimitive("BlueThermos", PrimitiveType.Cylinder, new Vector3(-3.8f, 0.85f, -2.15f), new Vector3(0.18f, 0.32f, 0.18f), materials[5]);
            targets.Add(ConfigureColorTarget(thermos, new Color(0.36f, 0.62f, 0.95f)));

            for (var i = 0; i < 5; i++)
            {
                var x = -7f + i * 3.5f;
                AddPrimitive($"TreeTrunk_{i}", PrimitiveType.Cylinder, new Vector3(x, 0.65f, 4.5f), new Vector3(0.35f, 1.3f, 0.35f), materials[2]);
                AddPrimitive($"TreeCrown_{i}", PrimitiveType.Sphere, new Vector3(x, 1.8f, 4.5f), new Vector3(1.4f, 1.1f, 1.4f), materials[3]);
            }

            return targets.ToArray();
        }

        private static ColorMemoryTarget ConfigureColorTarget(GameObject targetObject, Color memoryColor)
        {
            var target = targetObject.AddComponent<ColorMemoryTarget>();
            target.ConfigureForBuilder(memoryColor);
            return target;
        }

        private static PerchNode[] CreatePerches()
        {
            var perches = new[]
            {
                CreatePerch("Perch_Bench", new Vector3(-4f, 0.9f, -2f)),
                CreatePerch("Perch_TreeA", new Vector3(-7f, 2.35f, 4.5f)),
                CreatePerch("Perch_TreeB", new Vector3(-3.5f, 2.35f, 4.5f)),
                CreatePerch("Perch_Sign", new Vector3(5.5f, 1.75f, 3.5f)),
                CreatePerch("Perch_Roof", new Vector3(6f, 2.05f, -4.5f)),
                CreatePerch("Perch_Fountain", new Vector3(2.5f, 0.9f, -1.5f)),
                CreatePerch("Perch_Nest", new Vector3(0f, 1.2f, -5.5f)),
                CreatePerch("Perch_PathPost", new Vector3(0f, 1.1f, 2.5f))
            };

            perches[0].ConfigureForTests(new[] { perches[1], perches[5], perches[7] });
            perches[1].ConfigureForTests(new[] { perches[0], perches[2], perches[6] });
            perches[2].ConfigureForTests(new[] { perches[1], perches[3], perches[7] });
            perches[3].ConfigureForTests(new[] { perches[2], perches[4] });
            perches[4].ConfigureForTests(new[] { perches[3], perches[5] });
            perches[5].ConfigureForTests(new[] { perches[0], perches[4], perches[6] });
            perches[6].ConfigureForTests(new[] { perches[1], perches[5], perches[7] });
            perches[7].ConfigureForTests(new[] { perches[0], perches[2], perches[6] });
            return perches;
        }

        private static PerchNode CreatePerch(string name, Vector3 position)
        {
            var perchObject = new GameObject(name);
            perchObject.transform.position = position;

            var collider = perchObject.AddComponent<SphereCollider>();
            collider.radius = 0.45f;
            collider.isTrigger = true;

            return perchObject.AddComponent<PerchNode>();
        }

        private static void CreatePlayer(InteractionPrompt prompt, PerchNode startPerch, Material[] materials)
        {
            var player = new GameObject("PlayerBird");
            var characterController = player.AddComponent<CharacterController>();
            characterController.height = 0.7f;
            characterController.radius = 0.22f;
            characterController.center = new Vector3(0f, 0.35f, 0f);

            var body = AddPrimitive("BirdBody", PrimitiveType.Sphere, Vector3.zero, new Vector3(0.45f, 0.32f, 0.55f), materials[4]);
            body.transform.SetParent(player.transform, false);
            body.transform.localPosition = new Vector3(0f, 0.35f, 0f);

            var controller = player.AddComponent<BirdPlayerController>();
            controller.ConfigureForBuilder(prompt, startPerch);
        }

        private static void CreateFeathers(FeatherDefinition[] feathers, InvestigationRuntime runtime, Material[] materials)
        {
            var positions = new[]
            {
                new Vector3(-4.8f, 0.2f, -1.6f),
                new Vector3(-3.5f, 0.2f, -1.4f),
                new Vector3(-1f, 0.2f, 0.9f),
                new Vector3(4.6f, 0.2f, 3.2f),
                new Vector3(-6.5f, 0.2f, 4.2f),
                new Vector3(2.5f, 0.2f, -2.5f),
                new Vector3(5.8f, 0.2f, 3.9f),
                new Vector3(7f, 0.2f, 5f),
                new Vector3(-2f, 0.2f, 5.5f),
                new Vector3(-7f, 1.3f, 4.2f),
                new Vector3(6f, 2.2f, -4.3f),
                new Vector3(1.8f, 0.2f, -1.1f)
            };

            for (var i = 0; i < feathers.Length; i++)
            {
                var featherObject = AddPrimitive(
                    $"Feather_{feathers[i].Id}",
                    PrimitiveType.Capsule,
                    positions[i],
                    new Vector3(0.12f, 0.28f, 0.12f),
                    materials[4]);
                featherObject.transform.rotation = Quaternion.Euler(65f, i * 23f, 20f);

                var collider = featherObject.GetComponent<Collider>();
                collider.isTrigger = true;

                var pickup = featherObject.AddComponent<FeatherPickup>();
                pickup.ConfigureForBuilder(feathers[i], runtime);
            }
        }

        private static void CreateNestBoard(
            InvestigationRuntime runtime,
            EndingMessageUI endingUi,
            DeductionSolution solution,
            Light sun,
            Material[] materials)
        {
            var boardObject = AddPrimitive("NestBoard", PrimitiveType.Cube, new Vector3(0f, 0.75f, -5.5f), new Vector3(2.8f, 0.2f, 1.6f), materials[2]);
            boardObject.GetComponent<Collider>().isTrigger = true;

            var slotIds = new[]
            {
                DeductionSlotId.LastFeedingDay,
                DeductionSlotId.WhatChanged,
                DeductionSlotId.WhereHeWent,
                DeductionSlotId.WhyBirdsWaited,
                DeductionSlotId.WhatEvidenceMeans
            };
            var slots = new DeductionSlot[slotIds.Length];

            for (var i = 0; i < slotIds.Length; i++)
            {
                var slotObject = AddPrimitive(
                    $"DeductionSlot_{slotIds[i]}",
                    PrimitiveType.Cube,
                    new Vector3(-1.1f + i * 0.55f, 0.95f, -5.5f),
                    new Vector3(0.35f, 0.08f, 0.5f),
                    materials[4]);
                slotObject.transform.SetParent(boardObject.transform, true);

                var slot = slotObject.AddComponent<DeductionSlot>();
                slot.ConfigureForBuilder(slotIds[i]);
                slots[i] = slot;
            }

            var endingObject = new GameObject("EndingController");
            var ending = endingObject.AddComponent<EndingController>();
            ending.ConfigureForBuilder(endingUi, sun);

            var board = boardObject.AddComponent<DeductionBoard>();
            board.ConfigureForBuilder(runtime, solution, ending, slots);
        }

        private static void CreateCamera()
        {
            var cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";

            var camera = cameraObject.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.68f, 0.82f, 0.94f);

            var follow = cameraObject.AddComponent<FixedThirdPersonCamera>();
            var player = GameObject.Find("PlayerBird");
            if (player != null)
            {
                follow.SetTarget(player.transform);
            }

            cameraObject.transform.position = new Vector3(0f, 8f, -9f);
            cameraObject.transform.rotation = Quaternion.Euler(48f, 0f, 0f);
        }

        private static GameObject AddPrimitive(string name, PrimitiveType type, Vector3 position, Vector3 scale, Material material)
        {
            var gameObject = GameObject.CreatePrimitive(type);
            gameObject.name = name;
            gameObject.transform.position = position;
            gameObject.transform.localScale = scale;

            var renderer = gameObject.GetComponent<Renderer>();
            if (renderer != null && material != null)
            {
                renderer.sharedMaterial = material;
            }

            return gameObject;
        }

        private static void EnsureAssetFolder(string folderPath)
        {
            if (string.IsNullOrEmpty(folderPath) || AssetDatabase.IsValidFolder(folderPath))
            {
                return;
            }

            var normalized = folderPath.Replace("\\", "/");
            var parent = Path.GetDirectoryName(normalized)?.Replace("\\", "/");
            if (!string.IsNullOrEmpty(parent) && !AssetDatabase.IsValidFolder(parent))
            {
                EnsureAssetFolder(parent);
            }

            AssetDatabase.CreateFolder(parent ?? "Assets", Path.GetFileName(normalized));
        }

        private struct RuntimeBundle
        {
            public RuntimeBundle(InvestigationRuntime runtime, InteractionPrompt prompt, EndingMessageUI endingUi)
            {
                Runtime = runtime;
                Prompt = prompt;
                EndingUi = endingUi;
            }

            public InvestigationRuntime Runtime { get; }
            public InteractionPrompt Prompt { get; }
            public EndingMessageUI EndingUi { get; }
        }
    }
}
