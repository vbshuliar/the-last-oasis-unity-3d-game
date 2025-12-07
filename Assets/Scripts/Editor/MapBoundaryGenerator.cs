using UnityEngine;
using UnityEditor;

// Unity Editor tool to generate invisible barriers around the map to prevent out-of-bounds movement
public class MapBoundaryGenerator : EditorWindow
{
    private float mapSizeX = 30f;
    private float mapSizeZ = 30f;
    private Vector3 mapCenter = Vector3.zero;
    private float barrierHeight = 10f;
    private float barrierThickness = 1f;
    private string barrierTag = "Untagged";
    private bool deleteExistingBarriers = false;
    private bool useNavMeshObstacle = true;
    private bool createBoundaryChecker = true;
    private bool showBarriersInScene = false;

    [MenuItem("Tools/Map Boundary Generator")]
    public static void ShowWindow()
    {
        GetWindow<MapBoundaryGenerator>("Map Boundary Generator");
    }

    void OnGUI()
    {
        GUILayout.Label("Map Boundary Generator", EditorStyles.boldLabel);
        GUILayout.Space(10);

        EditorGUILayout.HelpBox(
            "This tool generates invisible collider barriers around your map to prevent players from going out of bounds.\n\n" +
            "Default: 30x30 map centered at origin (0, 0, 0)",
            MessageType.Info);

        GUILayout.Space(10);

        // Map settings
        EditorGUILayout.LabelField("Map Settings", EditorStyles.boldLabel);
        mapSizeX = EditorGUILayout.FloatField("Map Size X", mapSizeX);
        mapSizeZ = EditorGUILayout.FloatField("Map Size Z", mapSizeZ);
        mapCenter = EditorGUILayout.Vector3Field("Map Center", mapCenter);

        GUILayout.Space(10);

        // Barrier settings
        EditorGUILayout.LabelField("Barrier Settings", EditorStyles.boldLabel);
        barrierHeight = EditorGUILayout.FloatField("Barrier Height", barrierHeight);
        barrierThickness = EditorGUILayout.FloatField("Barrier Thickness", barrierThickness);
        barrierTag = EditorGUILayout.TagField("Barrier Tag", barrierTag);

        GUILayout.Space(10);

        // Options
        EditorGUILayout.LabelField("Options", EditorStyles.boldLabel);
        deleteExistingBarriers = EditorGUILayout.Toggle("Delete Existing Barriers", deleteExistingBarriers);
        useNavMeshObstacle = EditorGUILayout.Toggle("Use NavMesh Obstacle", useNavMeshObstacle);
        createBoundaryChecker = EditorGUILayout.Toggle("Create Boundary Checker Script", createBoundaryChecker);
        showBarriersInScene = EditorGUILayout.Toggle("Show Barriers in Scene (Debug)", showBarriersInScene);

        GUILayout.Space(20);

        // Generate button
        if (GUILayout.Button("Generate Map Boundaries", GUILayout.Height(40)))
        {
            GenerateBoundaries();
        }

        GUILayout.Space(10);

        // Quick preset buttons
        EditorGUILayout.LabelField("Quick Presets", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("30x30 Map", GUILayout.Height(25)))
        {
            mapSizeX = 30f;
            mapSizeZ = 30f;
            mapCenter = Vector3.zero;
        }
        if (GUILayout.Button("50x50 Map", GUILayout.Height(25)))
        {
            mapSizeX = 50f;
            mapSizeZ = 50f;
            mapCenter = Vector3.zero;
        }
        if (GUILayout.Button("100x100 Map", GUILayout.Height(25)))
        {
            mapSizeX = 100f;
            mapSizeZ = 100f;
            mapCenter = Vector3.zero;
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);

        EditorGUILayout.HelpBox(
            "The barriers will be created as invisible Box Colliders positioned at the edges of your map.\n" +
            "They will be organized under a 'MapBoundaries' parent GameObject.\n\n" +
            "⚠️ IMPORTANT: After generating barriers, you MUST rebake your NavMesh!\n" +
            "Go to: Window → AI → Navigation → Bake tab → Click 'Bake'",
            MessageType.Warning);
    }

    void GenerateBoundaries()
    {
        // Calculate boundaries
        float halfSizeX = mapSizeX / 2f;
        float halfSizeZ = mapSizeZ / 2f;
        
        // Delete existing barriers if requested
        if (deleteExistingBarriers)
        {
            GameObject existingBarriers = GameObject.Find("MapBoundaries");
            if (existingBarriers != null)
            {
                DestroyImmediate(existingBarriers);
                Debug.Log("Deleted existing map boundaries");
            }
        }

        // Check if boundaries already exist
        GameObject boundariesParent = GameObject.Find("MapBoundaries");
        if (boundariesParent != null && !deleteExistingBarriers)
        {
            if (EditorUtility.DisplayDialog("Boundaries Already Exist",
                "MapBoundaries GameObject already exists. Delete existing boundaries and create new ones?",
                "Yes", "No"))
            {
                DestroyImmediate(boundariesParent);
            }
            else
            {
                Debug.Log("Boundary generation cancelled");
                return;
            }
        }

        // Create parent GameObject
        boundariesParent = new GameObject("MapBoundaries");
        boundariesParent.transform.position = mapCenter;

        // Calculate positions for the 4 walls (positioned at exact boundaries)
        float northWallZ = mapCenter.z + halfSizeZ + barrierThickness / 2f;
        float southWallZ = mapCenter.z - halfSizeZ - barrierThickness / 2f;
        float eastWallX = mapCenter.x + halfSizeX + barrierThickness / 2f;
        float westWallX = mapCenter.x - halfSizeX - barrierThickness / 2f;

        // Create North Wall (extends full width)
        CreateWall("NorthWall", 
            new Vector3(mapCenter.x, mapCenter.y + barrierHeight / 2f, northWallZ),
            new Vector3(mapSizeX + barrierThickness * 2f, barrierHeight, barrierThickness),
            boundariesParent.transform);

        // Create South Wall (extends full width)
        CreateWall("SouthWall",
            new Vector3(mapCenter.x, mapCenter.y + barrierHeight / 2f, southWallZ),
            new Vector3(mapSizeX + barrierThickness * 2f, barrierHeight, barrierThickness),
            boundariesParent.transform);

        // Create East Wall (extends full height, but not overlapping corners)
        CreateWall("EastWall",
            new Vector3(eastWallX, mapCenter.y + barrierHeight / 2f, mapCenter.z),
            new Vector3(barrierThickness, barrierHeight, mapSizeZ),
            boundariesParent.transform);

        // Create West Wall (extends full height, but not overlapping corners)
        CreateWall("WestWall",
            new Vector3(westWallX, mapCenter.y + barrierHeight / 2f, mapCenter.z),
            new Vector3(barrierThickness, barrierHeight, mapSizeZ),
            boundariesParent.transform);

        // Select the parent GameObject
        Selection.activeGameObject = boundariesParent;
        EditorGUIUtility.PingObject(boundariesParent);

        // Create boundary checker if requested
        if (createBoundaryChecker)
        {
            CreateBoundaryChecker();
        }

        Debug.Log($"Map boundaries generated successfully! Map size: {mapSizeX}x{mapSizeZ}, Center: {mapCenter}");
        Debug.Log($"Barriers created at: X=[{westWallX}, {eastWallX}], Z=[{southWallZ}, {northWallZ}]");
        
        if (useNavMeshObstacle)
        {
            Debug.LogWarning("⚠️ CRITICAL: You must rebake your NavMesh now!");
            Debug.LogWarning("Go to: Window → AI → Navigation → Bake tab → Click 'Bake'");
            Debug.LogWarning("Without rebaking, NavMeshObstacles won't work and barriers will be ignored!");
        }
        
        if (createBoundaryChecker)
        {
            Debug.Log("Boundary checker script created. Player will be teleported back if they go out of bounds.");
        }
        
        EditorUtility.DisplayDialog("Boundaries Generated",
            "Map boundaries have been created!\n\n" +
            "⚠️ IMPORTANT NEXT STEP:\n" +
            "You MUST rebake your NavMesh for barriers to work!\n\n" +
            "1. Go to: Window → AI → Navigation\n" +
            "2. Click the 'Bake' tab\n" +
            "3. Click 'Bake' button\n\n" +
            "Without rebaking, NavMesh agents can still go through barriers!",
            "Got it!");
    }

    void CreateWall(string name, Vector3 position, Vector3 size, Transform parent)
    {
        // Create empty GameObject (no renderer needed)
        GameObject wall = new GameObject(name);
        wall.transform.SetParent(parent);
        wall.transform.position = position;
        
        // Add BoxCollider and set its size directly (more efficient than scaling)
        BoxCollider collider = wall.AddComponent<BoxCollider>();
        collider.size = size;
        collider.isTrigger = false; // Solid barrier

        // Set tag
        if (!string.IsNullOrEmpty(barrierTag))
        {
            wall.tag = barrierTag;
        }

        // Add NavMeshObstacle to block NavMesh paths
        if (useNavMeshObstacle)
        {
            UnityEngine.AI.NavMeshObstacle obstacle = wall.AddComponent<UnityEngine.AI.NavMeshObstacle>();
            obstacle.shape = UnityEngine.AI.NavMeshObstacleShape.Box;
            obstacle.size = size;
            obstacle.carving = true; // Carve out NavMesh so agents can't path through
            obstacle.carveOnlyStationary = false;
        }

        // Visual representation for debugging
        if (showBarriersInScene)
        {
            GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Cube);
            visual.name = name + "_Visual";
            visual.transform.SetParent(wall.transform);
            visual.transform.localPosition = Vector3.zero;
            visual.transform.localScale = size;
            
            Renderer renderer = visual.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material mat = new Material(Shader.Find("Standard"));
                mat.color = new Color(1f, 0f, 0f, 0.3f); // Semi-transparent red
                renderer.material = mat;
            }
            
            Collider visualCollider = visual.GetComponent<Collider>();
            if (visualCollider != null)
            {
                DestroyImmediate(visualCollider);
            }
        }
        
        // Mark as static (won't interfere with NavMesh if excluded)
        wall.isStatic = false; // Don't mark as static if using NavMeshObstacle
    }

    void CreateBoundaryChecker()
    {
        // Check if boundary checker already exists
        GameObject checker = GameObject.Find("MapBoundaryChecker");
        if (checker != null)
        {
            if (EditorUtility.DisplayDialog("Boundary Checker Exists",
                "MapBoundaryChecker already exists. Replace it?",
                "Yes", "No"))
            {
                DestroyImmediate(checker);
            }
            else
            {
                return;
            }
        }

        // Create boundary checker GameObject
        checker = new GameObject("MapBoundaryChecker");
        MapBoundaryChecker boundaryScript = checker.AddComponent<MapBoundaryChecker>();
        
        // Set values directly (fields are now public)
        boundaryScript.mapSizeX = mapSizeX;
        boundaryScript.mapSizeZ = mapSizeZ;
        boundaryScript.mapCenter = mapCenter;

        Debug.Log("MapBoundaryChecker created! It will automatically prevent player from going out of bounds.");
    }
}

