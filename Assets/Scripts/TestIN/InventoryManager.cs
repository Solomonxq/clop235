using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("Налаштування інвентарю")]
    public int inventorySize = 20; // Скільки всього клітинок буде у твоєму інвентарі
    public List<InventorySlot> inventory = new List<InventorySlot>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Створюємо порожні слоти при запуску
        for (int i = 0; i < inventorySize; i++)
        {
            inventory.Add(new InventorySlot());
        }
    }

    // Метод для додавання предмета в інвентар
    public bool AddItem(ItemData item, int amount)
    {
        // 1. Шукаємо слот, де вже є такий самий предмет (для стакування)
        foreach (var slot in inventory)
        {
            if (!slot.IsEmpty() && slot.item == item)
            {
                slot.amount += amount;
                return true;
            }
        }

        // 2. Якщо такого немає, шукаємо перший порожній слот
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