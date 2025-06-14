using System.Collections;
using UnityEngine;

[RequireComponent(typeof(UnitsSpawnSystem))]
public class UnitsManager : MonoBehaviour, IProviderHandler {
    
    private UnitsSpawnSystem _unitsSpawnSystem;
    
    public IEnumerator Initialize() {
        _unitsSpawnSystem = GetComponent<UnitsSpawnSystem>();

        yield return true;
    }
    
    public bool SpawnUnit(BeeUnitSO beeUnitSo, Vector3 position) {
        int index = _unitsSpawnSystem.SpawnBeeEntityObject(beeUnitSo, position);

        _unitsSpawnSystem.TryGetStoredBeeUnitObjectByIndex(index).GetComponent<BeeUnitBehaviour>();
        _unitsSpawnSystem.TryGetStoredBeeUnitObjectByIndex(index).name = $"Bee {index}";
        G.DataManager.TryStoreUnit(beeUnitSo);
        
        G.DataManager.RemoveCoins(beeUnitSo.price);

        return true;
    }
}