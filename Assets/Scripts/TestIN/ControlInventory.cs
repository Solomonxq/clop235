using UnityEngine;

public class ControlInventory : MonoBehaviour
{
    // Сюди ми перетягнемо твій об'єкт Inventory
    public GameObject inventoryPanel; 

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            // Цей рядок бере поточний стан (увімкнено чи вимкнено) і робить його протилежним
            inventoryPanel.SetActive(!inventoryPanel.activeSelf);
            
            Debug.Log("Інвентар відкрито/закрито!");
        }
    }
}