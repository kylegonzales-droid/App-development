using Manor.Game.CameraRig;
using Manor.Game.Interaction;
using Manor.Game.Missions;
using Manor.Game.NPC;
using Manor.Game.Player;
using Manor.Game.Systems;
using Manor.Game.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Manor.Editor
{
    /// <summary>
    /// Builds the playable Kingston-inspired vertical slice from primitives.
    ///
    /// Everything is generated in code rather than hand-placed, so the scene can be
    /// rebuilt from scratch at any time and there is no binary scene file to merge or
    /// corrupt. The geography follows docs/design/01-world-and-map.md: river west,
    /// pedestrianised core, high street, side streets, park to the south-east.
    ///
    /// This is a BLOCKOUT. It is the grey-box the art pass replaces — and per
    /// docs/graphics/09-kingston-environment-pipeline.md it must play well before
    /// anyone makes it pretty.
    /// </summary>
    public static class KingstonSceneBuilder
    {
        private const string ScenePath = ManorPaths.Scenes + "/KingstonSlice.unity";

        // Layout constants, metres.
        private const float RoadWidth = 9f;
        private const float PavementWidth = 3f;
        private const float KerbHeight = 0.14f;
        private const float HighStreetLength = 150f;
        private const float RiverX = -62f;

        [MenuItem("Manor/Build Kingston Slice Scene", priority = 0)]
        public static void Build()
        {
            ManorPaths.EnsureFolders();

            Scene scene = EditorSceneManager.NewScene(
                NewSceneSetup.EmptyScene, NewSceneMode.Single);

            var root = new GameObject("Kingston").transform;

            BuildGround(root);
            BuildRiver(root);
            BuildHighStreet(root);
            BuildSideStreets(root);
            BuildBuildings(root);
            BuildPark(root);
            BuildStreetFurniture(root);

            GameObject player = BuildPlayer(root);
            BuildCamera(root, player.transform);
            BuildLighting(root);
            GameObject weatherRoot = BuildWeather(root);
            BuildSystems(root, player, weatherRoot);
            BuildMissionZones(root);
            BuildNPCs(root);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.SaveAssets();

            Debug.Log($"[Manor] Kingston slice built and saved to {ScenePath}. Press Play.");
        }

        // ------------------------------------------------------------- terrain

        private static void BuildGround(Transform root)
        {
            Material grass = ManorPaths.Material("Ground", new Color(0.24f, 0.27f, 0.21f), 0.08f);
            GameObject ground = Box(root, "Ground", new Vector3(0f, -0.5f, 30f),
                new Vector3(260f, 1f, 300f), grass);
            ground.isStatic = true;
        }

        private static void BuildRiver(Transform root)
        {
            Material water = ManorPaths.Material("River", new Color(0.10f, 0.17f, 0.20f), 0.94f, 0.1f);
            GameObject river = Box(root, "River_Thames", new Vector3(RiverX, -0.35f, 30f),
                new Vector3(34f, 0.6f, 300f), water);
            river.isStatic = true;

            // Embankment wall along the east bank.
            Material stone = ManorPaths.Material("Stone", new Color(0.33f, 0.32f, 0.30f), 0.15f);
            Box(root, "Embankment", new Vector3(RiverX + 17.5f, 0.2f, 30f),
                new Vector3(1.4f, 1.4f, 300f), stone).isStatic = true;

            // Kings Bridge, crossing west.
            Material concrete = ManorPaths.Material("Concrete", new Color(0.38f, 0.38f, 0.36f), 0.18f);
            Box(root, "KingsBridge_Deck", new Vector3(RiverX, 0.9f, 0f),
                new Vector3(40f, 0.8f, 11f), concrete).isStatic = true;
            for (int i = -1; i <= 1; i++)
            {
                Box(root, $"KingsBridge_Pier_{i}", new Vector3(RiverX + i * 11f, 0f, 0f),
                    new Vector3(2.2f, 2f, 11f), stone).isStatic = true;
            }
        }

        // ------------------------------------------------------------- streets

        private static void BuildHighStreet(Transform root)
        {
            Material tarmac = ManorPaths.Material("Tarmac", new Color(0.13f, 0.13f, 0.14f), 0.16f);
            Material paving = ManorPaths.Material("Paving", new Color(0.31f, 0.31f, 0.30f), 0.12f);
            Material line = ManorPaths.Material("RoadLine", new Color(0.78f, 0.76f, 0.70f), 0.25f);
            Material yellow = ManorPaths.Material("DoubleYellow", new Color(0.72f, 0.58f, 0.16f), 0.2f);

            var street = new GameObject("HighStreet").transform;
            street.SetParent(root);

            GameObject road = Box(street, "HighStreet_Road", new Vector3(0f, 0.02f, 0f),
                new Vector3(RoadWidth, 0.1f, HighStreetLength), tarmac);
            Wet(road, 0.75f);

            foreach (int side in new[] { -1, 1 })
            {
                float x = side * (RoadWidth * 0.5f + PavementWidth * 0.5f);
                GameObject pavement = Box(street, $"Pavement_{(side < 0 ? "W" : "E")}",
                    new Vector3(x, KerbHeight * 0.5f, 0f),
                    new Vector3(PavementWidth, KerbHeight, HighStreetLength), paving);
                Wet(pavement, 0.65f);

                // Double yellow lines at the kerb — small detail, very British.
                Box(street, $"DoubleYellow_{(side < 0 ? "W" : "E")}",
                    new Vector3(side * (RoadWidth * 0.5f - 0.35f), 0.075f, 0f),
                    new Vector3(0.3f, 0.02f, HighStreetLength), yellow);
            }

            // Centre dashes.
            for (float z = -HighStreetLength * 0.5f + 4f; z < HighStreetLength * 0.5f; z += 8f)
            {
                Box(street, "CentreLine", new Vector3(0f, 0.075f, z),
                    new Vector3(0.16f, 0.02f, 3f), line);
            }

            // Zebra crossing.
            for (int i = -4; i <= 4; i++)
            {
                Box(street, "Zebra", new Vector3(i * 0.9f, 0.075f, 24f),
                    new Vector3(0.45f, 0.02f, RoadWidth), line);
            }
        }

        private static void BuildSideStreets(Transform root)
        {
            Material tarmac = ManorPaths.Material("Tarmac", new Color(0.13f, 0.13f, 0.14f), 0.16f);
            Material paving = ManorPaths.Material("Paving", new Color(0.31f, 0.31f, 0.30f), 0.12f);

            var side = new GameObject("SideStreets").transform;
            side.SetParent(root);

            float[] zs = { -48f, -14f, 22f, 56f };
            foreach (float z in zs)
            {
                GameObject road = Box(side, $"SideStreet_{z:0}", new Vector3(-26f, 0.02f, z),
                    new Vector3(46f, 0.1f, 7f), tarmac);
                Wet(road, 0.75f);

                foreach (int s in new[] { -1, 1 })
                {
                    Box(side, $"SidePavement_{z:0}_{s}",
                        new Vector3(-26f, KerbHeight * 0.5f, z + s * 5f),
                        new Vector3(46f, KerbHeight, 2.4f), paving);
                }
            }
        }

        // ------------------------------------------------------------- buildings

        private static void BuildBuildings(Transform root)
        {
            var parent = new GameObject("Buildings").transform;
            parent.SetParent(root);

            Color[] brickTones =
            {
                new(0.35f, 0.24f, 0.20f), new(0.40f, 0.27f, 0.22f),
                new(0.30f, 0.22f, 0.19f), new(0.44f, 0.33f, 0.27f),
                new(0.26f, 0.25f, 0.24f)
            };

            Material glass = ManorPaths.Material("ShopGlass", new Color(0.12f, 0.15f, 0.17f), 0.92f, 0.2f);
            Material fascia = ManorPaths.Material("Fascia", new Color(0.18f, 0.20f, 0.22f), 0.25f);

            var random = new System.Random(20260916);

            foreach (int sideSign in new[] { -1, 1 })
            {
                float x = sideSign * (RoadWidth * 0.5f + PavementWidth + 7f);
                float z = -HighStreetLength * 0.5f + 6f;

                int index = 0;
                while (z < HighStreetLength * 0.5f - 6f)
                {
                    float width = 7f + (float)random.NextDouble() * 6f;
                    float height = 8f + (float)random.NextDouble() * 9f;
                    Color tone = brickTones[random.Next(brickTones.Length)];

                    Material brick = ManorPaths.Material(
                        $"Brick_{ColorUtility.ToHtmlStringRGB(tone)}", tone, 0.08f);

                    GameObject block = Box(parent, $"Building_{(sideSign < 0 ? "W" : "E")}_{index:00}",
                        new Vector3(x, height * 0.5f, z + width * 0.5f),
                        new Vector3(14f, height, width), brick);
                    block.isStatic = true;
                    Wet(block, 0.85f);

                    // Shopfront glazing facing the street.
                    float frontX = x - sideSign * 7.05f;
                    Box(parent, "Shopfront", new Vector3(frontX, 1.9f, z + width * 0.5f),
                        new Vector3(0.12f, 2.6f, width * 0.8f), glass).isStatic = true;
                    Box(parent, "Fascia", new Vector3(frontX, 3.6f, z + width * 0.5f),
                        new Vector3(0.18f, 0.8f, width * 0.85f), fascia).isStatic = true;

                    z += width + 0.6f;
                    index++;
                }
            }
        }

        private static void BuildPark(Transform root)
        {
            var parent = new GameObject("Park").transform;
            parent.SetParent(root);

            Material grass = ManorPaths.Material("ParkGrass", new Color(0.19f, 0.28f, 0.16f), 0.07f);
            Material trunk = ManorPaths.Material("Trunk", new Color(0.19f, 0.15f, 0.12f), 0.1f);
            Material canopy = ManorPaths.Material("Canopy", new Color(0.16f, 0.26f, 0.14f), 0.06f);

            Box(parent, "ParkGround", new Vector3(48f, 0.03f, 46f),
                new Vector3(56f, 0.1f, 52f), grass).isStatic = true;

            var random = new System.Random(77);
            for (int i = 0; i < 18; i++)
            {
                float x = 48f + (float)(random.NextDouble() - 0.5) * 48f;
                float z = 46f + (float)(random.NextDouble() - 0.5) * 44f;
                float scale = 0.8f + (float)random.NextDouble() * 0.7f;

                Cylinder(parent, "Trunk", new Vector3(x, 1.6f * scale, z),
                    new Vector3(0.35f, 1.6f * scale, 0.35f), trunk).isStatic = true;
                Sphere(parent, "Canopy", new Vector3(x, 4.0f * scale, z),
                    Vector3.one * (3.4f * scale), canopy).isStatic = true;
            }
        }

        private static void BuildStreetFurniture(Transform root)
        {
            var parent = new GameObject("StreetFurniture").transform;
            parent.SetParent(root);

            Material dark = ManorPaths.Material("PaintedMetal", new Color(0.13f, 0.14f, 0.15f), 0.35f, 0.4f);
            Material binGreen = ManorPaths.Material("BinGreen", new Color(0.14f, 0.22f, 0.17f), 0.3f);
            Material red = ManorPaths.Material("SignRed", new Color(0.55f, 0.11f, 0.11f), 0.3f);

            for (float z = -HighStreetLength * 0.5f + 10f; z < HighStreetLength * 0.5f; z += 18f)
            {
                foreach (int side in new[] { -1, 1 })
                {
                    float x = side * (RoadWidth * 0.5f + PavementWidth - 0.7f);

                    // Lamp post with a real point light — sodium one side, LED the other.
                    Cylinder(parent, "LampPost", new Vector3(x, 2.6f, z),
                        new Vector3(0.16f, 2.6f, 0.16f), dark).isStatic = true;

                    var lampGO = new GameObject(side < 0 ? "Lamp_Sodium" : "Lamp_LED");
                    lampGO.transform.SetParent(parent);
                    lampGO.transform.position = new Vector3(x - side * 0.8f, 5.1f, z);
                    Light lamp = lampGO.AddComponent<Light>();
                    lamp.type = LightType.Point;
                    lamp.range = 13f;
                    lamp.intensity = 2.4f;
                    lamp.color = side < 0
                        ? new Color(0.95f, 0.63f, 0.24f)   // sodium
                        : new Color(0.75f, 0.90f, 0.94f);  // LED
                    lamp.shadows = LightShadows.None;
                }
            }

            // Bins and bollards.
            for (float z = -60f; z < 70f; z += 26f)
            {
                Box(parent, "WheelieBin", new Vector3(RoadWidth * 0.5f + 1.6f, 0.6f, z),
                    new Vector3(0.7f, 1.1f, 0.8f), binGreen).isStatic = true;
            }
            for (float z = -40f; z < 40f; z += 3f)
            {
                Cylinder(parent, "Bollard", new Vector3(-(RoadWidth * 0.5f + 0.4f), 0.45f, z),
                    new Vector3(0.14f, 0.45f, 0.14f), dark).isStatic = true;
            }

            // Bus stop.
            Box(parent, "BusStop_Shelter", new Vector3(RoadWidth * 0.5f + 2.2f, 1.3f, 34f),
                new Vector3(1.4f, 2.6f, 4.5f), dark).isStatic = true;
            Box(parent, "BusStop_Flag", new Vector3(RoadWidth * 0.5f + 0.9f, 2.6f, 31f),
                new Vector3(0.6f, 0.4f, 0.08f), red).isStatic = true;
        }

        // ------------------------------------------------------------- actors

        private static GameObject BuildPlayer(Transform root)
        {
            var player = new GameObject("Player");
            player.transform.SetParent(root);
            player.transform.position = new Vector3(0f, 1.2f, -60f);
            player.tag = "Player";

            var controller = player.AddComponent<CharacterController>();
            controller.height = 1.8f;
            controller.radius = 0.32f;
            controller.center = new Vector3(0f, 0.9f, 0f);
            controller.slopeLimit = 50f;
            controller.stepOffset = 0.35f;

            player.AddComponent<PlayerController>();
            player.AddComponent<Interactor>();

            // Placeholder body. Replaced by a real rigged character — see
            // docs/graphics/13-character-sourcing.md.
            Material body = ManorPaths.Material("PlayerBody", new Color(0.16f, 0.18f, 0.22f), 0.2f);
            GameObject capsule = Capsule(player.transform, "Body_Placeholder",
                new Vector3(0f, 0.9f, 0f), new Vector3(0.62f, 0.9f, 0.62f), body);
            Object.DestroyImmediate(capsule.GetComponent<Collider>());

            Material bag = ManorPaths.Material("CourierBag", new Color(0.55f, 0.30f, 0.10f), 0.2f);
            GameObject satchel = Box(player.transform, "CourierBag",
                new Vector3(0f, 1.15f, -0.34f), new Vector3(0.5f, 0.52f, 0.22f), bag);
            Object.DestroyImmediate(satchel.GetComponent<Collider>());

            return player;
        }

        private static void BuildCamera(Transform root, Transform target)
        {
            var cameraGO = new GameObject("MainCamera");
            cameraGO.transform.SetParent(root);
            cameraGO.tag = "MainCamera";

            UnityEngine.Camera cam = cameraGO.AddComponent<UnityEngine.Camera>();
            cam.fieldOfView = 62f;
            cam.nearClipPlane = 0.1f;
            cam.farClipPlane = 420f;
            cameraGO.AddComponent<AudioListener>();

            ThirdPersonCamera rig = cameraGO.AddComponent<ThirdPersonCamera>();
            rig.SetTarget(target);
        }

        private static void BuildLighting(Transform root)
        {
            var sunGO = new GameObject("Sun");
            sunGO.transform.SetParent(root);
            Light sun = sunGO.AddComponent<Light>();
            sun.type = LightType.Directional;
            sun.shadows = LightShadows.Soft;
            sun.intensity = 1.1f;
            sunGO.AddComponent<TimeOfDayDriver>();

            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.36f, 0.38f, 0.42f);
            RenderSettings.fog = true;
        }

        private static GameObject BuildWeather(Transform root)
        {
            var weatherGO = new GameObject("Weather");
            weatherGO.transform.SetParent(root);
            weatherGO.transform.position = new Vector3(0f, 14f, 0f);

            ParticleSystem rain = Precipitation(weatherGO.transform, "Rain",
                new Color(0.72f, 0.80f, 0.86f, 0.42f), speed: 24f, size: 0.055f, stretch: true);
            ParticleSystem snow = Precipitation(weatherGO.transform, "Snow",
                new Color(0.96f, 0.97f, 1f, 0.85f), speed: 2.2f, size: 0.09f, stretch: false);

            WeatherDriver driver = weatherGO.AddComponent<WeatherDriver>();
            SerializedObject so = new(driver);
            so.FindProperty("rain").objectReferenceValue = rain;
            so.FindProperty("snow").objectReferenceValue = snow;
            so.ApplyModifiedPropertiesWithoutUndo();

            return weatherGO;
        }

        private static ParticleSystem Precipitation(
            Transform parent, string name, Color colour, float speed, float size, bool stretch)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent);
            go.transform.localPosition = Vector3.zero;

            ParticleSystem ps = go.AddComponent<ParticleSystem>();
            ps.Stop();

            ParticleSystem.MainModule main = ps.main;
            main.startLifetime = 1.6f;
            main.startSpeed = speed;
            main.startSize = size;
            main.startColor = colour;
            main.maxParticles = 4000;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.gravityModifier = stretch ? 0.4f : 0.05f;

            ParticleSystem.ShapeModule shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Box;
            shape.scale = new Vector3(44f, 0.1f, 44f);
            shape.rotation = new Vector3(90f, 0f, 0f);

            ParticleSystem.EmissionModule emission = ps.emission;
            emission.rateOverTime = 0f;

            var renderer = go.GetComponent<ParticleSystemRenderer>();
            renderer.renderMode = stretch
                ? ParticleSystemRenderMode.Stretch
                : ParticleSystemRenderMode.Billboard;
            if (stretch) renderer.lengthScale = 5.5f;
            renderer.sharedMaterial = ManorPaths.Material($"Particle_{name}", colour, 0.1f);
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.receiveShadows = false;

            return ps;
        }

        private static void BuildSystems(Transform root, GameObject player, GameObject weatherRoot)
        {
            var systems = new GameObject("Systems");
            systems.transform.SetParent(root);

            systems.AddComponent<GameDirector>();

            QualityController quality = systems.AddComponent<QualityController>();
            SerializedObject qso = new(quality);
            qso.FindProperty("weather").objectReferenceValue = weatherRoot.GetComponent<WeatherDriver>();
            qso.ApplyModifiedPropertiesWithoutUndo();

            HUDController hud = systems.AddComponent<HUDController>();
            SerializedObject hso = new(hud);
            hso.FindProperty("interactor").objectReferenceValue = player.GetComponent<Interactor>();
            hso.FindProperty("player").objectReferenceValue = player.GetComponent<PlayerController>();
            hso.FindProperty("cameraRig").objectReferenceValue =
                Object.FindFirstObjectByType<ThirdPersonCamera>();
            hso.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void BuildMissionZones(Transform root)
        {
            var parent = new GameObject("MissionZones").transform;
            parent.SetParent(root);

            AddZone(parent, "town_centre", new Vector3(0f, 1f, 0f), 14f);
            AddZone(parent, "riverside", new Vector3(RiverX + 20f, 1f, 20f), 12f);
            AddZone(parent, "high_street", new Vector3(0f, 1f, 52f), 14f);
            AddZone(parent, "meeting_point", new Vector3(0f, 1f, -60f), 9f);
        }

        private static void AddZone(Transform parent, string id, Vector3 position, float radius)
        {
            var go = new GameObject($"Zone_{id}");
            go.transform.SetParent(parent);
            go.transform.position = position;
            go.AddComponent<SphereCollider>();
            go.AddComponent<MissionZone>().Configure(id, radius);
        }

        private static void BuildNPCs(Transform root)
        {
            GameObject prefab = NPCPrefabBuilder.EnsurePrefab();

            NPCDefinition pedestrian = NPCPrefabBuilder.EnsureDefinition(
                "NPC_Pedestrian", "Passer-by", NPCRole.Pedestrian, "");
            NPCDefinition local = NPCPrefabBuilder.EnsureDefinition(
                "NPC_LocalResident", "Local", NPCRole.Resident, "local_resident");

            var parent = new GameObject("NPCs").transform;
            parent.SetParent(root);

            AddSpawner(parent, "Spawner_HighStreet", new Vector3(0f, 0.2f, 40f), 22f, 7,
                prefab, new[] { pedestrian, local });
            AddSpawner(parent, "Spawner_TownCentre", new Vector3(0f, 0.2f, -8f), 20f, 6,
                prefab, new[] { pedestrian, local });
            AddSpawner(parent, "Spawner_Riverside", new Vector3(RiverX + 22f, 0.2f, 18f), 16f, 4,
                prefab, new[] { pedestrian });
        }

        private static void AddSpawner(
            Transform parent, string name, Vector3 position, float radius, int count,
            GameObject prefab, NPCDefinition[] defs)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent);
            go.transform.position = position;

            NPCSpawner spawner = go.AddComponent<NPCSpawner>();
            SerializedObject so = new(spawner);
            so.FindProperty("npcPrefab").objectReferenceValue = prefab;
            so.FindProperty("count").intValue = count;
            so.FindProperty("spawnRadius").floatValue = radius;

            SerializedProperty array = so.FindProperty("definitions");
            array.arraySize = defs.Length;
            for (int i = 0; i < defs.Length; i++)
            {
                array.GetArrayElementAtIndex(i).objectReferenceValue = defs[i];
            }
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        // ------------------------------------------------------------- primitives

        private static GameObject Primitive(
            PrimitiveType type, Transform parent, string name,
            Vector3 position, Vector3 scale, Material material)
        {
            GameObject go = GameObject.CreatePrimitive(type);
            go.name = name;
            go.transform.SetParent(parent);
            go.transform.position = position;
            go.transform.localScale = scale;
            go.GetComponent<Renderer>().sharedMaterial = material;
            return go;
        }

        private static GameObject Box(Transform p, string n, Vector3 pos, Vector3 s, Material m) =>
            Primitive(PrimitiveType.Cube, p, n, pos, s, m);

        private static GameObject Cylinder(Transform p, string n, Vector3 pos, Vector3 s, Material m) =>
            Primitive(PrimitiveType.Cylinder, p, n, pos, s, m);

        private static GameObject Sphere(Transform p, string n, Vector3 pos, Vector3 s, Material m) =>
            Primitive(PrimitiveType.Sphere, p, n, pos, s, m);

        private static GameObject Capsule(Transform p, string n, Vector3 pos, Vector3 s, Material m) =>
            Primitive(PrimitiveType.Capsule, p, n, pos, s, m);

        private static void Wet(GameObject go, float porosity)
        {
            WetSurface wet = go.AddComponent<WetSurface>();
            SerializedObject so = new(wet);
            so.FindProperty("porosity").floatValue = porosity;
            so.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
