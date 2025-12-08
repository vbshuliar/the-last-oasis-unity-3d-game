using UnityEngine;

// base behaviour that provides shared steering parameters
public abstract class SteeringBehaviour : MonoBehaviour
{
    [Header("Steering Settings")]
    [SerializeField] protected float weight = 1f;
    [SerializeField] protected float maxSpeed = 5f;
    [SerializeField] protected float maxForce = 10f;

    // derived classes return their steering force in this method
    public abstract Vector3 CalculateForce();

    // exposes the weight so other systems can balance forces
    public float GetWeight()
    {
        return weight;
    }

    // lets callers adjust how strongly this force influences motion
    public void SetWeight(float newWeight)
    {
        weight = newWeight;
    }
}

