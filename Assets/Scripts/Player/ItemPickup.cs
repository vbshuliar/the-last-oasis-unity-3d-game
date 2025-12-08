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
    [SerializeField] int healAmount = 15;

    [Header("Speed Boost Settings")]
    [SerializeField] float speedBoostMultiplier = 1.5f;
    [SerializeField] float speedBoostDuration = 15f;

    [Header("Damage Boost Settings")]
    [SerializeField] float damageBoostMultiplier = 2f;
    [SerializeField] float damageBoostDuration = 15f;

    bool hasBeenCollected = false;

    // sets up the collider when the component is reset in the editor
    void Reset()
    {
        EnsureTriggerCollider();
    }

    // ensures the collider is configured for trigger detection
    void Awake()
    {
        EnsureTriggerCollider();
    }

    // toggles the collider to trigger mode if needed
    void EnsureTriggerCollider()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
        }
    }

    // detects the player entering the trigger and begins collection
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

    // marks the pickup as collected and applies its effect
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

    // applies the configured effect type to the player controller
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
                // star only gives +50 score, doesn't count as regular item collected
                ApplyStar(player);
                break;
            default:
                Debug.LogWarning($"ItemPickup: {itemType} is no longer supported. Use Interactable for this item type.");
                break;
        }
    }

    // restores the player's health toward the configured amount
    void ApplyHealthPack(PlayerController player)
    {
        Actor playerActor = player.GetComponent<Actor>();
        if (playerActor != null)
        {
            // restore full health up to configured cap (default 15)
            int targetHealth = healAmount > 0 ? healAmount : playerActor.maxHealth;
            int amountToHeal = targetHealth - playerActor.currentHealth;
            if (amountToHeal > 0)
            {
                playerActor.Heal(amountToHeal);
            }
        }
    }

    // boosts movement speed temporarily
    void ApplySpeedBoost(PlayerController player)
    {
        player.ApplyGreenPotionEffect(1f, speedBoostMultiplier, speedBoostDuration);
    }

    // increases attack damage for the provided duration
    void ApplyDamageBoost(PlayerController player)
    {
        player.ApplyDamageBoost(damageBoostMultiplier, damageBoostDuration);
    }

    // adds bonus score without counting as a normal pickup
    void ApplyStar(PlayerController player)
    {
        // star gives +50 score
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(50);
        }
    }

}
