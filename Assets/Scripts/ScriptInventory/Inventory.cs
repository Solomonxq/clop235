using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    public DataBaseInventory data;
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

    public GameObject background;

    public void Start()
    {
        if (items.Count == 0)
        {
            AddGraphics();
        }

        // Тестове заповнення інвентарю
        for (int i = 0; i < MaxCount; i++) 
        {
            ItemInventory tempInv = new ItemInventory();
            tempInv.id = Random.Range(0, data.items.Count);
            tempInv.count = Random.Range(1, 22);
            AddItem(i, tempInv);
        } 

        UpdateInventory();
    }

    public void Update()
    {
        if (currentID != -1)
        {
            MoveObject();
        }

        // Виправлено input та GetKey умову
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (background != null)
            {
                background.SetActive(!background.activeSelf);
                if (background.activeSelf)
                {
                    UpdateInventory();
                }
            }
        }
    }

    public void AddItem(int id, ItemInventory InvItem)
    {
        items[id].id = InvItem.id;
        items[id].count = InvItem.count;
        
        // Встановлюємо спрайт з бази даних
        Image itemImage = items[id].itemGameObj.GetComponent<Image>();
        if (InvItem.id < data.items.Count)
        {
            itemImage.sprite = data.items[InvItem.id].img;
            itemImage.color = Color.white; 
        }

        // Шукаємо компонент тексту всередині комірки
        Text countText = items[id].itemGameObj.GetComponentInChildren<Text>();
        if (countText != null)
        {
            if (InvItem.count > 1 && InvItem.id != 0)
            {
                countText.text = InvItem.count.ToString();
            }
            else
            {
                countText.text = ""; 
            }
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
            rt.localScale = new Vector3(1, 1, 1);

            Button tempButton = newItem.GetComponent<Button>();
            if (tempButton != null)
            {
                tempButton.onClick.AddListener(delegate { SelectObject(); });
            }

            items.Add(ii);
        } 
    }

    public void UpdateInventory()
    {
        for (int i = 0; i < MaxCount; i++)
        {
            AddItem(i, items[i]);
        }
    }

    public void SelectObject()
    {
        if (es == null) es = EventSystem.current;
        if (es == null || es.currentSelectedGameObject == null) return;

        if (currentID == -1)
        {
            currentID = int.Parse(es.currentSelectedGameObject.name);
            currentItem = CopyInventoryItem(items[currentID]);
            
            if (movingObject != null)
            {
                movingObject.gameObject.SetActive(true);
                Image moveImg = movingObject.GetComponent<Image>();
                if (moveImg != null && currentItem.id < data.items.Count)
                {
                    moveImg.sprite = data.items[currentItem.id].img;
                    moveImg.color = Color.white;
                }
            }

            // Очищаємо вибраний слот (ставимо ID 0 - Empty)
            ItemInventory emptyItem = new ItemInventory { id = 0, count = 0, itemGameObj = items[currentID].itemGameObj };
            AddItem(currentID, emptyItem);
        }
        else
        {
            int targetID = int.Parse(es.currentSelectedGameObject.name);
            ItemInventory targetItem = items[targetID];

            // Якщо клікнули на той самий тип предметів і це не порожній слот
            if (currentItem.id == targetItem.id && currentItem.id != 0)
            {
                int totalCount = targetItem.count + currentItem.count;
                if (totalCount <= 64)
                {
                    targetItem.count = totalCount;
                    AddItem(targetID, targetItem);
                    currentID = -1;
                }
                else
                {
                    currentItem.count = totalCount - 64;
                    targetItem.count = 64;
                    AddItem(targetID, targetItem);
                    // Залишаємо перетягування для залишку предметів
                }
            }
            else
            {
                // Міняємо місцями або ставимо в новий слот
                ItemInventory targetItemCopy = CopyInventoryItem(targetItem);
                AddItem(targetID, currentItem);
                AddItem(currentID, targetItemCopy);

                currentID = -1;
            }

            if (currentID == -1 && movingObject != null)
            {
                movingObject.gameObject.SetActive(false);
            }
        }
    }

    public void MoveObject()
    {
        if (movingObject == null || cam == null) return;

        Vector3 pos = Input.mousePosition + offset;
        if (InventoryMainObject != null)
        {
            pos.z = InventoryMainObject.GetComponent<RectTransform>().position.z;
        }
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