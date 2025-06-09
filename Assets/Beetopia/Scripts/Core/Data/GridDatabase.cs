using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GridDatabase<T> where T : BasePlaceableSO {
    public Dictionary<Vector2Int, PlacementData<T>> placedGridObjectsDatabase = new();

    public Dictionary<Vector2Int, PlacementData<T>> GetPlacedGridObjectsDatabase() => placedGridObjectsDatabase;
    
    public void AddGridObject(Vector2Int position, T gridObject, int placedObjectIndex) {
        placedGridObjectsDatabase.Add(position, new PlacementData<T>(position, gridObject, placedObjectIndex));
    }

    public void RemoveGridObject(Vector2Int position) {
        placedGridObjectsDatabase.Remove(position);
    }
    
    public PlacementData<T> TryGetGridObject(Vector2Int position) {
        return placedGridObjectsDatabase.TryGetValue(position, out var result) ? result : null;
    }
    
    public void UpdateGridObject(Vector2Int position, T newGridObject) {
        if (!HasGridObject(position)) {
            Debug.LogWarning($"No object at |{position}|");
        }
        else {
            placedGridObjectsDatabase[position].gridObject = newGridObject;
        }
    }
    
    public bool HasGridObject(Vector2Int position) {
        return placedGridObjectsDatabase.ContainsKey(position);
    }

    public bool IsValidGridPosition() {
        LayerMask layerMask = LayerMask.NameToLayer("Placing");
        RaycastHit2D rayHit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(G.InputManager.GetMouseScreenPosition()));

        return rayHit && rayHit.transform.gameObject.layer == layerMask;
    }
}

[System.Serializable]
public class PlacementData<T> {
    public Vector2Int position;
    public T gridObject;
    public int placedObjectIndex;
    public PlacementData(Vector2Int position, T gridObject, int placedObjectIndex) {
        this.position = position;
        this.gridObject = gridObject;
        this.placedObjectIndex = placedObjectIndex;
    }
}