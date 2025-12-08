using UnityEngine;

public abstract class SteeringBehaviour : MonoBehaviour
{
    [Header("Steering Settings")]
    [SerializeField] protected float weight = 1f;
    [SerializeField] protected float maxSpeed = 5f;
    [SerializeField] protected float maxForce = 10f;

    public abstract Vector3 CalculateForce();

    public float GetWeight()
    {
        return weight;
    }

    public void SetWeight(float newWeight)
    {
        weight = newWeight;
    }
}

