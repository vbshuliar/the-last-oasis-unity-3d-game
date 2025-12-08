using UnityEngine;

// predicts the target path and steers toward the intercept point
public class PursuitBehaviour : SteeringBehaviour
{
    [Header("Pursuit Settings")]
    [SerializeField] private Transform target;
    [SerializeField] private float predictionTime = 2f;

    private Rigidbody rb;
    private Rigidbody targetRb;

    // acquires rigidbody references for both pursuer and target
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
        }

        if (target != null)
        {
            targetRb = target.GetComponent<Rigidbody>();
        }
    }

    // returns the pursuit steering force aimed at predicted future position
    public override Vector3 CalculateForce()
    {
        if (target == null) return Vector3.zero;

        // predict target's future position
        Vector3 predictedPosition = target.position;

        if (targetRb != null)
        {
            predictedPosition = target.position + targetRb.linearVelocity * predictionTime;
        }

        Vector3 desiredVelocity = (predictedPosition - transform.position).normalized * maxSpeed;
        Vector3 steering = desiredVelocity - (rb != null ? rb.linearVelocity : Vector3.zero);

        // limit steering force
        steering = Vector3.ClampMagnitude(steering, maxForce);

        return steering * weight;
    }

    // sets the pursuit target and caches its rigidbody
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        if (target != null)
        {
            targetRb = target.GetComponent<Rigidbody>();
        }
    }

    // returns the currently tracked target transform
    public Transform GetTarget()
    {
        return target;
    }
}

