using System;

[Serializable]
public class InventorySlot
{
    public ItemData item;
    public int amount;

    public InventorySlot()
    {
        Clear();
    }

    public InventorySlot(ItemData newItem, int newAmount)
    {
        item = newItem;
        amount = newAmount;
    }

    public void Clear()
    {
        item = null;
        amount = 0;
    }

    public bool IsEmpty() => item == null || amount <= 0;
}