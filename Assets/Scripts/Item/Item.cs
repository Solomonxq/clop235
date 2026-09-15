using UnityEngine;
using System;

public class Item : MonoBehaviour
{
    [Header("Налаштування предмету")]
    public string itemID = "Coin"; // Унікальний ідентифікатор для інвентарю (наприклад, "HealthPotion", "GoldCoin")
    [SerializeField] private string playerTag = "Player";

    // Статична подія, яку слухатиме інвентар. Передає ID предмета та кількість/об'єкт
    public static event Action<string> OnItemCollected;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(playerTag))
        {
            // Викликаємо подію підбору (інвентар зловить цей сигнал)
            OnItemCollected?.Invoke(itemID);

            Debug.Log($"Предмет [{itemID}] підібрано гравцем!");

            // Видаляємо предмет зі сцени
            Destroy(gameObject);
        }
    }
}