using System;

public class StorageObject : BasePlacedObject, IItemStorage {
    public event Action<IItemStorage> OnItemStorageCountChanged;

    private ItemStackList itemStackList;
    
    public override void Setup(BasePlaceableSO basePlaceableSO) {
        base.Setup(basePlaceableSO);
        //Debug.Log("Storage.Setup()");
        itemStackList = new ItemStackList();
    }

    public override string ToString() {
        return itemStackList.ToString();
    }

    public ItemStackList GetItemStackList() {
        return itemStackList;
    }

    public uint GetItemStoredCount(ItemSO filterItemSO) {
        return itemStackList.GetItemStoredCount(filterItemSO);
    }

    public bool TryGetStoredItem(ItemSO[] filterItemSO, out ItemSO itemSO) {
        ItemStack itemStack = G.DataManager.GameData.itemStackList.GetFirstItemStackWithFilter(filterItemSO);
        if (itemStack != null && itemStack.amount > 0) {
            itemStack.amount--;
            itemSO = itemStack.itemSO;
            OnItemStorageCountChanged?.Invoke(this);
            return true;
        } else {
            itemSO = null;
            return false;
        }
    }

    public ItemSO[] GetItemSOThatCanStore() {
        return new ItemSO[] { G.GameAssets.itemSO_Refs.any };
    }

    public bool TryStoreItem(ItemSO itemSO) {
        G.DataManager.TryStoreItem(itemSO, 1);
        return false;
    }
}
