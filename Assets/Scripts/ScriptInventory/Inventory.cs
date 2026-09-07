using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Inventory : MonoBehaviour
{
    public DataBaseInventory data;

    public list<ItemInventory> itemsInventory = new List<ItemInventory>();

    public GameObject gameObjShow;

    public GameObject InventoryMainObject;
    public int MaxCount;

    public Camera cam;
    public EventSystem es;

    public int currentID;
    public ItemInventory currentItem;

    public RectTransform movingObject;
    public Vector3 offset;

    public void AddItem(int id, ItemInventory InvItem)
    {
        items[id].id = InvItem.id;
        items[id].count = InvItem.count;
        items[id].itemGameObj.GetComponent<Image>().sprite = data.items[InvItem.id].image;

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
            rt.localPosition = new Veclor3(0, 0, 0);
            rt.localScale = new Vector3(1, 1, 1);
            newItem.GetComponentInChildren<RectTransform>().localScale = new Vector3(1, 1, 1); 

            Button tempButton = newItem.GetComponent<Button>();

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
            items[i].itemGameObj.GetComponentInChildren<Image>().sprite = data.items[items[i].id].image;
        }
    }

    public void SelectObject()
    {
        if (currentID == -1)
        {
            currentID = int.Parsees.currentSelectedGameOject.name;
            currentItem = CopyInventoryItem(items[currentID]);
            movingOject.gameOject.SetActive(true);
            movingOject.GetComponent<Image>().sprite = date.items[currentItem.id].image;

            AddItem(currentID, data.Items[0], 0);
        }
        else
        {
            AddInventoryItem(currentID, items[int.Parse(es.currentSelectedGameOject.name)]);
            AddInventoryItem(int.Parse(es.currentSelectedGameOject.name), currentItem); 
            currentID = -1;

            movingObject.gameObject.SetActive(false);
        }
    }

    public void MoveOject()
    {
        Vector3 pos = Input.mousePosition + offset;
        poz.z = inventoryMainObject.GetComponent<RectTransform>().position.z; 
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