using NaughtyAttributes;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Assets/Game Data/Structures", fileName = "New Structure")]
public class BasePlaceableSO : ItemSO
{
    [ShowAssetPreview] public Transform prefab;
    [ShowAssetPreview] public Transform visual;
    public bool canBeDestroyed = true;
}