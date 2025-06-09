using System.Collections.Generic;

public interface IRecipeStorage {
    bool HasItemRecipe();
    ItemRecipeSO GetItemRecipeSO();
    void SetItemRecipeScriptableObject(ItemRecipeSO itemRecipeSO);
    List<ItemRecipeSO> GetItemRecipeScriptableObjectList();
}