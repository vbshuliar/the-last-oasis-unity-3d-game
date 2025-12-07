#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

/// <summary>
/// Creates or updates the enemy/boss spawners so they spawn far from the arena center.
/// </summary>
public class SpawningSetupTool : EditorWindow
{
    const string ToolPath = "Tools/Spawning Setup";
    const string CenterObjectName = "SpawnCenter";

    [MenuItem(ToolPath)]
    public static void ShowWindow()
    {
        GetWindow<SpawningSetupTool>("Spawning Setup");
    }

    void OnGUI()
    {
        GUILayout.Label("Enemy & Boss Spawner Setup", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "This tool drops an EnemySpawner and BossSpawner into the current scene, " +
            "assigns a shared SpawnCenter, and configures them to spawn far from the origin.",
            MessageType.Info);

        if (GUILayout.Button("Create / Update Spawners", GUILayout.Height(30f)))
        {
            CreateOrUpdate();
        }

        GUILayout.Space(10f);
        DrawCurrentStatus();
    }

    void CreateOrUpdate()
    {
        Transform center = GetOrCreateCenter();
        SetupEnemySpawner(center);
        SetupBossSpawner(center);
    }

    Transform GetOrCreateCenter()
    {
        GameObject centerObj = GameObject.Find(CenterObjectName);
        if (centerObj == null)
        {
            centerObj = new GameObject(CenterObjectName);
            centerObj.transform.position = Vector3.zero;
            Undo.RegisterCreatedObjectUndo(centerObj, "Create Spawn Center");
        }

        Selection.activeObject = centerObj;
        EditorGUIUtility.PingObject(centerObj);
        return centerObj.transform;
    }

    void SetupEnemySpawner(Transform center)
    {
        EnemySpawner spawner = FindObjectOfType<EnemySpawner>(true);
        if (spawner == null)
        {
            GameObject spawnerObj = new GameObject("EnemySpawner");
            spawner = spawnerObj.AddComponent<EnemySpawner>();
            Undo.RegisterCreatedObjectUndo(spawnerObj, "Create Enemy Spawner");
        }
        else
        {
            Undo.RecordObject(spawner.gameObject, "Update Enemy Spawner");
        }

        SerializedObject so = new SerializedObject(spawner);
        so.FindProperty("spawnCenter").objectReferenceValue = center;
        so.FindProperty("useRandomSpawnPositions").boolValue = true;
        so.FindProperty("minSpawnDistance").floatValue = 35f;
        so.FindProperty("maxSpawnDistance").floatValue = 85f;
        so.ApplyModifiedProperties();
    }

    void SetupBossSpawner(Transform center)
    {
        BossSpawner spawner = FindObjectOfType<BossSpawner>(true);
        if (spawner == null)
        {
            GameObject spawnerObj = new GameObject("BossSpawner");
            spawner = spawnerObj.AddComponent<BossSpawner>();
            Undo.RegisterCreatedObjectUndo(spawnerObj, "Create Boss Spawner");
        }
        else
        {
            Undo.RecordObject(spawner.gameObject, "Update Boss Spawner");
        }

        SerializedObject so = new SerializedObject(spawner);
        so.FindProperty("spawnCenter").objectReferenceValue = center;
        so.FindProperty("minSpawnDistance").floatValue = 35f;
        so.FindProperty("maxSpawnDistance").floatValue = 90f;
        so.FindProperty("spawnDelaySeconds").floatValue = 90f;
        so.ApplyModifiedProperties();
    }

    void DrawCurrentStatus()
    {
        EnemySpawner enemySpawner = FindObjectOfType<EnemySpawner>(true);
        BossSpawner bossSpawner = FindObjectOfType<BossSpawner>(true);

        if (enemySpawner == null && bossSpawner == null)
        {
            EditorGUILayout.HelpBox("No spawners detected in the open scene.", MessageType.Warning);
            return;
        }

        if (enemySpawner != null)
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                SerializedObject so = new SerializedObject(enemySpawner);
                EditorGUILayout.LabelField("Enemy Spawner", enemySpawner.gameObject.name);
                EditorGUILayout.LabelField("Min Distance", so.FindProperty("minSpawnDistance").floatValue.ToString("F1"));
                EditorGUILayout.LabelField("Max Distance", so.FindProperty("maxSpawnDistance").floatValue.ToString("F1"));
                EditorGUILayout.ObjectField("Center", so.FindProperty("spawnCenter").objectReferenceValue, typeof(Transform), true);
            }
        }

        if (bossSpawner != null)
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                SerializedObject so = new SerializedObject(bossSpawner);
                EditorGUILayout.LabelField("Boss Spawner", bossSpawner.gameObject.name);
                EditorGUILayout.LabelField("Delay", so.FindProperty("spawnDelaySeconds").floatValue.ToString("F1") + " s");
                EditorGUILayout.LabelField("Min Distance", so.FindProperty("minSpawnDistance").floatValue.ToString("F1"));
                EditorGUILayout.LabelField("Max Distance", so.FindProperty("maxSpawnDistance").floatValue.ToString("F1"));
                EditorGUILayout.ObjectField("Center", so.FindProperty("spawnCenter").objectReferenceValue, typeof(Transform), true);
            }
        }
    }
}
#endif
