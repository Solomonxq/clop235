using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    public DataBaseInventory data;

    // Використовуємо єдину назву списку — items
    public List<ItemInventory> items = new List<ItemInventory>();

    public GameObject gameObjShow;

    public GameObject InventoryMainObject;
    public int MaxCount;

    public Camera cam;
    public EventSystem es;

    public int currentID = -1;
    public ItemInventory currentItem;

    public RectTransform movingObject;
    public Vector3 offset;

    public void AddItem(int id, ItemInventory InvItem)
    {
        items[id].id = InvItem.id;
        items[id].count = InvItem.count;
        // Виправляємо .image на .icon відповідно до твого класу Item
        items[id].itemGameObj.GetComponent<Image>().sprite = data.items[InvItem.id].icon;

        if (InvItem.count > 1 && InvItem.id != 0)
        {
            items[id].itemGameObj.GetComponentInChildren<Text>().text = InvItem.count.ToString();
        }
        else
        {
            items[id].itemGameObj.GetComponentInChildren<Text>().text = "";
        }
    }   

    public void AddGraphics()
    {
        for (int i = 0; i < MaxCount; i++)
        {
            GameObject newItem = Instantiate(gameObjShow, InventoryMainObject.transform) as GameObject;
            newItem.name = i.ToString();
            ItemInventory ii = new ItemInventory();
            ii.itemGameObj = newItem;

            RectTransform rt = newItem.GetComponent<RectTransform>();
            rt.localPosition = new Vector3(0, 0, 0); // Виправлено Veclor3 на Vector3
            rt.localScale = new Vector3(1, 1, 1);
            newItem.GetComponentInChildren<RectTransform>().localScale = new Vector3(1, 1, 1); 

            Button tempButton = newItem.GetComponent<Button>();

            tempButton.onClick.AddListener(delegate { SelectObject(); });

            items.Add(ii);
        } 
    }

    public void UpdateInventory()
    {
        for (int i = 0; i < MaxCount; i++)
        {
            if (items[i].id != 0 && items[i].count > 1)
            {
                items[i].itemGameObj.GetComponentInChildren<Text>().text = items[i].count.ToString();
            }
            else
            {
                items[i].itemGameObj.GetComponentInChildren<Text>().text = "";
            }
            items[i].itemGameObj.GetComponentInChildren<Image>().sprite = data.items[items[i].id].icon; // .icon замість .image
        }
    }

    public void SelectObject()
    {
        if (currentID == -1)
        {
            // Виправлено синтаксис отримання вибраного об'єкта через EventSystem
            currentID = int.Parse(es.currentSelectedGameObject.name);
            currentItem = CopyInventoryItem(items[currentID]);
            movingObject.gameObject.SetActive(true); // Виправлено gameOject на gameObject
            movingObject.GetComponent<Image>().sprite = data.items[currentItem.id].icon; // Виправлено date на data та image на icon

            // Очищаємо слот, ставимо нульовий елемент
            ItemInventory emptyItem = new ItemInventory { id = 0, count = 0 };
            AddItem(currentID, emptyItem);
        }
        else
        {
            AddItem(currentID, items[int.Parse(es.currentSelectedGameObject.name)]);
            AddItem(int.Parse(es.currentSelectedGameObject.name), currentItem); 
            currentID = -1;

            movingObject.gameObject.SetActive(false);
        }
    }

    public void MoveObject()
    {
        Vector3 pos = Input.mousePosition + offset;
        pos.z = InventoryMainObject.GetComponent<RectTransform>().position.z; // Виправлено poz на pos та регістр InventoryMainObject
        movingObject.position = cam.ScreenToWorldPoint(pos);
    }

    public ItemInventory CopyInventoryItem(ItemInventory old)
    {
        ItemInventory New = new ItemInventory();
        New.id = old.id;
        New.itemGameObj = old.itemGameObj;
        New.count = old.count;

        return New;
    }
}

[System.Serializable]
public class ItemInventory
{
    public int id;
    public GameObject itemGameObj;
    public int count;
}