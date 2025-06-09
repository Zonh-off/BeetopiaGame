using System;
using CodeMonkey.Utils;
using TMPro;
using UnityEngine;

public class TooltipCanvas : MonoBehaviour {

    public static TooltipCanvas Instance { get; private set; }

    [SerializeField]
    private RectTransform canvasRectTransform = null;

    private TextMeshProUGUI textMeshPro;
    private RectTransform backgroundRectTransform;
    private Func<string> getTooltipStringFunc;

    private void Awake() {
        Instance = this;
        backgroundRectTransform = transform.Find("Background").GetComponent<RectTransform>();
        textMeshPro = transform.Find("Text").GetComponent<TextMeshProUGUI>();

        if (canvasRectTransform == null) {
            Debug.LogError("Need to set Canvas Rect Transform!");
        }

        HideTooltip();

        //TestTooltip();
    }

    private void TestTooltip() {
        ShowTooltip("Testing tooltip...");

        FunctionPeriodic.Create(() => {
            string abc = "qwertyuioplkjhgffdssaZX,CMBVBSDF";
            string text = "Testing tooltip...";
            for (int i = 0; i < UnityEngine.Random.Range(5, 100); i++) {
                text += abc[UnityEngine.Random.Range(0, abc.Length)];
            }
            ShowTooltip(text);
        }, .5f);
    }


    private void Update() {
        SetText(getTooltipStringFunc());

        Vector2 anchoredPosition = Input.mousePosition / canvasRectTransform.localScale.x;
        if (anchoredPosition.x + backgroundRectTransform.rect.width > canvasRectTransform.rect.width) {
            anchoredPosition.x = canvasRectTransform.rect.width - backgroundRectTransform.rect.width;
        }
        if (anchoredPosition.y + backgroundRectTransform.rect.height > canvasRectTransform.rect.height) {
            anchoredPosition.y = canvasRectTransform.rect.height - backgroundRectTransform.rect.height;
        }
        transform.GetComponent<RectTransform>().anchoredPosition = anchoredPosition;
    }

    private void ShowTooltip(string tooltipString) {
        ShowTooltip(() => tooltipString);
    }

    private void ShowTooltip(Func<string> getTooltipStringFunc) {
        gameObject.SetActive(true);
        transform.SetAsLastSibling();
        this.getTooltipStringFunc = getTooltipStringFunc;
        Update();
    }

    private void SetText(string tooltipString) {
        textMeshPro.SetText(tooltipString);
        textMeshPro.ForceMeshUpdate();
        Vector2 textSize = textMeshPro.GetRenderedValues(false);

        Vector2 padding = new Vector2(0f, 0f);
        backgroundRectTransform.sizeDelta = textSize + padding;
    }

    private void HideTooltip() {
        gameObject.SetActive(false);
    }

    public static void ShowTooltip_Static(string tooltipString) {
        Instance.ShowTooltip(tooltipString);
    }

    public static void ShowTooltip_Static(Func<string> getTooltipStringFunc) {
        Instance.ShowTooltip(getTooltipStringFunc);
    }

    public static void HideTooltip_Static() {
        Instance.HideTooltip();
    }
}