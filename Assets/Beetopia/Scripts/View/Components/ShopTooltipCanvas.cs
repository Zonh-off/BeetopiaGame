using System;
using CodeMonkey.Utils;
using TMPro;
using UnityEngine;

public class ShopTooltipCanvas : MonoBehaviour {
    public static ShopTooltipCanvas Instance { get; private set; }

    [SerializeField] private RectTransform canvasRectTransform = null;

    [SerializeField] private TextMeshProUGUI textMeshPro;
    [SerializeField] private RectTransform backgroundRectTransform;
    private Func<string> getTooltipStringFunc;
    private Vector2 position;

    private void Awake() {
        Instance = this;
        //backgroundRectTransform = transform.Find("Background").GetComponent<RectTransform>();
        //textMeshPro = transform.Find("Text").GetComponent<TextMeshProUGUI>();

        if (canvasRectTransform == null) {
            Debug.LogError("Need to set Canvas Rect Transform!");
        }

        HideTooltip();

        //TestTooltip();
    }

    private void TestTooltip() {
        ShowTooltip("Testing tooltip...", new Vector2(0, 0));

        FunctionPeriodic.Create(() => {
            string abc = "qwertyuioplkjhgffdssaZX,CMBVBSDF";
            string text = "Testing tooltip...";
            for (int i = 0; i < UnityEngine.Random.Range(5, 100); i++) {
                text += abc[UnityEngine.Random.Range(0, abc.Length)];
            }
            ShowTooltip(text, new Vector2(0, 0));
        }, .5f);
    }


    private void Update() {
        SetText(getTooltipStringFunc());
        SetPosition(position);

        /*Vector2 anchoredPosition = Input.mousePosition / canvasRectTransform.localScale.x;
        if (anchoredPosition.x + backgroundRectTransform.rect.width > canvasRectTransform.rect.width) {
            anchoredPosition.x = canvasRectTransform.rect.width - backgroundRectTransform.rect.width;
        }
        if (anchoredPosition.y + backgroundRectTransform.rect.height > canvasRectTransform.rect.height) {
            anchoredPosition.y = canvasRectTransform.rect.height - backgroundRectTransform.rect.height;
        }
        transform.GetComponent<RectTransform>().anchoredPosition = anchoredPosition;*/
    }

    private void ShowTooltip(string tooltipString, Vector2 position) {
        ShowTooltip(() => tooltipString, position);
    }

    private void ShowTooltip(Func<string> getTooltipStringFunc, Vector2 position) {
        gameObject.SetActive(true);
        transform.SetAsLastSibling();
        this.getTooltipStringFunc = getTooltipStringFunc;
        this.position = position;
        Update();
    }

    private void SetText(string tooltipString) {
        textMeshPro.SetText(tooltipString);
    }
    
    private void SetPosition(Vector2 position) {
        transform.position = position;
    }

    private void HideTooltip() {
        gameObject.SetActive(false);
    }

    public static void ShowTooltip_Static(string tooltipString, Vector2 position) {
        Instance.ShowTooltip(tooltipString, position);
    }

    public static void ShowTooltip_Static(Func<string> getTooltipStringFunc, Vector2 position) {
        Instance.ShowTooltip(getTooltipStringFunc, position);
    }

    public static void HideTooltip_Static() {
        Instance.HideTooltip();
    }
}
