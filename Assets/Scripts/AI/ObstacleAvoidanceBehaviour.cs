using UnityEngine;

// samples several rays forward to create avoidance steering forces
public class ObstacleAvoidanceBehaviour : SteeringBehaviour
{
    [Header("Obstacle Avoidance Settings")]
    [SerializeField] private float lookAheadDistance = 5f;
    [SerializeField] private float avoidanceForce = 10f;
    [SerializeField] private LayerMask obstacleLayer;

    private Rigidbody rb;

    // prepares rigidbody data and default layers if none were assigned
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

    // computes the summed avoidance force from raycast hits
    public override Vector3 CalculateForce()
    {
        Vector3 forward = transform.forward;
        Vector3 avoidanceForceVector = Vector3.zero;

        // cast rays in multiple directions
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
                // calculate avoidance force
                Vector3 avoidanceDirection = (transform.position - hit.point).normalized;
                float distance = hit.distance;
                float forceStrength = avoidanceForce * (1f - (distance / lookAheadDistance));

                avoidanceForceVector += avoidanceDirection * forceStrength;
            }
        }

        // limit avoidance force
        avoidanceForceVector = Vector3.ClampMagnitude(avoidanceForceVector, maxForce);

        return avoidanceForceVector * weight;
    }
}

