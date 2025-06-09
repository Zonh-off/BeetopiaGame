using System;
using System.Collections.Generic;
using CodeMonkey.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StorageUI : MonoBehaviour, ISidePanel {
    private Dictionary<ItemSO, Transform> itemButtonDic;
    private Dictionary<ItemSO, Action> itemActionDic;
    
    private void OnDestroy() => DataManager.OnItemsUpdate -= UpdateItemList;

    public void Init()
    {
        Hide();
    }

    public void Show() {
        gameObject.SetActive(true);
        DataManager.OnItemsUpdate += UpdateItemList;
        UpdateItemList();
    }

    public void Hide() {
        DataManager.OnItemsUpdate -= UpdateItemList;
        gameObject.SetActive(false);
    }
    
    private void UpdateItemList() {
        Transform itemContainer = transform.Find("ItemsContainer");
        Transform itemTemplate = itemContainer.Find("Template");
        itemTemplate.gameObject.SetActive(false);

        foreach (Transform transform in itemContainer) {
            if (transform != itemTemplate) {
                Destroy(transform.gameObject);
            }
        }
        
        itemActionDic = new();
        itemButtonDic = new();

        ItemStackList itemStackList = G.DataManager?.GameData.itemStackList;
        
        foreach (var item in itemStackList.itemStackList) {
            var currentItem = item.itemSO;
            if (currentItem == null) continue;

            itemActionDic[currentItem] = () => {
                var filter = new[] { currentItem };
                if (G.DataManager.CheckStoredItem(filter, 1)) {
                    G.DataManager.GameData.itemStackList.RemoveItemFromItemStack(currentItem);
                    G.DataManager.AddCoins(currentItem.price);
                } else {
                    UtilsClass.CreateWorldTextPopup("Not enough items!", Vector3.zero);
                }
            };
        }
        
        foreach (ItemStack itemStack in itemStackList!.GetItemStackList()) {
            ItemSO itemSO = itemStack.itemSO;
            
            Transform itemTransform = Instantiate(itemTemplate, itemContainer);
            itemTransform.gameObject.SetActive(true);
            
            itemButtonDic[itemSO] = itemTransform;
            
            itemTransform.Find("Item/Icon").GetComponent<Image>().sprite = itemSO.icon;
            itemTransform.Find("Item/Amount").GetComponent<TextMeshProUGUI>().text = $"{itemStack.amount} | <color=red>{itemSO.price}C</color>";
            itemTransform.name = $"{itemSO.name}Btn";
            
            if (itemActionDic.TryGetValue(itemSO, out Action action)) {
                itemTransform.GetComponent<Button_UI>().ClickFunc = action;
            }
        }
    }
}