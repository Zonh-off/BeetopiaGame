using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SaveManager))]
public class DataManager : MonoBehaviour, IProviderHandler {
    
    public static event Action OnItemsUpdate;
    public static event Action OnBeesUpdate;
    public static event Action OnStatsUpdate;
    
    [SerializeField] private GameData gameData;
    
    public GameData GameData { set => gameData = value; get => gameData; }

    private SaveManager _saveManager;
    private bool _isGameDataInitialized;
    private GridDatabase<BasePlaceableSO> _gridDatabase = new();
    
    public GridDatabase<BasePlaceableSO> GridDatabase {
        get => _gridDatabase;
        set => _gridDatabase = value;
    }

    public IEnumerator InitializeData() {
        //_saveManager.LoadGame();
        _saveManager = GetComponent<SaveManager>();

        gameData.beeList = new();
        
        _isGameDataInitialized = true;
        
        OnStatsUpdate?.Invoke();

        yield return true;
    }
    
#region Item Handling
    public bool CheckStoredItem(ItemSO[] filter, uint requiredAmount) {
        var stack = gameData.itemStackList.GetFirstItemStackWithFilter(filter);
        return stack != null && stack.amount >= requiredAmount;
    }
    
    public bool TryGetStoredItem(ItemSO[] filter, uint amount, out ItemStack result) {
        var sourceStack = gameData.itemStackList.GetFirstItemStackWithFilter(filter);
        if (sourceStack != null && sourceStack.amount >= amount) {
            sourceStack.amount -= amount;
            result = new ItemStack { itemSO = sourceStack.itemSO, amount = amount };
            return true;
        }

        result = null;
        return false;
    }
    
    public bool TryStoreItem(ItemSO itemSO, uint amount) {
        if (!gameData.itemStackList.CanAddItemToItemStack(itemSO)) return false;

        gameData.itemStackList.AddItemToItemStack(itemSO, amount);
        OnItemsUpdate?.Invoke();
        return true;
    }
#endregion

#region BeeUnitBehaviour Handling
    public bool TryStoreUnit(BeeUnitSO beeUnitSo) {
        if (beeUnitSo == null) return false;

        gameData.beeList.Add(beeUnitSo);
        OnBeesUpdate?.Invoke();
        return true;
    }
#endregion  
    
#region Stats / Economy
    public void AddCoins(uint amount) {
        gameData.coins += amount;
        OnItemsUpdate?.Invoke();
        OnStatsUpdate?.Invoke();
    }
    
    public void RemoveCoins(uint amount) {
        gameData.coins = gameData.coins > amount ? gameData.coins - amount : 0;
        OnStatsUpdate?.Invoke();
    }
    
    public bool CanAfford(uint price) => gameData.coins >= price;
#endregion
}