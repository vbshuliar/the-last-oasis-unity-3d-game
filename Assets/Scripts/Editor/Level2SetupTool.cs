using UnityEngine;
using UnityEditor;
using System.IO;

// Unity Editor tool to automatically set up Level 2 (Jungle) with all required components
public class Level2SetupTool : EditorWindow
{
    private GameObject level2Manager;
    private GameObject enemySpawnPointsParent;
    private GameObject itemSpawnPointsParent;
    private GameObject bossSpawnPointsParent;

    [MenuItem("Tools/Level 2 Setup Tool")]
    public static void ShowWindow()
    {
        GetWindow<Level2SetupTool>("Level 2 Setup Tool");
    }

    void OnGUI()
    {
        GUILayout.Label("Level 2 (Jungle) Setup Tool", EditorStyles.boldLabel);
        GUILayout.Space(10);

        EditorGUILayout.HelpBox(
            "This tool will help you set up Level 2 with:\n" +
            "- Enemy spawning (increasing rate over time)\n" +
            "- Item spawning (every 20 seconds)\n" +
            "- Boss spawning (at 4 minutes)\n\n" +
            "Make sure you have your prefabs ready before running this tool!",
            MessageType.Info);

        GUILayout.Space(10);

        if (GUILayout.Button("Setup Level 2 Scene", GUILayout.Height(30)))
        {
            SetupLevel2();
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Create Spawn Point Groups", GUILayout.Height(30)))
        {
            CreateSpawnPointGroups();
        }

        GUILayout.Space(10);

        EditorGUILayout.HelpBox(
            "Instructions:\n" +
            "1. Click 'Create Spawn Point Groups' to create empty GameObjects for spawn points\n" +
            "2. Position the spawn point GameObjects around your map\n" +
            "3. Select the Level2Manager GameObject in the scene\n" +
            "4. In the Inspector, assign:\n" +
            "   - Enemy prefabs array\n" +
            "   - Item prefabs array (Star, SpeedBoost, HealthPack, DamageBoost)\n" +
            "   - Boss prefab\n" +
            "   - Spawn point arrays\n" +
            "5. Make sure all prefabs are properly configured with required components",
            MessageType.Info);
    }

    void SetupLevel2()
    {
        // Find or create Level2Manager GameObject
        level2Manager = GameObject.Find("Level2Manager");
        if (level2Manager == null)
        {
            level2Manager = new GameObject("Level2Manager");
            level2Manager.AddComponent<Level2Manager>();
            Debug.Log("Created Level2Manager GameObject");
        }
        else
        {
            // Check if it already has Level2Manager component
            Level2Manager manager = level2Manager.GetComponent<Level2Manager>();
            if (manager == null)
            {
                level2Manager.AddComponent<Level2Manager>();
                Debug.Log("Added Level2Manager component to existing GameObject");
            }
        }

        Selection.activeGameObject = level2Manager;
        EditorGUIUtility.PingObject(level2Manager);

        Debug.Log("Level 2 setup complete! Please configure the Level2Manager component in the Inspector.");
        Debug.Log("You need to assign:");
        Debug.Log("1. Enemy prefabs array");
        Debug.Log("2. Item prefabs array (Star, SpeedBoost, HealthPack, DamageBoost)");
        Debug.Log("3. Boss prefab");
        Debug.Log("4. Spawn point arrays (create spawn point groups first)");
    }

    void CreateSpawnPointGroups()
    {
        // Create parent objects for organizing spawn points
        enemySpawnPointsParent = GameObject.Find("EnemySpawnPoints");
        if (enemySpawnPointsParent == null)
        {
            enemySpawnPointsParent = new GameObject("EnemySpawnPoints");
            Debug.Log("Created EnemySpawnPoints parent GameObject");
        }

        itemSpawnPointsParent = GameObject.Find("ItemSpawnPoints");
        if (itemSpawnPointsParent == null)
        {
            itemSpawnPointsParent = new GameObject("ItemSpawnPoints");
            Debug.Log("Created ItemSpawnPoints parent GameObject");
        }

        bossSpawnPointsParent = GameObject.Find("BossSpawnPoints");
        if (bossSpawnPointsParent == null)
        {
            bossSpawnPointsParent = new GameObject("BossSpawnPoints");
            Debug.Log("Created BossSpawnPoints parent GameObject");
        }

        // Create some initial spawn points
        CreateInitialSpawnPoints(enemySpawnPointsParent, "EnemySpawnPoint", 8);
        CreateInitialSpawnPoints(itemSpawnPointsParent, "ItemSpawnPoint", 12);
        CreateInitialSpawnPoints(bossSpawnPointsParent, "BossSpawnPoint", 3);

        Debug.Log("Spawn point groups created! Position them around your map, then assign them to Level2Manager.");
    }

    void CreateInitialSpawnPoints(GameObject parent, string namePrefix, int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject spawnPoint = new GameObject(namePrefix + "_" + (i + 1));
            spawnPoint.transform.SetParent(parent.transform);
            
            // Position in a circle pattern around origin (you can move them manually)
            float angle = (360f / count) * i;
            float radius = 15f;
            float x = Mathf.Sin(angle * Mathf.Deg2Rad) * radius;
            float z = Mathf.Cos(angle * Mathf.Deg2Rad) * radius;
            spawnPoint.transform.position = new Vector3(x, 0, z);
        }
    }
}

