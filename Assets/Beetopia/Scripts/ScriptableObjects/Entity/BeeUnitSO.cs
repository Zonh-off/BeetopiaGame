using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Assets/Game Data/Entity", fileName = "New Entity")]
public class BeeUnitSO : ItemSO {
    public Transform prefab;
    public uint maxCarryingAmount = 0;
    public float speed = 0;
}
