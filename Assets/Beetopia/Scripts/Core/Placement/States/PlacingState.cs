using CodeMonkey.Utils;
using UnityEngine;

public class PlacingState : IPlacementState {
    private GridDatabase<BasePlaceableSO> gridDatabase;
    private BasePlaceableSO gridObjectSO;
    private ObjectPlaceSystem objectPlaceSystem;
    private PreviewSystem previewSystem;
    
    public PlacingState(BasePlaceableSO gridObjectSO, GridDatabase<BasePlaceableSO> gridDatabase, ObjectPlaceSystem objectPlaceSystem, PreviewSystem previewSystem) {
        this.gridDatabase = gridDatabase;
        this.gridObjectSO = gridObjectSO;
        this.objectPlaceSystem = objectPlaceSystem;
        this.previewSystem = previewSystem;

        previewSystem.StartShowingPreview(gridObjectSO);
    }
    
    public void OnAction(Vector2Int position) {
        bool canPlace = !gridDatabase.HasGridObject(position);

        if (!UtilsClass.IsPointerOverUI()) {
            if (canPlace) {
                G.TaskManager.AddTask(new PlaceObjectTask(gridObjectSO, gridDatabase, objectPlaceSystem, previewSystem, position));
            }
            else {
                // Cannot build here
                UtilsClass.CreateWorldTextPopup("Cannot Build Here!", new Vector3(position.x, position.y, 0));
            }
        }
        
        previewSystem.UpdatePosition(position, canPlace);
    }

    public void EndState() {
        previewSystem.StopShowingPreview();
    }

    public void UpdateState(Vector2Int position) {
        previewSystem.UpdatePosition(position, gridDatabase.HasGridObject(position) || !gridDatabase.IsValidGridPosition());
    }
}