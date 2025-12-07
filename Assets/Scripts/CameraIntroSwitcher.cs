using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Enables an animated camera for the opening fly-through, then switches to the gameplay camera
/// once the animation finishes.
/// Call <see cref="EndIntro"/> via an Animation Event, Timeline signal, or script when the
/// intro sequence has reached the gameplay framing.
/// </summary>
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

    void Awake()
    {
        CachePlayerReferences();
    }

    void Start()
    {
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

    /// <summary>
    /// Call when the intro camera animation finishes to switch rendering back to the gameplay camera.
    /// </summary>
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
