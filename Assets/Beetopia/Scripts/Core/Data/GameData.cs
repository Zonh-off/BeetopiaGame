using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData {
    public uint coins = 50;
    
    public ItemStackList itemStackList = new();
    public List<PlacementData<BasePlacedObject>> placedObjectsList = new();
    public List<BeeUnitSO> beeList = new();
    
    public GameData() {
        this.coins = 50;
    }
    
    public string ToJson() {
        return JsonUtility.ToJson(this, true);
    }

    public void LoadJson(string jsonFilepath) {
        JsonUtility.FromJsonOverwrite(jsonFilepath, this);
    }
}