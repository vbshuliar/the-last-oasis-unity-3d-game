using UnityEngine;

// simple clamp that keeps the player within a square centered on map center
public class MapBoundaryChecker : MonoBehaviour
{
    [Header("Boundary Settings")]
    [SerializeField] private float limitX = 25f;
    [SerializeField] private float limitZ = 25f;
    [SerializeField] private Vector3 mapCenter = Vector3.zero;

    private Transform playerTransform;
    private UnityEngine.AI.NavMeshAgent playerAgent;

    // validates attachment and caches references
    void Start()
    {
        if (!transform.CompareTag("Player"))
        {
            Debug.LogWarning("MapBoundaryChecker: This component should only be attached to the player. Disabling on '" + gameObject.name + "'.");
            enabled = false;
            return;
        }

        playerTransform = transform;
        playerAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    // clamps the player position each frame
    void Update()
    {
        if (playerTransform == null)
        {
            return;
        }

        ClampPosition();
    }

    // constrains the player within the configured rectangle
    void ClampPosition()
    {
        Vector3 pos = playerTransform.position;
        Vector3 clamped = pos;

        clamped.x = Mathf.Clamp(pos.x, mapCenter.x - limitX, mapCenter.x + limitX);
        clamped.z = Mathf.Clamp(pos.z, mapCenter.z - limitZ, mapCenter.z + limitZ);

        if (clamped == pos)
        {
            return;
        }

        if (playerAgent != null)
        {
            playerAgent.Warp(clamped);
        }
        else
        {
            playerTransform.position = clamped;
        }
    }

    // draws the boundary square for debugging
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 corner1 = mapCenter + new Vector3(-limitX, 0f, -limitZ);
        Vector3 corner2 = mapCenter + new Vector3(limitX, 0f, -limitZ);
        Vector3 corner3 = mapCenter + new Vector3(limitX, 0f, limitZ);
        Vector3 corner4 = mapCenter + new Vector3(-limitX, 0f, limitZ);

        Gizmos.DrawLine(corner1, corner2);
        Gizmos.DrawLine(corner2, corner3);
        Gizmos.DrawLine(corner3, corner4);
        Gizmos.DrawLine(corner4, corner1);
    }
}

