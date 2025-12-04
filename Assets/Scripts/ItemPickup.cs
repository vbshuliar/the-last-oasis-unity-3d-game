using UnityEngine;

public enum ItemType { GreenPotion, HealthPack, SpeedBoost, DamageBoost }

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
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddItemCollected();
        }

        switch (itemType)
        {
            case ItemType.GreenPotion:
                player.ApplyGreenPotionEffect(sizeMultiplier, speedMultiplier, duration);
                break;
            case ItemType.HealthPack:
                ApplyHealthPack(player);
                break;
            case ItemType.SpeedBoost:
                ApplySpeedBoost(player);
                break;
            case ItemType.DamageBoost:
                ApplyDamageBoost(player);
                break;
        }
    }

    void ApplyHealthPack(PlayerController player)
    {
        Actor playerActor = player.GetComponent<Actor>();
        if (playerActor != null)
        {
            playerActor.Heal(healAmount);
        }
    }

    void ApplySpeedBoost(PlayerController player)
    {
        player.ApplyGreenPotionEffect(1f, speedBoostMultiplier, speedBoostDuration);
    }

    void ApplyDamageBoost(PlayerController player)
    {
        // placeholder can be extended later
        player.ApplyGreenPotionEffect(1f, 1f, damageBoostDuration);
    }

    public float GetSizeMultiplier() => sizeMultiplier;
    public float GetSpeedMultiplier() => speedMultiplier;
    public float GetDuration() => duration;
}
