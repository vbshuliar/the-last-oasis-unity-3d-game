using UnityEngine;

// generates a steering force that runs away from a threat
public class FleeBehaviour : SteeringBehaviour
{
    [Header("Flee Settings")]
    [SerializeField] private Transform threat;
    [SerializeField] private float fleeDistance = 10f;

    private Rigidbody rb;

    // ensures a rigidbody is available for velocity data
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
        }
    }

    // returns the flee steering force based on current threat position
    public override Vector3 CalculateForce()
    {
        if (threat == null) return Vector3.zero;

        float distance = Vector3.Distance(transform.position, threat.position);

        // only flee if threat is within flee distance
        if (distance > fleeDistance) return Vector3.zero;

        Vector3 desiredVelocity = (transform.position - threat.position).normalized * maxSpeed;
        Vector3 steering = desiredVelocity - (rb != null ? rb.linearVelocity : Vector3.zero);

        // limit steering force
        steering = Vector3.ClampMagnitude(steering, maxForce);

        return steering * weight;
    }

    // assigns the transform that should be avoided
    public void SetThreat(Transform newThreat)
    {
        threat = newThreat;
    }

    // returns the current threat transform
    public Transform GetThreat()
    {
        return threat;
    }
}

