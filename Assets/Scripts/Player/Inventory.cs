using System.Collections.Generic;
using UnityEngine;

// maintains the player's collected items between scenes
public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }

    [Header("Inventory Settings")]
    [SerializeField] private int maxInventorySize = 4;

    private List<ItemType> items = new List<ItemType>();

    // implements a simple singleton so the inventory persists
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // tries to add an item and returns false if full
    public bool AddItem(ItemType itemType)
    {
        if (items.Count >= maxInventorySize)
        {
            Debug.Log("Inventory is full!");
            return false;
        }

        items.Add(itemType);
        Debug.Log("Added " + itemType + " to inventory. Total items: " + items.Count);
        return true;
    }

    // removes one instance of the supplied item type
    public bool RemoveItem(ItemType itemType)
    {
        if (items.Remove(itemType))
        {
            Debug.Log("Removed " + itemType + " from inventory. Total items: " + items.Count);
            return true;
        }
        return false;
    }

    // reports whether the inventory currently holds the item
    public bool HasItem(ItemType itemType)
    {
        return items.Contains(itemType);
    }

    // counts how many of a specific item type exist
    public int GetItemCount(ItemType itemType)
    {
        int count = 0;
        foreach (ItemType item in items)
        {
            if (item == itemType)
            {
                count++;
            }
        }
        return count;
    }

    // returns a copy of the item list for display
    public List<ItemType> GetAllItems()
    {
        return new List<ItemType>(items);
    }

    // removes every stored item
    public void ClearInventory()
    {
        items.Clear();
    }

    // returns total number of stored items
    public int GetItemCount()
    {
        return items.Count;
    }

    // checks whether the inventory reached its size cap
    public bool IsFull()
    {
        return items.Count >= maxInventorySize;
    }
}

