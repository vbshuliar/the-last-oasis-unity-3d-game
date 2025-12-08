using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// renders the player's inventory into visual slots
public class InventoryUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Transform inventorySlotParent;
    [SerializeField] private GameObject inventorySlotPrefab;

    private List<GameObject> inventorySlots = new List<GameObject>();
    private Dictionary<ItemType, Sprite> itemIcons = new Dictionary<ItemType, Sprite>();

    // refreshes the display once the singleton exists
    void Start()
    {
        if (Inventory.Instance != null)
        {
            UpdateInventoryDisplay();
        }
    }

    // rebuilds the ui slots to match the current contents
    public void UpdateInventoryDisplay()
    {
        if (Inventory.Instance == null) return;

        // clear existing slots
        foreach (GameObject slot in inventorySlots)
        {
            if (slot != null)
            {
                Destroy(slot);
            }
        }
        inventorySlots.Clear();

        // create slots for each item
        List<ItemType> items = Inventory.Instance.GetAllItems();
        for (int i = 0; i < items.Count; i++)
        {
            CreateInventorySlot(items[i], i);
        }
    }

    // instantiates a new slot and fills in its label and icon
    void CreateInventorySlot(ItemType itemType, int index)
    {
        if (inventorySlotPrefab == null || inventorySlotParent == null) return;

        GameObject slot = Instantiate(inventorySlotPrefab, inventorySlotParent);
        inventorySlots.Add(slot);

        // set item icon if available
        Image iconImage = slot.GetComponentInChildren<Image>();
        if (iconImage != null && itemIcons.ContainsKey(itemType))
        {
            iconImage.sprite = itemIcons[itemType];
        }

        // set item name
        TextMeshProUGUI nameText = slot.GetComponentInChildren<TextMeshProUGUI>();
        if (nameText != null)
        {
            nameText.text = itemType.ToString();
        }

        // set hotkey number
        TextMeshProUGUI keyText = slot.transform.Find("KeyText")?.GetComponent<TextMeshProUGUI>();
        if (keyText != null)
        {
            keyText.text = (index + 1).ToString();
        }
    }

    // registers a sprite override for a given item type
    public void SetItemIcon(ItemType itemType, Sprite icon)
    {
        itemIcons[itemType] = icon;
    }
}

