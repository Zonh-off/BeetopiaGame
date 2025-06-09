using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BeeListUI : MonoBehaviour, ISidePanel {
    private void OnDestroy() {
        DataManager.OnBeesUpdate -= DataManager_OnBeesUpdate;
    }

    private void OnEnable() {
        DataManager.OnBeesUpdate += DataManager_OnBeesUpdate;
    }

    private void DataManager_OnBeesUpdate() {
        UpdateItemList();
    }

    public void Init() {
        Hide();
    }

    public void Show() {
        gameObject.SetActive(true);
        UpdateItemList();
    }

    public void Hide() {
        gameObject.SetActive(false);
    }
    
    private void UpdateItemList() {
        Transform itemContainer = FindInactiveChildRecursive(transform, "ItemsContainer");
        Transform itemTemplate = itemContainer.Find("Template");
        itemTemplate.gameObject.SetActive(false);

        foreach (Transform transform in itemContainer) {
            if (transform != itemTemplate) {
                Destroy(transform.gameObject);
            }
        }

        List<BeeUnitSO> beeList = G.DataManager.GameData.beeList;

        foreach (BeeUnitSO bee in beeList) {
            Transform itemTransform = Instantiate(itemTemplate, itemContainer);
            itemTransform.gameObject.SetActive(true);

            itemTransform.Find("Item").Find("Icon").GetComponent<Image>().sprite = bee.icon;
            itemTransform.Find("Item").Find("Amount").GetComponent<TextMeshProUGUI>().text = bee.name;
        }
    }
    public static Transform FindInactiveChildRecursive(Transform parent, string name) {
        foreach (Transform child in parent) {
            if (child.name == name)
                return child;

            Transform result = FindInactiveChildRecursive(child, name);
            if (result != null)
                return result;
        }
        return null;
    }
    
}