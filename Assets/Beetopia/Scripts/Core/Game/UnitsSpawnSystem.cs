using System.Collections.Generic;
using UnityEngine;

public class UnitsSpawnSystem : MonoBehaviour {
    [SerializeField] private List<Transform> beeList = new();

    // TODO: Can be modified to spawn any entity type
    public int SpawnBeeEntityObject(BeeUnitSO beeUnitSo, Vector3 position) {
        if (beeUnitSo != null) {
            Transform newEntityObject = Instantiate(beeUnitSo.prefab, transform);
            newEntityObject.transform.position = position;
            newEntityObject.name = beeUnitSo.name;
            newEntityObject.GetComponent<BeeUnitBehaviour>().Setup(beeUnitSo.maxCarryingAmount, beeUnitSo.speed);

            if (beeList.Exists(obj => obj == null)) {
                for (int i = 0; i < beeList.Count; i++) {
                    if (beeList[i] == null) {
                        beeList[i] = newEntityObject;
                        return i;
                    }
                }
            }
            else {
                beeList.Add(newEntityObject);
            }
        }

        return beeList.Count - 1;
    }
    
    // TODO: Can be modified to remove any entity type
    public void RemoveBeeEntity(int index) {
        if(index <= -1) return;
        
        Transform beeEntityObject = beeList[index];
        
        beeEntityObject.GetComponent<BeeUnitBehaviour>().DestroySelf();
    }

    public Transform TryGetStoredBeeUnitObjectByIndex(int index) {
        if (beeList[index] == null) {
            Debug.Log("Incorrect index or out of bounds!");
            return null;
        }
        return beeList[index];
    }
}