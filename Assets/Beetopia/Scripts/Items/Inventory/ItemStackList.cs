using System;
using System.Collections.Generic;
[Serializable]
public class ItemStackList {
    public List<ItemStack> itemStackList;
    
    public ItemStackList() {
        itemStackList = new List<ItemStack>();
    }
    
    public List<ItemStack> GetItemStackList() {
        return itemStackList;
    }
    
    public uint GetItemStoredCount(ItemSO filterItemSO) {
        uint amount = 0;
        foreach (ItemStack itemStack in itemStackList) {
            if (filterItemSO == G.GameAssets.itemSO_Refs.any || filterItemSO == itemStack.itemSO) {
                amount += itemStack.amount;
            }
        }
        return amount;
    }
    
    public bool CanAddItemToItemStack(ItemSO itemSO, int amount = 1) {
        ItemStack itemStack = GetItemStackWithItemType(itemSO);
        if (itemStack != null) {
            // Stack already exists, has space?
            if (itemStack.amount + amount <= itemSO.maxStackAmount) {
                // Can add
                return true;
            } else {
                // Stack full
                return false;
            }
        } else {
            // No item stack exists, can add
            return true;
        }
    }

    public void AddItemToItemStack(ItemSO itemSO, uint amount = 1) {
        ItemStack itemStack = GetItemStackWithItemType(itemSO);
        if (itemStack != null) {
            itemStack.amount += amount;
        } else {
            itemStack = new ItemStack { itemSO = itemSO, amount = amount };
            itemStackList.Add(itemStack);
        }
    }
    
    public void RemoveItemFromItemStack(ItemSO itemSO, uint amount = 1) {
        var itemStack = GetItemStackWithItemType(itemSO);
        if (itemStack == null || itemStack.amount == 0) return;

        itemStack.amount = Math.Max(0, itemStack.amount - amount);

        if (itemStack.amount == 0) {
            itemStackList.Remove(itemStack);
        }
    }
    
    public ItemStack GetItemStackWithItemType(ItemSO itemSO) {
        foreach (ItemStack itemStack in itemStackList) {
            if (itemStack.itemSO == itemSO) {
                return itemStack;
            }
        }
        return null;
    }
    
    public ItemStack GetFirstItemStackWithFilter(ItemSO[] filterItemSO) {
        foreach (ItemSO itemSO in filterItemSO) {
            foreach (ItemStack itemStack in itemStackList) {
                if (itemStack.itemSO == itemSO) {
                    return itemStack;
                }
            }
        }
        return null;
    }
}