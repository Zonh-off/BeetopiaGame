using System;
using System.Collections;
using CodeMonkey.Utils;
using TMPro;
using UnityEngine;

public class SidePanelUI : MonoBehaviour, IProviderHandler {
    [Serializable]
    public enum SidePanelType {
        None,
        Storage,
        Shop,
        BeeList,
        WorldExpansion
    }
    
    [SerializeField] private Transform sidePanelContainer;
    
    [SerializeField] private Transform sectionsSelectorsContainer;
    
    [SerializeField] private Transform storageSection;
    [SerializeField] private Transform shopSection;
    [SerializeField] private Transform beesSection;
    [SerializeField] private Transform worldExpansion;
    
    [SerializeField] private TextMeshProUGUI coinsAmountText;

    [SerializeField] private SidePanelType sidePanelType;

    public IEnumerator InitializeComponents() {
        sidePanelType = SidePanelType.None;
        
        sectionsSelectorsContainer.Find("StorageBtn").GetComponent<Button_UI>().ClickFunc = () => ShowSelectedSidePanel(SidePanelType.Storage);
        sectionsSelectorsContainer.Find("ShopBtn").GetComponent<Button_UI>().ClickFunc = () =>  ShowSelectedSidePanel(SidePanelType.Shop);
        sectionsSelectorsContainer.Find("BeesBtn").GetComponent<Button_UI>().ClickFunc = () => ShowSelectedSidePanel(SidePanelType.BeeList);
        sectionsSelectorsContainer.Find("WorldExpansionBtn").GetComponent<Button_UI>().ClickFunc = () => ShowSelectedSidePanel(SidePanelType.WorldExpansion);
        
        shopSection.GetComponent<ISidePanel>().Init();
        worldExpansion.GetComponent<ISidePanel>().Init();
        
        ShowSelectedSidePanel(SidePanelType.None);

        DataManager.OnStatsUpdate += OnStatsUpdate;

        yield return true;
    }
    
    public void ShowSelectedSidePanel(SidePanelType sidePanelType) {
        this.sidePanelType = sidePanelType;

        shopSection.GetComponent<ISidePanel>().Hide();
        storageSection.GetComponent<ISidePanel>().Hide();
        beesSection.GetComponent<ISidePanel>().Hide();
        worldExpansion.GetComponent<ISidePanel>().Hide();

        var result = sidePanelType switch {
            SidePanelType.None => null,
            SidePanelType.Storage => storageSection,
            SidePanelType.Shop => shopSection,
            SidePanelType.BeeList => beesSection,
            SidePanelType.WorldExpansion => worldExpansion,
            _ => null
        };
        result?.GetComponent<ISidePanel>().Show();
    }

    private void OnStatsUpdate() {
        SetText();
    }

    private void Update() {
        if (!UtilsClass.IsPointerOverUI() && G.PlacementManager.GetCurrentSelectedPlacableObjectSO() == null && Input.GetMouseButtonDown(0)) {
            ShowSelectedSidePanel(SidePanelType.None);
        }
    }
    
    private void SetText() {
        coinsAmountText.text = $"{G.DataManager.GameData.coins}<color=yellow>C</color>";
    }
}
