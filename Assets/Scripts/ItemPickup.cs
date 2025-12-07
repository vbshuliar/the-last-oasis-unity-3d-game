using UnityEngine;

public enum ItemType { HealthPack, SpeedBoost, DamageBoost, Star }

// handles items that player can pick up like health packs and power ups
[RequireComponent(typeof(Collider))]
public class ItemPickup : MonoBehaviour
{
    [Header("Item Settings")]
    public ItemType itemType;

    [Header("Pickup Settings")]
    [SerializeField] string playerTag = "Player";

    [Header("Health Pack Settings")]
    [SerializeField] int healAmount = 3;

    [Header("Speed Boost Settings")]
    [SerializeField] float speedBoostMultiplier = 1.5f;
    [SerializeField] float speedBoostDuration = 15f;

    [Header("Damage Boost Settings")]
    [SerializeField] float damageBoostMultiplier = 2f;
    [SerializeField] float damageBoostDuration = 15f;

    bool hasBeenCollected = false;

    void Reset()
    {
        EnsureTriggerCollider();
    }

    void Awake()
    {
        EnsureTriggerCollider();
    }

    void EnsureTriggerCollider()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasBeenCollected)
        {
            return;
        }

        if (!other.CompareTag(playerTag))
        {
            return;
        }

        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null)
        {
            player = other.GetComponentInParent<PlayerController>();
        }

        if (player == null)
        {
            return;
        }

        Collect(player);
    }

    void Collect(PlayerController player)
    {
        hasBeenCollected = true;
        ApplyEffect(player);

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayPickupSoundForItem(itemType);
        }

        Destroy(gameObject);
    }

    public void ApplyEffect(PlayerController player)
    {
        switch (itemType)
        {
            case ItemType.HealthPack:
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.AddItemCollected();
                }
                ApplyHealthPack(player);
                break;
            case ItemType.SpeedBoost:
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.AddItemCollected();
                }
                ApplySpeedBoost(player);
                break;
            case ItemType.DamageBoost:
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.AddItemCollected();
                }
                ApplyDamageBoost(player);
                break;
            case ItemType.Star:
                // Star only gives +50 score, doesn't count as regular item collected
                ApplyStar(player);
                break;
            default:
                Debug.LogWarning($"ItemPickup: {itemType} is no longer supported. Use Interactable for this item type.");
                break;
        }
    }

    void ApplyHealthPack(PlayerController player)
    {
        Actor playerActor = player.GetComponent<Actor>();
        if (playerActor != null)
        {
            // Restore 20% of maximum health
            int healAmount = Mathf.RoundToInt(playerActor.maxHealth * 0.2f);
            playerActor.Heal(healAmount);
        }
    }

    void ApplySpeedBoost(PlayerController player)
    {
        player.ApplyGreenPotionEffect(1f, speedBoostMultiplier, speedBoostDuration);
    }

    void ApplyDamageBoost(PlayerController player)
    {
        player.ApplyDamageBoost(damageBoostMultiplier, damageBoostDuration);
    }

    void ApplyStar(PlayerController player)
    {
        // Star gives +50 score
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(50);
        }
    }

}
