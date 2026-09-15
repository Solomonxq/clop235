using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [Header("Налаштування")]
    public int inventorySize = 20;
    public List<InventorySlot> inventory = new List<InventorySlot>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        InitializeInventory();
    }

    private void InitializeInventory()
    {
        inventory.Clear();
        for (int i = 0; i < inventorySize; i++)
        {
            inventory.Add(new InventorySlot());
        }
    }

    public bool AddItem(ItemData item, int amount = 1)
    {
        foreach (var slot in inventory)
        {
            if (!slot.IsEmpty() && slot.item == item && slot.amount < item.maxStackSize)
            {
                slot.amount += amount;
                return true;
            }
        }

        foreach (var slot in inventory)
        {
            if (slot.IsEmpty())
            {
                slot.item = item;
                slot.amount = amount;
                return true;
            }
        }

        Debug.Log("Інвентар повний!");
        return false;
    }
}