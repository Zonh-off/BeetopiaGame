using UnityEngine;

public class RemovingState : IPlacementState {
    private GridDatabase<BasePlaceableSO> gridDatabase;
    private ObjectPlaceSystem _objectPlaceSystem;

    public RemovingState(GridDatabase<BasePlaceableSO> gridDatabase, ObjectPlaceSystem objectPlaceSystem) {
        this.gridDatabase = gridDatabase;
        this._objectPlaceSystem = objectPlaceSystem;
    }
    
    public void OnAction(Vector2Int position) {
        BasePlaceableSO gridObject = gridDatabase.TryGetGridObject(position).gridObject;
        bool isObject = gridDatabase.HasGridObject(position);
        bool canBeDestroyed = gridObject.canBeDestroyed;
        
        if (isObject && canBeDestroyed) {
            var worldPosition = G.PlacementManager.GetCellToWorldPosition(position);
            
            int index = gridDatabase.TryGetGridObject(position).placedObjectIndex - 1;

            gridDatabase.RemoveGridObject(position);
            _objectPlaceSystem.RemoveObject(worldPosition);
        }
    }

    public void EndState() {
        
    }

    public void UpdateState(Vector2Int position) {
        
    }
}