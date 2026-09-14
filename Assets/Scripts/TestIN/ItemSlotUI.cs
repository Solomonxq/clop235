using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlotUI : MonoBehaviour
{
    [SerializeField] private Image itemIcon;          // Посилання на картинку предмета всередині слота
    [SerializeField] private TextMeshProUGUI amountText; // Посилання на текст кількості

    public void UpdateSlot(InventorySlot slot)
    {
        if (slot == null || slot.IsEmpty())
        {
            // Якщо слот порожній — ховаємо іконку та текст
            if (itemIcon != null)
            {
                itemIcon.sprite = null;
                itemIcon.gameObject.SetActive(false);
            }
            if (amountText != null)
            {
                amountText.text = string.Empty;
            }
        }
        else
        {
            // Якщо в слоті є предмет — показуємо його іконку та кількість
            if (itemIcon != null)
            {
                itemIcon.gameObject.SetActive(true);
                itemIcon.sprite = slot.item.icon; // Замініть .icon на назву вашого поля спрайта в ItemData, якщо воно відрізняється
            }

            if (amountText != null)
            {
                amountText.text = slot.amount > 1 ? slot.amount.ToString() : string.Empty;
            }
        }
    }
}