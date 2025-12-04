using UnityEngine;

public class ObstacleAvoidanceBehaviour : SteeringBehaviour
{
    [Header("Obstacle Avoidance Settings")]
    [SerializeField] private float lookAheadDistance = 5f;
    [SerializeField] private float avoidanceForce = 10f;
    [SerializeField] private LayerMask obstacleLayer;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
        }

        if (obstacleLayer.value == 0)
        {
            obstacleLayer = LayerMask.GetMask("Default");
        }
    }

    public override Vector3 CalculateForce()
    {
        Vector3 forward = transform.forward;
        Vector3 avoidanceForceVector = Vector3.zero;

        // Cast rays in multiple directions
        Vector3[] rayDirections = new Vector3[]
        {
            forward,
            forward + transform.right * 0.5f,
            forward - transform.right * 0.5f,
            forward + transform.up * 0.3f,
            forward - transform.up * 0.3f
        };

        foreach (Vector3 direction in rayDirections)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, direction, out hit, lookAheadDistance, obstacleLayer))
            {
                // Calculate avoidance force
                Vector3 avoidanceDirection = (transform.position - hit.point).normalized;
                float distance = hit.distance;
                float forceStrength = avoidanceForce * (1f - (distance / lookAheadDistance));
                
                avoidanceForceVector += avoidanceDirection * forceStrength;
            }
        }

        // Limit avoidance force
        avoidanceForceVector = Vector3.ClampMagnitude(avoidanceForceVector, maxForce);

        return avoidanceForceVector * weight;
    }
}

