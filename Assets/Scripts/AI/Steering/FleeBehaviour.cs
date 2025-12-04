using UnityEngine;

public class FleeBehaviour : SteeringBehaviour
{
    [Header("Flee Settings")]
    [SerializeField] private Transform threat;
    [SerializeField] private float fleeDistance = 10f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
        }
    }

    public override Vector3 CalculateForce()
    {
        if (threat == null) return Vector3.zero;

        float distance = Vector3.Distance(transform.position, threat.position);
        
        // Only flee if threat is within flee distance
        if (distance > fleeDistance) return Vector3.zero;

        Vector3 desiredVelocity = (transform.position - threat.position).normalized * maxSpeed;
        Vector3 steering = desiredVelocity - (rb != null ? rb.linearVelocity : Vector3.zero);

        // Limit steering force
        steering = Vector3.ClampMagnitude(steering, maxForce);

        return steering * weight;
    }

    public void SetThreat(Transform newThreat)
    {
        threat = newThreat;
    }

    public Transform GetThreat()
    {
        return threat;
    }
}

