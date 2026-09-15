using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [Header("Посилання на UI")]
    [SerializeField] private InventoryUI inventoryUI;

    [Header("Тестові предмети")]
    [SerializeField] private ItemData testItem1;
    [SerializeField] private ItemData testItem2;

    private void Update()
    {
        // 1. Відкриття / Закриття інвентарю на клавішу E
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (inventoryUI != null)
            {
                inventoryUI.ToggleInventory();
            }
        }

        // 2. Тестове додавання предметів на клавіші 1 та 2
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            AddTestItem(testItem1, 1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            AddTestItem(testItem2, 5);
        }
    }

    public void AddTestItem(ItemData item, int amount)
    {
        if (item == null)
        {
            Debug.LogWarning("Предмет не призначено в Inspector!");
            return;
        }

        bool success = InventoryManager.Instance.AddItem(item, amount);
        if (success && inventoryUI != null)
        {
            inventoryUI.RefreshUI();
            Debug.Log($"Додано: {item.itemName} x{amount}");
        }
    }

    // Виклик для кнопки [CLOSE]
    public void OnCloseButton()
    {
        if (inventoryUI != null)
        {
            inventoryUI.ToggleInventory();
        }
    }
}