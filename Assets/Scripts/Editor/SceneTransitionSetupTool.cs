#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

/// <summary>
/// Simple helper window that ensures a SceneTransitionManager exists in the open scene
/// and that it has a fade image ready to drive cross-scene fades.
/// </summary>
public class SceneTransitionSetupTool : EditorWindow
{
    private const string ToolPath = "Tools/Scene Transition Setup";
    private Color previewFadeColor = Color.black;
    private float previewFadeDuration = 1f;

    [MenuItem(ToolPath)]
    public static void ShowWindow()
    {
        GetWindow<SceneTransitionSetupTool>("Scene Transition Setup");
    }

    void OnGUI()
    {
        GUILayout.Label("Scene Transition Manager", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "Use this tool to drop a SceneTransitionManager into the current scene " +
            "and configure the default fade colour/duration without digging through the hierarchy.",
            MessageType.Info);

        previewFadeColor = EditorGUILayout.ColorField("Fade Color", previewFadeColor);
        previewFadeDuration = Mathf.Max(0.01f, EditorGUILayout.FloatField("Fade Duration", previewFadeDuration));

        GUILayout.Space(8f);

        if (GUILayout.Button("Create / Update Manager", GUILayout.Height(30f)))
        {
            CreateOrUpdateManager();
        }

        GUILayout.Space(12f);
        DrawCurrentState();
    }

    void CreateOrUpdateManager()
    {
        SceneTransitionManager manager = FindObjectOfType<SceneTransitionManager>();
        if (manager == null)
        {
            GameObject managerObj = new GameObject("SceneTransitionManager");
            manager = managerObj.AddComponent<SceneTransitionManager>();
            Undo.RegisterCreatedObjectUndo(managerObj, "Create Scene Transition Manager");
        }
        else
        {
            Undo.RecordObject(manager.gameObject, "Update Scene Transition Manager");
        }

        SerializedObject so = new SerializedObject(manager);
        so.FindProperty("fadeColor").colorValue = previewFadeColor;
        so.FindProperty("fadeDuration").floatValue = previewFadeDuration;
        so.ApplyModifiedProperties();

        if (manager.gameObject.scene.path == null)
        {
            EditorUtility.DisplayDialog("Scene Transition Setup",
                "The manager lives in a prefab instance. Open a scene to place it in-scene.",
                "OK");
        }
        else
        {
            Selection.activeObject = manager.gameObject;
            EditorGUIUtility.PingObject(manager.gameObject);
        }
    }

    void DrawCurrentState()
    {
        SceneTransitionManager manager = FindObjectOfType<SceneTransitionManager>();
        if (manager == null)
        {
            EditorGUILayout.HelpBox("No SceneTransitionManager found in the current scene.", MessageType.Warning);
            return;
        }

        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            SerializedObject so = new SerializedObject(manager);
            float duration = so.FindProperty("fadeDuration").floatValue;
            Color color = so.FindProperty("fadeColor").colorValue;

            EditorGUILayout.LabelField("Current Manager", manager.gameObject.name);
            EditorGUILayout.LabelField("Fade Duration", duration.ToString("F2") + "s");
            EditorGUILayout.LabelField("Fade Color", color.ToString());
            EditorGUILayout.ObjectField("Fade Image",
                manager.GetComponentInChildren<UnityEngine.UI.Image>(true),
                typeof(UnityEngine.UI.Image),
                true);
        }
    }
}
#endif
