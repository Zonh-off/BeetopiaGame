using System;

public interface IItemStorage {
    event Action<IItemStorage> OnItemStorageCountChanged;
    uint GetItemStoredCount(ItemSO filterItemSO);
    bool TryGetStoredItem(ItemSO[] filterItemSO, out ItemSO itemSO);
    bool TryStoreItem(ItemSO itemSO);
    ItemSO[] GetItemSOThatCanStore();  
}