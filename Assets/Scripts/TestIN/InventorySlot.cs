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

    public void Clear()
    {
        item = null;
        amount = 0;
    }

    public bool IsEmpty() => item == null || amount <= 0;

    public InventorySlot Clone()
    {
        return new InventorySlot { item = this.item, amount = this.amount };
    }
}