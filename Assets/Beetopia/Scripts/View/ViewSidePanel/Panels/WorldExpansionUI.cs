using UnityEngine;

public class WorldExpansionUI : MonoBehaviour, ISidePanel {
    public void Init() {
        Hide();
    }

    public void Show() {
        gameObject.SetActive(true);
        G.WorldExpansionManager.ShowAvailableChunks();
    }

    public void Hide() {
        gameObject.SetActive(false); 
        G.WorldExpansionManager.DestroyAvailableChunks();
    }
}