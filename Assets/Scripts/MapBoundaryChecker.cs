using UnityEngine;

// Script that prevents player from going out of bounds by teleporting them back
// Attach this to the player or create a manager that checks boundaries
public class MapBoundaryChecker : MonoBehaviour
{
    [Header("Boundary Settings")]
    public float mapSizeX = 30f;
    public float mapSizeZ = 30f;
    public Vector3 mapCenter = Vector3.zero;
    [SerializeField] private float checkInterval = 0.1f; // Check every 0.1 seconds
    [SerializeField] private float boundaryBuffer = 0.5f; // How close to boundary before teleporting back

    private float lastCheckTime = 0f;
    private Transform playerTransform;
    private UnityEngine.AI.NavMeshAgent playerAgent;

    void Start()
    {
        // Find player if not attached to player
        if (transform.CompareTag("Player"))
        {
            playerTransform = transform;
            playerAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        }
        else
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
                playerAgent = player.GetComponent<UnityEngine.AI.NavMeshAgent>();
            }
        }
    }

    void Update()
    {
        if (playerTransform == null) return;
        if (Time.time - lastCheckTime < checkInterval) return;

        lastCheckTime = Time.time;
        CheckBoundaries();
    }

    void CheckBoundaries()
    {
        Vector3 playerPos = playerTransform.position;
        float halfSizeX = mapSizeX / 2f;
        float halfSizeZ = mapSizeZ / 2f;

        bool outOfBounds = false;
        Vector3 newPosition = playerPos;

        // Check X boundaries
        if (playerPos.x > mapCenter.x + halfSizeX - boundaryBuffer)
        {
            newPosition.x = mapCenter.x + halfSizeX - boundaryBuffer;
            outOfBounds = true;
        }
        else if (playerPos.x < mapCenter.x - halfSizeX + boundaryBuffer)
        {
            newPosition.x = mapCenter.x - halfSizeX + boundaryBuffer;
            outOfBounds = true;
        }

        // Check Z boundaries
        if (playerPos.z > mapCenter.z + halfSizeZ - boundaryBuffer)
        {
            newPosition.z = mapCenter.z + halfSizeZ - boundaryBuffer;
            outOfBounds = true;
        }
        else if (playerPos.z < mapCenter.z - halfSizeZ + boundaryBuffer)
        {
            newPosition.z = mapCenter.z - halfSizeZ + boundaryBuffer;
            outOfBounds = true;
        }

        // Teleport player back if out of bounds
        if (outOfBounds)
        {
            if (playerAgent != null)
            {
                playerAgent.Warp(newPosition); // Warp preserves NavMeshAgent state
            }
            else
            {
                playerTransform.position = newPosition;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        // Visualize boundaries in Scene view
        Gizmos.color = Color.red;
        float halfSizeX = mapSizeX / 2f;
        float halfSizeZ = mapSizeZ / 2f;

        Vector3 corner1 = mapCenter + new Vector3(-halfSizeX, 0, -halfSizeZ);
        Vector3 corner2 = mapCenter + new Vector3(halfSizeX, 0, -halfSizeZ);
        Vector3 corner3 = mapCenter + new Vector3(halfSizeX, 0, halfSizeZ);
        Vector3 corner4 = mapCenter + new Vector3(-halfSizeX, 0, halfSizeZ);

        Gizmos.DrawLine(corner1, corner2);
        Gizmos.DrawLine(corner2, corner3);
        Gizmos.DrawLine(corner3, corner4);
        Gizmos.DrawLine(corner4, corner1);
    }
}

