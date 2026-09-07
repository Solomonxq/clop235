using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DataBaseInventory : MonoBehaviour
{
    public List<Item> items = new List<Item>();
}


[System.Serializable]

public class Item
{
    public int id;
    public string name;
    public Sprite icon;
}