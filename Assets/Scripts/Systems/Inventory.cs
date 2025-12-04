using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }

    [Header("Inventory Settings")]
    [SerializeField] private int maxInventorySize = 4;

    private List<ItemType> items = new List<ItemType>();

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

    public bool RemoveItem(ItemType itemType)
    {
        if (items.Remove(itemType))
        {
            Debug.Log("Removed " + itemType + " from inventory. Total items: " + items.Count);
            return true;
        }
        return false;
    }

    public bool HasItem(ItemType itemType)
    {
        return items.Contains(itemType);
    }

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

    public List<ItemType> GetAllItems()
    {
        return new List<ItemType>(items);
    }

    public void ClearInventory()
    {
        items.Clear();
    }

    public int GetItemCount()
    {
        return items.Count;
    }

    public bool IsFull()
    {
        return items.Count >= maxInventorySize;
    }
}

