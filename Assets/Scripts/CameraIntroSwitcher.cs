using UnityEngine;

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
}
