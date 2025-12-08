using UnityEngine;

public enum InteractableType { Enemy, Item }

// allows enemies or pickups to expose interaction logic to the player
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

    private ItemPickup itemPickupComponent; // for backward compatibility

    // caches actor references and optional legacy pickup scripts
    void Awake()
    {
        if (interactionType == InteractableType.Enemy)
        {
            myActor = GetComponent<Actor>();
        }

        // check if itempickup component exists for backward compatibility
        itemPickupComponent = GetComponent<ItemPickup>();
    }

    // triggers item or legacy pickup logic when the player interacts
    public void InteractWithItem(PlayerController player)
    {
        // if itempickup component exists, use it (backward compatibility)
        if (itemPickupComponent != null)
        {
            itemPickupComponent.ApplyEffect(player);
            Destroy(gameObject);
            return;
        }

        // otherwise, handle item effects directly based on itemtype
        if (interactionType == InteractableType.Item)
        {
            ApplyItemEffect(player);
            Destroy(gameObject);
        }
    }

    // applies the configured power up effect and updates stats
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
                // star only gives +50 score, doesn't count as regular item collected
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.AddScore(50);
                }
                break;
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayPickupSoundForItem(itemType);
        }
    }

    // heals the player actor up to their max health
    void ApplyHealthPack(PlayerController player)
    {
        Actor playerActor = player.GetComponent<Actor>();
        if (playerActor != null)
        {
            int targetHealth = playerActor.maxHealth;
            int amountToHeal = targetHealth - playerActor.currentHealth;
            if (amountToHeal > 0)
            {
                playerActor.Heal(amountToHeal);
            }
        }
    }

    // validate that item settings only show when interaction type is item
    void OnValidate()
    {
        // this ensures the inspector updates when interactiontype changes
    }
}
