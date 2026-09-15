using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [Header("Посилання")]
    [SerializeField] private GameObject inventoryPanel;   // Панель Background
    [SerializeField] private Transform slotsContainer;    // SlotsContainer
    [SerializeField] private GameObject slotPrefab;       // Префаб слота z папки Assets

    private List<InventorySlotUI> slotUIs = new List<InventorySlotUI>();
    private bool isOpen = false;

    private void Start()
    {
        InitializeInventoryUI();
        SetInventoryState(false);
    }

    public void ToggleInventory()
    {
        isOpen = !isOpen;
        SetInventoryState(isOpen);
    }

    private void SetInventoryState(bool state)
    {
        isOpen = state;

        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(isOpen);
        }

        if (isOpen)
        {
            RefreshUI();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void InitializeInventoryUI()
    {
        if (slotsContainer == null) return;

        foreach (Transform child in slotsContainer)
        {
            Destroy(child.gameObject);
        }
        slotUIs.Clear();

        if (InventoryManager.Instance == null) return;

        int size = InventoryManager.Instance.inventorySize;
        for (int i = 0; i < size; i++)
        {
            GameObject newSlotObj = Instantiate(slotPrefab, slotsContainer);
            InventorySlotUI slotUI = newSlotObj.GetComponent<InventorySlotUI>();
            slotUIs.Add(slotUI);
        }

        RefreshUI();
    }

    public void RefreshUI()
    {
        if (InventoryManager.Instance == null) return;

        var inventoryData = InventoryManager.Instance.inventory;

        for (int i = 0; i < slotUIs.Count; i++)
        {
            if (i < inventoryData.Count)
            {
                slotUIs[i].UpdateSlotUI(inventoryData[i]);
            }
            else
            {
                slotUIs[i].UpdateSlotUI(null);
            }
        }
    }
}