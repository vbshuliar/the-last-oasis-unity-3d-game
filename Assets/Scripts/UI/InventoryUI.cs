using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Transform inventorySlotParent;
    [SerializeField] private GameObject inventorySlotPrefab;

    private List<GameObject> inventorySlots = new List<GameObject>();
    private Dictionary<ItemType, Sprite> itemIcons = new Dictionary<ItemType, Sprite>();

    void Start()
    {
        if (Inventory.Instance != null)
        {
            UpdateInventoryDisplay();
        }
    }

    public void UpdateInventoryDisplay()
    {
        if (Inventory.Instance == null) return;

        // Clear existing slots
        foreach (GameObject slot in inventorySlots)
        {
            if (slot != null)
            {
                Destroy(slot);
            }
        }
        inventorySlots.Clear();

        // Create slots for each item
        List<ItemType> items = Inventory.Instance.GetAllItems();
        for (int i = 0; i < items.Count; i++)
        {
            CreateInventorySlot(items[i], i);
        }
    }

    void CreateInventorySlot(ItemType itemType, int index)
    {
        if (inventorySlotPrefab == null || inventorySlotParent == null) return;

        GameObject slot = Instantiate(inventorySlotPrefab, inventorySlotParent);
        inventorySlots.Add(slot);

        // Set item icon if available
        Image iconImage = slot.GetComponentInChildren<Image>();
        if (iconImage != null && itemIcons.ContainsKey(itemType))
        {
            iconImage.sprite = itemIcons[itemType];
        }

        // Set item name
        TextMeshProUGUI nameText = slot.GetComponentInChildren<TextMeshProUGUI>();
        if (nameText != null)
        {
            nameText.text = itemType.ToString();
        }

        // Set hotkey number
        TextMeshProUGUI keyText = slot.transform.Find("KeyText")?.GetComponent<TextMeshProUGUI>();
        if (keyText != null)
        {
            keyText.text = (index + 1).ToString();
        }
    }

    public void SetItemIcon(ItemType itemType, Sprite icon)
    {
        itemIcons[itemType] = icon;
    }
}

