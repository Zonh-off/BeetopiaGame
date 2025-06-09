using System;
using System.Collections.Generic;
using CodeMonkey.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour, ISidePanel {
    private List<ItemSO> itemSOList = new();
    private Dictionary<ItemSO, Transform> itemButtonDic;
    private Dictionary<ItemSO, Action> itemActionDic;

    public void Init() {
        itemSOList.Add(G.GameAssets.cropObjectTypeSO_Refs.chamomile);
        itemSOList.Add(G.GameAssets.cropObjectTypeSO_Refs.pinkCat);
        itemSOList.Add(G.GameAssets.cropObjectTypeSO_Refs.purpleConeflower);
        itemSOList.Add(G.GameAssets.cropObjectTypeSO_Refs.rose);
        itemSOList.Add(G.GameAssets.cropObjectTypeSO_Refs.shadedViolet);
        itemSOList.Add(G.GameAssets.cropObjectTypeSO_Refs.springRose);
        itemSOList.Add(G.GameAssets.cropObjectTypeSO_Refs.sweetJasmine);
        itemSOList.Add(G.GameAssets.placedObjectTypeSO_Refs.nectarProcessorComb);
        itemSOList.Add(G.GameAssets.placedObjectTypeSO_Refs.nectarProcessorFramedComb);
        itemSOList.Add(G.GameAssets.placedObjectTypeSO_Refs.combProcessor);
        itemSOList.Add(G.GameAssets.placedObjectTypeSO_Refs.framedCombProcessor);

        itemSOList.Add(G.GameAssets.entity_Refs.beeUnitDefault);
        itemSOList.Add(G.GameAssets.entity_Refs.beeUnitSpeed);
        itemSOList.Add(G.GameAssets.entity_Refs.beeUnitLoader);
        
        itemActionDic = new();
        
        foreach (var item in itemSOList) {
            if (item is BasePlaceableSO) {
                itemActionDic.Add(item, () => {
                    if (G.DataManager.CanAfford(item.price)) {
                        G.PlacementManager.StartPlacing((BasePlaceableSO)item);
                        G.DataManager.RemoveCoins(item.price);
                    }
                    else {
                        UtilsClass.CreateWorldTextPopup("Not enough coins!", new Vector3(0, 0,0));
                    }
                });    
            }
            
            if (item is BeeUnitSO) {
                itemActionDic.Add(item, () => {
                    if (G.DataManager.CanAfford(item.price)) {
                        G.UnitsManager.SpawnUnit((BeeUnitSO)item, Vector3.zero);
                        G.DataManager.RemoveCoins(item.price);
                    }
                    else {
                        UtilsClass.CreateWorldTextPopup("Not enough coins!", new Vector3(0, 0,0));
                    }
                });    
            }
        }
        
        Hide();
    }

    public void Show() {
        gameObject.SetActive(true);
        SetupItemsInContainer();
    }

    public void Hide() {
        gameObject.SetActive(false);
    }
    
    private void SetupItemsInContainer() {
        Transform itemsContainer = transform.Find("ItemsContainer");
        Transform itemTemplate = itemsContainer.Find("Template");
        itemTemplate.gameObject.SetActive(false);

        foreach (Transform transform in itemsContainer) {
            if (transform != itemTemplate) {
                Destroy(transform.gameObject);
            }
        }

        itemButtonDic = new();
        
        for (int i = 0; i < itemSOList.Count; i++) {
            ItemSO itemSO = itemSOList[i];
            Transform itemTransform = Instantiate(itemTemplate, itemsContainer);
            itemTransform.gameObject.SetActive(true);

            itemButtonDic[itemSO] = itemTransform;
            
            itemTransform.Find("Item/Icon").GetComponent<Image>().sprite = itemSO.icon;
            itemTransform.Find("Item/Amount").GetComponent<TextMeshProUGUI>().text = $"<color=green>{itemSO.price}C</color>";
            itemTransform.name = $"{itemSO.name}Btn";
            
            if (itemActionDic.TryGetValue(itemSO, out Action action)) {
                itemTransform.GetComponent<Button_UI>().ClickFunc = action;
                itemTransform.GetComponent<Button_UI>().MouseOverFunc = () => {
                    float slotX = itemTransform.transform.position.x;
                    float slotY = itemTransform.transform.position.y;
                    Vector2 pos = new Vector3(
                        slotX,
                        slotY - itemTransform.GetComponent<RectTransform>().rect.size.y  - 25f
                    );
                    AddTooltipToButton(itemTransform.GetComponent<Button_UI>(), itemSO.name, pos);
                };
            }
        }
    }
    
    private void AddTooltipToButton(Button_UI buttonUI, string tooltip, Vector2 position) {
        buttonUI.MouseOverOnceTooltipFunc = () => {
            ShopTooltipCanvas.ShowTooltip_Static(tooltip, position);
        };
        buttonUI.MouseOutOnceTooltipFunc = () => {
            ShopTooltipCanvas.HideTooltip_Static();
        };
    }
}