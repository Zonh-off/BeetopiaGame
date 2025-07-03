using System;
using System.Collections.Generic;
using UnityEngine;

public class CraftingObject : BasePlacedObject, IInteractable, IItemStorage, IRecipeStorage {
    public event Action<CraftingObject> OnNectarProcessorVisualChanged;
    public event Action<IItemStorage> OnItemStorageCountChanged;
    public static event Action<CraftingObject> OnRecipeCanvasTrigger;

    [SerializeField] private List<ItemRecipeSO> itemRecipeScriptableObjectList;

    private ItemRecipeSO itemRecipeSO;
    private ItemStackList inputItemStackList;
    private ItemStackList outputItemStackList;
    private float craftingProgress;

    public override void Setup(BasePlaceableSO basePlaceableSO) {
        base.Setup(basePlaceableSO);

        OnItemStorageCountChanged?.Invoke(this);
        OnNectarProcessorVisualChanged?.Invoke(this);

        inputItemStackList = new ItemStackList();
        outputItemStackList = new ItemStackList();

        itemRecipeSO = null;
    }

    public void OnInteract() {
        OnRecipeCanvasTrigger?.Invoke(this);
        PlayPunchAnim();
    }

    public void OnHoldInteract() { }

    public override string ToString() {
        string str = "";
        str += inputItemStackList.ToString();
        str += outputItemStackList.ToString();
        return str;
    }

    private void Update() {
        if (!HasItemRecipe()) return;

        if (HasEnoughItemsToCraft()) {
            craftingProgress += Time.deltaTime;

            if (craftingProgress >= itemRecipeSO.craftingTime) {
                // Item crafting complete
                craftingProgress = 0f;

                // Add Crafted Output Items
                foreach (ItemRecipeSO.RecipeItem recipeItem in itemRecipeSO.output) {
                    outputItemStackList.AddItemToItemStack(recipeItem.item, recipeItem.amount);
                    if (recipeItem.amount > 0) {
                        var storageObject = FindFirstObjectByType<StorageObject>();
                        G.TaskManager.AddTask(new DeliverItemTask(this, storageObject, recipeItem.item,
                            recipeItem.amount));
                    }
                }

                // Consume Input Items
                foreach (ItemRecipeSO.RecipeItem recipeItem in itemRecipeSO.input) {
                    ItemStack itemStack = inputItemStackList.GetItemStackWithItemType(recipeItem.item);
                    itemStack.amount -= recipeItem.amount;
                }

                OnItemStorageCountChanged?.Invoke(this);
                //TODO: TriggerGridObjectChanged();
            }
        }
    }

    public float GetCraftingProgressNormalized() {
        if (HasItemRecipe()) {
            return craftingProgress / itemRecipeSO.craftingTime;
        }
        else {
            return 0f;
        }
    }

    public uint GetItemStoredCount(ItemSO filterItemSO) {
        uint amount = 0;

        amount += outputItemStackList.GetItemStoredCount(filterItemSO);
        amount += inputItemStackList.GetItemStoredCount(filterItemSO);

        return amount;
    }

    public bool TryGetStoredItem(ItemSO[] filterItemSO, out ItemSO itemSO) {
        
        if (!HasItemRecipe()) {
            itemSO = null;
            return false;
        }

        if (ItemSO.IsItemSOInFilter(G.GameAssets.itemSO_Refs.any, filterItemSO) ||
            ItemSO.IsItemSOInFilter(itemRecipeSO.output[0].item, filterItemSO)) {
            // If filter matches any or filter matches this itemType
            ItemStack itemStack = outputItemStackList.GetItemStackWithItemType(itemRecipeSO.output[0].item);
            if (itemStack != null) {
                if (itemStack.amount > 0) {
                    itemStack.amount -= 1;
                    itemSO = itemStack.itemSO;
                    OnItemStorageCountChanged?.Invoke(this);
                    OnNectarProcessorVisualChanged?.Invoke(this);
                    return true;
                }
                else {
                    itemSO = null;
                    return false;
                }
            }
            else {
                itemSO = null;
                return false;
            }
        }
        else {
            itemSO = null;
            return false;
        }
    }


    public ItemSO[] GetItemSOThatCanStore() {
        if (!HasItemRecipe()) return new ItemSO[] { G.GameAssets.itemSO_Refs.none };

        List<ItemSO> canStoreItemSOList = new List<ItemSO>();
        foreach (ItemRecipeSO.RecipeItem recipeItem in itemRecipeSO.input) {
            canStoreItemSOList.Add(recipeItem.item);
        }

        return canStoreItemSOList.ToArray();
    }
    
    public bool TryStoreItem(ItemSO itemSO) {
        if (!HasItemRecipe()) {
            Debug.LogWarning("CraftingObject: No recipe set.");
            return false;
        }

        foreach (var recipeItem in itemRecipeSO.input) {
            if (itemSO == recipeItem.item) {
                if (inputItemStackList.CanAddItemToItemStack(itemSO)) {
                    inputItemStackList.AddItemToItemStack(itemSO);
                    OnItemStorageCountChanged?.Invoke(this);
                    OnNectarProcessorVisualChanged?.Invoke(this);
                    Debug.Log($"CraftingObject: Added item {itemSO.name}");
                    return true;
                } else {
                    Debug.LogWarning($"CraftingObject: Cannot stack more of {itemSO.name}");
                    return false;
                }
            }
        }

        Debug.LogWarning($"CraftingObject: Item {itemSO.name} not part of required input.");
        return false;
    }
    
    /*public bool TryStoreItem(ItemSO itemSO) {
        if (!HasItemRecipe()) return false;

        foreach (ItemRecipeSO.RecipeItem recipeItem in itemRecipeSO.input) {
            if (itemSO == recipeItem.item) {
                // Can add item to input stack?
                if (inputItemStackList.CanAddItemToItemStack(itemSO)) {
                    inputItemStackList.AddItemToItemStack(itemSO);
                    OnItemStorageCountChanged?.Invoke(this);
                    // TODO: TriggerGridObjectChanged();
                    return true;
                }
                else {
                    // It's this item but cannot fit in stack
                    return false;
                }
            }
        }

        return false;
    }*/

    private bool HasEnoughItemsToCraft() {
        if (!HasItemRecipe()) return false;

        foreach (ItemRecipeSO.RecipeItem recipeItem in itemRecipeSO.input) {
            ItemStack itemStack = inputItemStackList.GetItemStackWithItemType(recipeItem.item);
            if (itemStack == null) {
                // There's no item stack with this item type
                return false;
            }
            else {
                if (itemStack.amount < recipeItem.amount) {
                    // Not enough amount of this item type
                    return false;
                }
            }
        }

        // Everything is here, ready to craft
        return true;
    }

    public bool HasItemRecipe() {
        return itemRecipeSO != null;
    }

    public ItemRecipeSO GetItemRecipeSO() {
        return itemRecipeSO;
    }

    public void SetItemRecipeScriptableObject(ItemRecipeSO itemRecipeSO) {
        this.itemRecipeSO = itemRecipeSO;

        foreach (var item in itemRecipeSO.input) {
            if (G.DataManager.CheckStoredItem(new ItemSO[] { item.item }, item.amount)) {
                var storageObject = FindFirstObjectByType<StorageObject>();
                G.TaskManager.AddTask(new DeliverItemTask(storageObject, this, item.item, item.amount));
            }
        }
    }

    public List<ItemRecipeSO> GetItemRecipeScriptableObjectList() {
        return itemRecipeScriptableObjectList;
    }
}