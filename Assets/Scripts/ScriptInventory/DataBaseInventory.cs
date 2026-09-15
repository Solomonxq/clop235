using UnityEngine;
using System.Collections.Generic;

public class DataBaseInventory : MonoBehaviour
{
    public List<Itemm> items = new List<Itemm>();
}

[System.Serializable]
public class Itemm
{
    public int id;
    public string name;
    public Sprite img; // Змінили з icon на img, щоб відповідати твойму прикладу
}