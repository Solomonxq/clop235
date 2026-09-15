using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlotUI : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private Button button;

    private InventorySlot currentSlotData;

    public void UpdateSlotUI(InventorySlot slotData)
    {
        currentSlotData = slotData;

        if (currentSlotData == null || currentSlotData.IsEmpty())
        {
            ClearUI();
            return;
        }

        if (icon != null)
        {
            icon.sprite = currentSlotData.item.icon;
            icon.enabled = true;
        }

        if (amountText != null)
        {
            if (currentSlotData.amount > 1)
            {
                amountText.text = currentSlotData.amount.ToString();
                amountText.enabled = true;
            }
            else
            {
                amountText.enabled = false;
            }
        }

        if (button != null) button.interactable = true;
    }

    public void ClearUI()
    {
        if (icon != null) icon.enabled = false;
        if (amountText != null) amountText.enabled = false;
        if (button != null) button.interactable = false;
    }

    public void OnSlotClick()
    {
        if (currentSlotData != null && !currentSlotData.IsEmpty())
        {
            Debug.Log($"Вибрано предмет: {currentSlotData.item.itemName} x{currentSlotData.amount}");
        }
    }
}