using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Collider helper that blocks only objects with the specified player tag.
/// All other objects get their collisions ignored so they can pass through freely.
/// </summary>
[RequireComponent(typeof(Collider))]
public class PlayerOnlyBarrier : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";

    private Collider barrierCollider;
    private readonly HashSet<Collider> ignoredColliders = new HashSet<Collider>();

    void Awake()
    {
        barrierCollider = GetComponent<Collider>();
        barrierCollider.isTrigger = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        HandleCollision(collision.collider);
    }

    void OnCollisionStay(Collision collision)
    {
        HandleCollision(collision.collider);
    }

    void HandleCollision(Collider other)
    {
        if (other == null || barrierCollider == null)
        {
            return;
        }

        if (other.CompareTag(playerTag))
        {
            return;
        }

        if (ignoredColliders.Contains(other))
        {
            return;
        }

        Physics.IgnoreCollision(barrierCollider, other, true);
        ignoredColliders.Add(other);
    }

    public void SetPlayerTag(string tag)
    {
        if (!string.IsNullOrEmpty(tag))
        {
            playerTag = tag;
        }
    }
}
