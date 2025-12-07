using UnityEditor;
using UnityEngine;

/// <summary>
/// Menu utility that drops a fully wired AudioManager prefab into the active scene.
/// </summary>
public static class AudioManagerSetupTool
{
    private const string MenuPath = "Tools/Audio/Create Audio Manager";

    [MenuItem(MenuPath, priority = 200)]
    public static void CreateAudioManager()
    {
        AudioManager existing = Object.FindObjectOfType<AudioManager>();
        if (existing != null)
        {
            Selection.activeObject = existing.gameObject;
            EditorUtility.DisplayDialog("Audio Manager", "An AudioManager already exists in the scene.", "OK");
            return;
        }

        GameObject root = new GameObject("AudioManager");
        Undo.RegisterCreatedObjectUndo(root, "Create Audio Manager");

        AudioManager manager = root.AddComponent<AudioManager>();
        AudioSource musicSource = CreateChildSource(root.transform, "MusicSource", loop: true);
        AudioSource sfxSource = CreateChildSource(root.transform, "SFXSource", loop: false);

        SerializedObject serializedManager = new SerializedObject(manager);
        serializedManager.FindProperty("musicSource").objectReferenceValue = musicSource;
        serializedManager.FindProperty("sfxSource").objectReferenceValue = sfxSource;
        serializedManager.ApplyModifiedPropertiesWithoutUndo();

        Selection.activeGameObject = root;
        EditorGUIUtility.PingObject(root);
        Debug.Log("AudioManager created and configured. Assign background/punch/coin/potion clips in the inspector.");
    }

    static AudioSource CreateChildSource(Transform parent, string name, bool loop)
    {
        GameObject child = new GameObject(name);
        child.transform.SetParent(parent);
        AudioSource source = child.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.loop = loop;
        source.spatialBlend = 0f;
        return source;
    }
}
