using System.Collections;
using UnityEngine;

public class PlaceObjectTask : ITask {
    public int Priority => 2;
    
    private BasePlaceableSO _targetBasePlaceableSo;
    private GridDatabase<BasePlaceableSO> gridDatabase;
    private ObjectPlaceSystem objectPlaceSystem;
    private Vector2Int position;
    
    private bool isCompleted = false;

    private PreviewSystem previewSystem;
    private int previewIndex;
    private int index;
    
    public PlaceObjectTask(
        BasePlaceableSO targetBasePlaceableSo,
        GridDatabase<BasePlaceableSO> gridDatabase, 
        ObjectPlaceSystem objectPlaceSystem,
        PreviewSystem previewSystem,
        Vector2Int position) {
        this._targetBasePlaceableSo = targetBasePlaceableSo;
        this.objectPlaceSystem = objectPlaceSystem;
        this.gridDatabase = gridDatabase;
        this.previewSystem = previewSystem;
        this.position = position;
        
        previewIndex = previewSystem.AddPreviewObject(targetBasePlaceableSo, this.position);
        
        index = objectPlaceSystem.PlaceObject(_targetBasePlaceableSo, position);
        objectPlaceSystem.TryGetStoredGameObjectByIndex(index).gameObject.SetActive(false);
        
        BasePlaceableSO newGridObject = Object.Instantiate(_targetBasePlaceableSo);
        ((Object)newGridObject).name = ((Object)_targetBasePlaceableSo).name + index; 
            
        gridDatabase.AddGridObject(position, newGridObject, index);
    }

    public Vector3 GetTargetPosition() => new Vector3(position.x, position.y, 0);

    public IEnumerator Execute(BeeUnitBehaviour workerBeeUnitBehaviour) {
        yield return workerBeeUnitBehaviour.MoveTo(GetTargetPosition());

        objectPlaceSystem.TryGetStoredGameObjectByIndex(index).gameObject.SetActive(true);
        objectPlaceSystem.TryGetStoredGameObjectByIndex(index).GetComponent<BasePlacedObject>().PlayPunchAnim();

        previewSystem.RemovePreviewObject(previewIndex);
        
        isCompleted = true;
    }

    public bool IsCompleted() {
        return isCompleted;
    }
}