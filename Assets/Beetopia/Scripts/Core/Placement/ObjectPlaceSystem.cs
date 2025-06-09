using System.Collections.Generic;
using UnityEngine;

public class ObjectPlaceSystem : MonoBehaviour {
    
    [SerializeField] private List<Transform> gridObjectsList = new();

    public int PlaceObject(BasePlaceableSO basePlaceableSo, Vector2 position) {
        if (basePlaceableSo != null) {
            Transform newObject = Instantiate(basePlaceableSo.prefab, transform);
            newObject.transform.position = position + new Vector2(0.5f, 0.5f);
            newObject.name = $"{basePlaceableSo.prefab.name} : {position}";
            newObject.GetComponent<BasePlacedObject>().Setup(basePlaceableSo);

            if (gridObjectsList.Exists(obj => obj == null)) {
                for (int i = 0; i < gridObjectsList.Count; i++) {
                    if (gridObjectsList[i] == null) {
                        gridObjectsList[i] = newObject;
                        return i;
                    }
                }
            }
            else {
                gridObjectsList.Add(newObject);
            } 
        }

        return gridObjectsList.Count - 1;
    }

    public void RemoveObject(Vector2 position) {
        int index = G.DataManager.GridDatabase
            .TryGetGridObject(new Vector2Int((int)position.x, (int)position.y))?.placedObjectIndex ?? -1;
        
        if(index == -1) Debug.Log($"Obj index: {index}");
        
        for (int i = 0; i < gridObjectsList.Count; i++) {
            var gridObject = gridObjectsList[i];
            if (gridObject == null) continue;

            if ((Vector2)gridObject.transform.position == position + new Vector2(0.5f, 0.5f)) {
                Destroy(gridObject.gameObject);
                
                break;
            }
        }
    }

    public Transform TryGetStoredGameObjectByPosition(Vector2Int position) {
        if (G.DataManager.GridDatabase.TryGetGridObject(position) == null) {
            return null;
        }
        //Debug.Log($"ID: {GameG.GameAssets.GridDatabase.TryGetGridObject(position).placedObjectIndex}");
        return TryGetStoredGameObjectByIndex(G.DataManager.GridDatabase.TryGetGridObject(position).placedObjectIndex);
    }

    
    public Transform TryGetStoredGameObjectByIndex(int index) {
        if (gridObjectsList[index] == null) {
            return null;
        }
        return gridObjectsList[index];
    }
}