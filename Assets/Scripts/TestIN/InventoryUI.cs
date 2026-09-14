using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [Header("Посилання")]
    [SerializeField] private GameObject inventoryPanel;   // Сама панель інвентарю (щоб вмикати/вимикати)
    [SerializeField] private Transform slotsContainer;    // Контейнер із Grid Layout Group, куди будуть падати слоти
    [SerializeField] private GameObject slotPrefab;       // Твій префаб клітинки (слота)

    private List<ItemSlotUI> slotUIs = new List<ItemSlotUI>();
    private bool isOpen = false;

    private void Start()
    {
        // Створюємо слоти один раз при старті гри на основі даних з менеджера
        InitializeInventoryUI();

        // Одразу закриваємо вікно на старті (або залиш відкритим, якщо хочеш)
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
            isOpen = false;
        }
    }

    private void Update()
    {
        // Натискання клавіші 'E' для відкриття/закриття інвентарю
        if (Input.GetKeyDown(KeyCode.E))
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        isOpen = !isOpen;
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(isOpen);
        }

        if (isOpen)
        {
            RefreshUI();
        }
    }

    private void InitializeInventoryUI()
    {
        // Очищаємо контейнер на випадок, якщо там щось було
        foreach (Transform child in slotsContainer)
        {
            Destroy(child.gameObject);
        }
        slotUIs.Clear();

        // Створюємо стільки слотів, скільки вказано в InventoryManager
        int size = InventoryManager.Instance.inventorySize;
        for (int i = 0; i < size; i++)
        {
            GameObject newSlotObj = Instantiate(slotPrefab, slotsContainer);
            ItemSlotUI slotUI = newSlotObj.GetComponent<ItemSlotUI>();
            slotUIs.Add(slotUI);
        }

        RefreshUI();
    }

    public void RefreshUI()
    {
        var inventoryData = InventoryManager.Instance.inventory;

        for (int i = 0; i < slotUIs.Count; i++)
        {
            if (i < inventoryData.Count)
            {
                slotUIs[i].UpdateSlot(inventoryData[i]);
            }
            else
            {
                slotUIs[i].UpdateSlot(null);
            }
        }
    }
}