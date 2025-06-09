using CodeMonkey.Utils;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Vector3 = UnityEngine.Vector3;

public class CropObjectVisual : MonoBehaviour {
    [SerializeField] private CropObject cropObject;
    [SerializeField] private Transform statusIndicatorCanvas;
    [SerializeField] private Transform infoTooltipCanvas;

    private void OnEnable() {
        BasePlacedObject.OnPlacedObjectSelected += OnPlacedObjectSelected;
        CropObject.OnCropPhaseChanged += OnCropPhaseChanged;
        CropObject.OnStatusIndicatorTriggered += OnStatusIndicatorTriggered;
        
        InitStatusIndicatorCanvas();
    }

    private void OnPlacedObjectSelected(BasePlacedObject obj) {
        if (obj == cropObject) {
            ShowInfoTooltip();
        }
        else {
            HideInfoTooltip();
        }
    }

    private void ShowInfoTooltip() {
        if (infoTooltipCanvas != null) {
            if (cropObject.HasWater() && !cropObject.HasHarvest()) {
                infoTooltipCanvas.gameObject.SetActive(true);
                if (infoTooltipCanvas.gameObject.activeSelf) {
                    infoTooltipCanvas.Find("Time").GetComponent<TextMeshProUGUI>().text = UtilsClass.FormatTime(cropObject.GetRemainingProduceTime());
                }
            }
            else {
                HideInfoTooltip();
            } 
        }
    }
    
    private void HideInfoTooltip() {
        if (infoTooltipCanvas != null) {
            infoTooltipCanvas.gameObject.SetActive(false);
        }
    }
    
    private void OnDisable() {
        CropObject.OnCropPhaseChanged -= OnCropPhaseChanged;
        CropObject.OnStatusIndicatorTriggered -= OnStatusIndicatorTriggered;
    }
    
    private void InitStatusIndicatorCanvas() {
        statusIndicatorCanvas.Find("Icon").GetComponent<Image>().sprite = G.GameAssets.hintSprite_Refs.water;
    }

    private void OnCropPhaseChanged(CropObject obj) {
        if (obj == cropObject) {
            // Animate object when item produced
            cropObject.PlayPunchAnim();
            transform.Find("Visual").GetComponent<SpriteRenderer>().sprite = obj.currentCropPhase.sprite;
        }
    }
    
    private void OnStatusIndicatorTriggered(CropObject obj) {
        if(obj != cropObject) return;

        if (!cropObject.HasWater()) {
            statusIndicatorCanvas.Find("Icon").gameObject.SetActive(true);

            statusIndicatorCanvas.Find("Icon").GetComponent<Image>().sprite = G.GameAssets.hintSprite_Refs.water;
        }
        else {
            statusIndicatorCanvas.Find("Icon").gameObject.SetActive(false);
        }

        if (cropObject.storedNectarCount > 0) {
            statusIndicatorCanvas.Find("Icon").gameObject.SetActive(true);

            statusIndicatorCanvas.Find("Icon").GetComponent<Image>().sprite = G.GameAssets.hintSprite_Refs.nectar;
        }
    }
}
