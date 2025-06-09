using System.Collections;
using UnityEngine;
using CodeMonkey.Utils;

public class SelectToolTypeUI : MonoBehaviour, IProviderHandler {
         private Transform toolsContainer;

    public IEnumerator Initialize() {
        toolsContainer = transform.Find("ToolsContainer");

#region Tools Container

        toolsContainer.Find("Point/HandBtn").GetComponent<Button_UI>().ClickFunc = () => {
            G.GameManager.SetToolState(ToolState.Hand);
        };
        toolsContainer.Find("Point/DemolishBtn").GetComponent<Button_UI>().ClickFunc = () => {
            G.PlacementManager.StartRemoving();
            G.GameManager.SetToolState(ToolState.Demolishing);
        };
        
#endregion
        GameManager.OnToolStateChanged += Instance_OnSelectedChanged;

        UpdateSelectedPlacedObject();
                
        AddTooltipToButton(toolsContainer.Find("Point/HandBtn").GetComponent<Button_UI>(), "Hand");
        AddTooltipToButton(toolsContainer.Find("Point/DemolishBtn").GetComponent<Button_UI>(), "Demolish");

        yield return true;
    }

    private void Instance_OnSelectedChanged() {
        UpdateSelectedPlacedObject();
    }

    private void UpdateSelectedPlacedObject() {
        toolsContainer.Find("Point/HandBtn").transform.localScale =
            G.GameManager.GetToolState() == ToolState.Hand && G.PlacementManager.GetPlacementState() == null ? new Vector3(1.2f, 1.2f, 1.2f) : Vector3.one;
        toolsContainer.Find("Point/DemolishBtn").transform.localScale =
            G.GameManager.GetToolState() == ToolState.Demolishing ? new Vector3(1.2f, 1.2f, 1.2f) : Vector3.one;
    }
    
    private void AddTooltipToButton(Button_UI buttonUI, string tooltip) {
        buttonUI.MouseOverOnceTooltipFunc = () => {
            TooltipCanvas.ShowTooltip_Static(tooltip);
        };
        buttonUI.MouseOutOnceTooltipFunc = () => {
            TooltipCanvas.HideTooltip_Static();
        };
    }
}