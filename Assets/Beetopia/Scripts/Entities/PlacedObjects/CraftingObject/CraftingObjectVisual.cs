using UnityEngine;

public class CraftingObjectVisual : MonoBehaviour {
    [SerializeField] private CraftingObject craftingObject;
    [SerializeField] private SelectRecipeCanvas recipeCanvas;
    
    private void Start() {
        recipeCanvas.Setup(craftingObject);
        CraftingObject.OnRecipeCanvasTrigger += OnRecipeCanvasTrigger;
        recipeCanvas.Hide();
    }

    private void OnDestroy() {
        CraftingObject.OnRecipeCanvasTrigger -= OnRecipeCanvasTrigger;
    }

    private void OnRecipeCanvasTrigger(CraftingObject craftingObject)
    {
        if (this.craftingObject == craftingObject)
        {
            if (!recipeCanvas.gameObject.activeSelf)
                recipeCanvas.Show();
            else
                recipeCanvas.Hide();
        }
        else
        {
            recipeCanvas.Hide();
        }
    }
}
