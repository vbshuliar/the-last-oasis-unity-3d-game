#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class BonusItemSpawnerTool
{
    private const int DefaultSpawnPointCount = 6;
    private const float DefaultSpawnRadius = 5f;

    [MenuItem("Tools/Create Bonus Item Spawner")]
    public static void CreateBonusItemSpawner()
    {
        GameObject spawnerGO = new GameObject("BonusItemSpawner");
        Undo.RegisterCreatedObjectUndo(spawnerGO, "Create Bonus Item Spawner");

        ItemBonusSpawner spawner = spawnerGO.AddComponent<ItemBonusSpawner>();
        Transform[] spawnPoints = CreateSpawnPoints(spawnerGO.transform);

        SerializedObject serializedSpawner = new SerializedObject(spawner);
        serializedSpawner.FindProperty("spawnIntervalSeconds").floatValue = 15f;
        serializedSpawner.FindProperty("spawnImmediately").boolValue = true;
        serializedSpawner.FindProperty("maxSimultaneousBonuses").intValue = 1;

        SerializedProperty spawnPointsProperty = serializedSpawner.FindProperty("spawnPoints");
        spawnPointsProperty.arraySize = spawnPoints.Length;
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            spawnPointsProperty.GetArrayElementAtIndex(i).objectReferenceValue = spawnPoints[i];
        }

        serializedSpawner.ApplyModifiedPropertiesWithoutUndo();

        Selection.activeGameObject = spawnerGO;
        EditorGUIUtility.PingObject(spawnerGO);

        Debug.Log("Bonus Item Spawner created. Assign the four ItemPickup prefabs (Health, Speed, Damage, Star) in the component inspector.");
    }

    private static Transform[] CreateSpawnPoints(Transform parent)
    {
        Transform[] points = new Transform[DefaultSpawnPointCount];
        for (int i = 0; i < DefaultSpawnPointCount; i++)
        {
            GameObject point = new GameObject($"SpawnPoint_{i + 1}");
            Undo.RegisterCreatedObjectUndo(point, "Create Spawn Point");
            point.transform.SetParent(parent);

            float angle = (Mathf.PI * 2f / DefaultSpawnPointCount) * i;
            Vector3 position = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * DefaultSpawnRadius;
            point.transform.localPosition = position;
            points[i] = point.transform;
        }

        return points;
    }
}
#endif
