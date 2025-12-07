using UnityEngine;

public enum InteractableType { Enemy, Item }

public class Interactable : MonoBehaviour
{
    public Actor myActor { get; private set; }

    [Header("Interaction Settings")]
    public InteractableType interactionType;

    [Header("Item Settings")]
    [ConditionalHide("interactionType")]
    [SerializeField] private ItemType itemType = ItemType.HealthPack;

    [Header("Item Effect Settings")]
    [ConditionalHide("interactionType")]
    [SerializeField] private float sizeMultiplier = 2f;
    [ConditionalHide("interactionType")]
    [SerializeField] private float speedMultiplier = 2f;
    [ConditionalHide("interactionType")]
    [SerializeField] private float duration = 5f;
    [ConditionalHide("interactionType")]
    [SerializeField] private float speedBoostMultiplier = 2f;
    [ConditionalHide("interactionType")]
    [SerializeField] private float speedBoostDuration = 10f;
    [ConditionalHide("interactionType")]
    [SerializeField] private float damageBoostMultiplier = 2f;
    [ConditionalHide("interactionType")]
    [SerializeField] private float damageBoostDuration = 10f;

    private ItemPickup itemPickupComponent; // For backward compatibility

    void Awake()
    {
        if (interactionType == InteractableType.Enemy)
        {
            myActor = GetComponent<Actor>();
        }

        // Check if ItemPickup component exists for backward compatibility
        itemPickupComponent = GetComponent<ItemPickup>();
    }

    public void InteractWithItem(PlayerController player)
    {
        // If ItemPickup component exists, use it (backward compatibility)
        if (itemPickupComponent != null)
        {
            itemPickupComponent.ApplyEffect(player);
            Destroy(gameObject);
            return;
        }

        // Otherwise, handle item effects directly based on itemType
        if (interactionType == InteractableType.Item)
        {
            ApplyItemEffect(player);
            Destroy(gameObject);
        }
    }

    void ApplyItemEffect(PlayerController player)
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
                player.ApplyGreenPotionEffect(1f, speedBoostMultiplier, speedBoostDuration);
                break;

            case ItemType.DamageBoost:
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.AddItemCollected();
                }
                player.ApplyDamageBoost(damageBoostMultiplier, damageBoostDuration);
                break;

            case ItemType.Star:
                // Star only gives +50 score, doesn't count as regular item collected
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.AddScore(50);
                }
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

    // Validate that item settings only show when interaction type is Item
    void OnValidate()
    {
        // This ensures the inspector updates when interactionType changes
    }
}
