using UnityEngine;
using UnityEngine.AI;

// handles switching from the intro camera to the gameplay camera when ready
public class CameraIntroSwitcher : MonoBehaviour
{
    [Header("Camera References")]
    [SerializeField] private Camera animatedCamera;
    [SerializeField] private Camera mainCamera;

    [Header("Optional Components")]
    [SerializeField] private Behaviour[] componentsToEnable;

    [Header("Player Control")]
    [SerializeField] private bool disablePlayerControlsDuringIntro = true;
    [SerializeField] private PlayerController playerControllerOverride;

    PlayerController cachedPlayerController;
    NavMeshAgent cachedPlayerAgent;

    // caches player references needed for disabling controls
    void Awake()
    {
        CachePlayerReferences();
    }

    // shows the intro camera unless the save system requires skipping
    void Start()
    {
        bool skipIntro = SaveSystem.Instance != null && SaveSystem.Instance.IsRestoringSave;
        if (skipIntro)
        {
            ActivateGameplayCameraImmediate();
            return;
        }

        if (animatedCamera != null)
        {
            animatedCamera.enabled = true;
        }

        if (mainCamera != null)
        {
            mainCamera.enabled = false;
        }

        SetComponentsEnabled(false);
        SetPlayerControlsEnabled(false);
    }

    // activates the gameplay camera without waiting for animation events
    void ActivateGameplayCameraImmediate()
    {
        if (animatedCamera != null)
        {
            animatedCamera.enabled = false;
        }

        if (mainCamera != null)
        {
            mainCamera.enabled = true;
        }

        SetComponentsEnabled(true);
        SetPlayerControlsEnabled(true);
    }

    // call when the intro animation has completed to switch cameras
    public void EndIntro()
    {
        if (animatedCamera != null)
        {
            animatedCamera.enabled = false;
        }

        if (mainCamera != null)
        {
            mainCamera.enabled = true;
        }

        SetComponentsEnabled(true);
        SetPlayerControlsEnabled(true);
    }

    // toggles optional behaviours alongside the camera switch
    void SetComponentsEnabled(bool enabled)
    {
        if (componentsToEnable == null)
        {
            return;
        }

        foreach (var component in componentsToEnable)
        {
            if (component == null)
            {
                continue;
            }

            component.enabled = enabled;
        }
    }

    // finds the player controller and its navmesh agent once
    void CachePlayerReferences()
    {
        if (cachedPlayerController != null)
        {
            return;
        }

        cachedPlayerController = playerControllerOverride != null
            ? playerControllerOverride
            : FindObjectOfType<PlayerController>();

        if (cachedPlayerController != null)
        {
            cachedPlayerAgent = cachedPlayerController.GetComponent<NavMeshAgent>();
        }
    }

    // enables or disables the player controller during the intro
    void SetPlayerControlsEnabled(bool enable)
    {
        if (!disablePlayerControlsDuringIntro)
        {
            return;
        }

        CachePlayerReferences();

        if (cachedPlayerController == null)
        {
            return;
        }

        if (!enable && cachedPlayerAgent != null)
        {
            cachedPlayerAgent.ResetPath();
        }

        cachedPlayerController.enabled = enable;
    }
}
