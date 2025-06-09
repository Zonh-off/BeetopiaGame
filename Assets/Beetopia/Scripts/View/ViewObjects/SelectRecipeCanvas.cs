using System.Collections.Generic;
using CodeMonkey.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectRecipeCanvas : MonoBehaviour {
    
    [SerializeField] private BasePlacedObject basePlacedObject;
    
    private List<ItemRecipeSO> itemRecipeScriptableObjectList = new();
    private ItemRecipeSO selectedRecipe;
    private int selectedIndex = 0;
    
    public void Setup(BasePlacedObject basePlacedObject)
    {
        this.basePlacedObject = basePlacedObject;
        Init();
    }

    private void Init() {
        if(basePlacedObject == null) return;
        
        selectedIndex = 0;
        
        itemRecipeScriptableObjectList =
            basePlacedObject.GetComponent<IRecipeStorage>().GetItemRecipeScriptableObjectList();
        
        selectedRecipe = itemRecipeScriptableObjectList[selectedIndex];

        if (itemRecipeScriptableObjectList.Count > 1) {
            transform.Find("RightSelectBtn").gameObject.SetActive(true);
            transform.Find("LeftSelectBtn").gameObject.SetActive(true);

            transform.Find("RightSelectBtn").GetComponent<Button_UI>().ClickFunc = () => {
                selectedIndex = (selectedIndex + 1) % itemRecipeScriptableObjectList.Count;
                selectedRecipe = itemRecipeScriptableObjectList[selectedIndex];
                UpdateInputs();
                UpdateOutputs();
            };
            transform.Find("LeftSelectBtn").GetComponent<Button_UI>().ClickFunc = () => {
                selectedIndex = (selectedIndex - 1 + itemRecipeScriptableObjectList.Count) % itemRecipeScriptableObjectList.Count;
                selectedRecipe = itemRecipeScriptableObjectList[selectedIndex];
                UpdateInputs();
                UpdateOutputs();
            };
        }
        else {
            transform.Find("RightSelectBtn").gameObject.SetActive(false);
            transform.Find("LeftSelectBtn").gameObject.SetActive(false);
        }
        
        transform.Find("SelectBtn").GetComponent<Button_UI>().ClickFunc = () => {
            UpdateSelectedRecipe();
            Hide();
        };
        
        UpdateInputs();
        UpdateOutputs();
    }

    private void UpdateSelectedRecipe() {
        if (basePlacedObject != null && !basePlacedObject.GetComponent<IRecipeStorage>().HasItemRecipe()) {
            basePlacedObject.GetComponent<IRecipeStorage>().SetItemRecipeScriptableObject(selectedRecipe);
        }
    }
    
    private void UpdateInputs() {
        Transform inputsContainer = transform.Find("InputsContainer");
        Transform inputsTemplate = inputsContainer.Find("Template");
        inputsTemplate.gameObject.SetActive(false);
        
        foreach (Transform child in inputsContainer) {
            if (child != inputsTemplate) Destroy(child.gameObject);
        }
        
        foreach (var input in selectedRecipe.input) {
            Transform inputTransform = Instantiate(inputsTemplate, inputsContainer);
            inputTransform.gameObject.SetActive(true);
            
            inputTransform.Find("Item/Icon").GetComponent<Image>().sprite = input.item.icon;
            inputTransform.Find("Item/Amount").GetComponent<TextMeshProUGUI>().text =
                basePlacedObject.GetComponent<IItemStorage>().GetItemStoredCount(input.item) + "/" + input.amount;
        }
    }
    
    private void UpdateOutputs() {
        Transform outputsContainer = transform.Find("OutputsContainer");
        Transform outputsTemplate = outputsContainer.Find("Template");
        outputsTemplate.gameObject.SetActive(false);
        
        foreach (Transform child in outputsContainer) {
            if (child != outputsTemplate) Destroy(child.gameObject);
        }
        
        foreach (var output in selectedRecipe.output) {
            Transform outputTransform = Instantiate(outputsTemplate, outputsContainer);
            outputTransform.gameObject.SetActive(true);
            
            outputTransform.Find("Item/Icon").GetComponent<Image>().sprite = output.item.icon;
            outputTransform.Find("Item/Amount").GetComponent<TextMeshProUGUI>().text =
                basePlacedObject.GetComponent<IItemStorage>().GetItemStoredCount(output.item).ToString();
        }
    }
    
    private void OnItemStorageCountChanged(IItemStorage obj) {
        UpdateInputs();
        UpdateOutputs();
    }

    public void Show() {
        gameObject.SetActive(true);
        
        if (basePlacedObject != null) {
            basePlacedObject.GetComponent<IItemStorage>().OnItemStorageCountChanged += OnItemStorageCountChanged;
            UpdateInputs();
            UpdateOutputs();
        }
    }
    
    public void Hide() {
        gameObject.SetActive(false);
        
        if (this.basePlacedObject != null) {
            basePlacedObject.GetComponent<IItemStorage>().OnItemStorageCountChanged -= OnItemStorageCountChanged;
        }
    }
}