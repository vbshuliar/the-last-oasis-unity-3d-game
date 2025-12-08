#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditor.AI;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// One-click utility that bakes the NavMesh for the currently active scene.
/// </summary>
public static class NavMeshSceneBaker
{
    private const string MenuPath = "Tools/NavMesh/Build Scene NavMesh";

    [MenuItem(MenuPath, priority = 220)]
    public static void BuildActiveSceneNavMesh()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            Debug.Log("NavMesh bake cancelled: scene changes were not saved.");
            return;
        }

        try
        {
            NavMeshBuilder.ClearAllNavMeshes();
            NavMeshBuilder.BuildNavMesh();
            Scene scene = SceneManager.GetActiveScene();
            Debug.Log($"NavMesh baked for scene '{scene.name}'.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"NavMesh bake failed: {ex.Message}");
        }
    }

    [MenuItem(MenuPath, true)]
    public static bool ValidateBuildActiveSceneNavMesh()
    {
        return !Application.isPlaying;
    }
}
#endif
