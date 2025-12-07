using UnityEngine;

public enum ItemType { GreenPotion, HealthPack, SpeedBoost, DamageBoost, Star }

// handles items that player can pick up like health packs and power ups
public class ItemPickup : MonoBehaviour
{
    [Header("Item Settings")]
    public ItemType itemType;

    [Header("Green Potion Settings")]
    [SerializeField] float sizeMultiplier = 2f;
    [SerializeField] float speedMultiplier = 2f;
    [SerializeField] float duration = 5f;

    [Header("Health Pack Settings")]
    [SerializeField] int healAmount = 20;

    [Header("Speed Boost Settings")]
    [SerializeField] float speedBoostMultiplier = 1.5f;
    [SerializeField] float speedBoostDuration = 10f;

    [Header("Damage Boost Settings")]
    [SerializeField] float damageBoostMultiplier = 2f;
    [SerializeField] float damageBoostDuration = 15f;

    public void ApplyEffect(PlayerController player)
    {
        switch (itemType)
        {
            case ItemType.GreenPotion:
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.AddItemCollected();
                }
                player.ApplyGreenPotionEffect(sizeMultiplier, speedMultiplier, duration);
                break;
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

    public float GetSizeMultiplier() => sizeMultiplier;
    public float GetSpeedMultiplier() => speedMultiplier;
    public float GetDuration() => duration;
}
