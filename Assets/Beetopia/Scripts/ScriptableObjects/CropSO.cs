using NaughtyAttributes;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Assets/Game Data/Crop", fileName = "New Crop")]
public class CropSO : BasePlaceableSO {
    public ItemSO nectar;
    public ItemSO harvest;

    public int yieldAmount;
    
    public CropPhase[] cropPhasesList;

    [System.Serializable]
    public struct CropPhase {
        public int phase;
        [ShowAssetPreview]
        public Sprite sprite;
        public float time;
    }
}