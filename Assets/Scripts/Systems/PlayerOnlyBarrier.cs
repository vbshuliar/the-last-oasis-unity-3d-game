using System.Collections.Generic;
using UnityEngine;

// collider helper that blocks only objects with the specified player tag
[RequireComponent(typeof(Collider))]
public class PlayerOnlyBarrier : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";

    private Collider barrierCollider;
    private readonly HashSet<Collider> ignoredColliders = new HashSet<Collider>();

    // caches the barrier collider and ensures it is solid
    void Awake()
    {
        barrierCollider = GetComponent<Collider>();
        barrierCollider.isTrigger = false;
    }

    // processes incoming collisions for non player objects
    void OnCollisionEnter(Collision collision)
    {
        HandleCollision(collision.collider);
    }

    // continually ignores collisions for objects that stay in contact
    void OnCollisionStay(Collision collision)
    {
        HandleCollision(collision.collider);
    }

    // ignores collisions for anything that is not tagged as the player
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

    // updates which tag counts as the player for blocking
    public void SetPlayerTag(string tag)
    {
        if (!string.IsNullOrEmpty(tag))
        {
            playerTag = tag;
        }
    }
}
