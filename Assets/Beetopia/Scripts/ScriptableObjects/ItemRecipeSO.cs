using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Assets/Game Data/Recipes/New Item Recipe", fileName = "NewItemRecipe")]
public class ItemRecipeSO : ScriptableObject {
    public List<RecipeItem> input;
    public List<RecipeItem> output;
    public float craftingTime;
    
    [System.Serializable]
    public struct RecipeItem {
        public ItemSO item;
        public uint amount;
    }
}