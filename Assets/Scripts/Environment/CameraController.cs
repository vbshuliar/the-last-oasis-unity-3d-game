using UnityEngine;

// smoothly follows the target using an offset
public class CameraController : MonoBehaviour
{
    public Transform target;

    public float smoothSpeed = 8f;
    public Vector3 offset;

    // lerps the camera toward the desired offset position each frame
    void Update()
    {
        if (target == null) return;

        Vector3 desiredPosition = new Vector3(target.position.x + offset.x, target.position.y + offset.y, target.position.z + offset.z);
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }
}