using UnityEngine;

public class SeekBehaviour : SteeringBehaviour
{
    [Header("Seek Settings")]
    [SerializeField] private Transform target;
    [SerializeField] private float seekRadius = 0.5f;

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
        if (target == null) return Vector3.zero;

        Vector3 desiredVelocity = (target.position - transform.position).normalized * maxSpeed;
        Vector3 steering = desiredVelocity - (rb != null ? rb.linearVelocity : Vector3.zero);

        // Limit steering force
        steering = Vector3.ClampMagnitude(steering, maxForce);

        return steering * weight;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public Transform GetTarget()
    {
        return target;
    }
}

