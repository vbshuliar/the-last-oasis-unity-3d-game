using UnityEngine;

// steers directly toward the assigned target transform
public class SeekBehaviour : SteeringBehaviour
{
    [Header("Seek Settings")]
    [SerializeField] private Transform target;
    [SerializeField] private float seekRadius = 0.5f;

    private Rigidbody rb;

    // ensures a rigidbody exists so velocity can be sampled
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
        }
    }

    // produces the seek steering force toward the target
    public override Vector3 CalculateForce()
    {
        if (target == null) return Vector3.zero;

        Vector3 desiredVelocity = (target.position - transform.position).normalized * maxSpeed;
        Vector3 steering = desiredVelocity - (rb != null ? rb.linearVelocity : Vector3.zero);

        // limit steering force
        steering = Vector3.ClampMagnitude(steering, maxForce);

        return steering * weight;
    }

    // updates the target transform to chase
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    // exposes the current target transform
    public Transform GetTarget()
    {
        return target;
    }
}

