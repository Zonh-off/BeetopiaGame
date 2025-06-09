using System.Collections;
using CodeMonkey.Utils;
using UnityEngine;

[RequireComponent(typeof(ObjectPlaceSystem))]
[RequireComponent(typeof(PreviewSystem))]
public class PlacementManager : MonoBehaviour, IProviderHandler {
    [SerializeField] private Grid grid;
    
    private ObjectPlaceSystem _objectPlaceSystem;
    private PreviewSystem _previewSystem;

    private GridDatabase<BasePlaceableSO> _gridDatabase;

    private IPlacementState _placementState;

    private BasePlacedObject _selectedPlacedObject;
    private BasePlaceableSO _selectedObjectToPlace;

    public BasePlaceableSO GetCurrentSelectedPlacableObjectSO() => _selectedObjectToPlace;

    public IEnumerator Initialize() {
        _objectPlaceSystem = GetComponent<ObjectPlaceSystem>();
        _previewSystem = GetComponent<PreviewSystem>();
        
        _gridDatabase = G.DataManager?.GridDatabase;

        G.GameManager.OnInteract += TryInteract;

        yield return true;
    }

    private void OnDestroy() {
        G.GameManager.OnInteract -= TryInteract;
    }

    public void TryInteract() {
        if (_selectedPlacedObject != null) {
            if (_selectedPlacedObject.TryGetComponent(out IInteractable interactable)) {
                interactable.OnInteract();
            }
        }
    }
    
    private void HandleInteraction() {
        var data = _objectPlaceSystem?.TryGetStoredGameObjectByPosition(GetWorldToCellPosition(UtilsClass.GetMouseWorldPosition()));

        if (!UtilsClass.IsPointerOverUI()) {
            SetSelectedObject(data?.gameObject.GetComponent<BasePlacedObject>());
        }
    }
    
    private void SetSelectedObject(BasePlacedObject selectedObject) {
        _selectedPlacedObject = selectedObject;
        BasePlacedObject.OnPlacedObjectSelected?.Invoke(_selectedPlacedObject);
    }
    
    private void Update() {
        if (_placementState is null) {
            HandleInteraction();
        }
        
        _placementState?.UpdateState(GetWorldToCellPosition(UtilsClass.GetMouseWorldPosition()));
    }
    
    public void StartPlacing(BasePlaceableSO gridObjectSo) {
        StopPlacing();
        _selectedObjectToPlace = gridObjectSo;
        _placementState = new PlacingState(gridObjectSo, _gridDatabase, _objectPlaceSystem, _previewSystem);
        G.GameManager.OnClick += PlaceStructure;
        G.GameManager.OnCancel += StopPlacing;
    }
    
    public void StopPlacing() {
        if (_placementState == null) return;
        _placementState?.EndState();
        G.GameManager.OnClick -= PlaceStructure;
        G.GameManager.OnClick -= StopPlacing;
        _selectedObjectToPlace = null;
        _placementState = null;
        G.GameManager.SetToolState(ToolState.Hand);
    }
    
    public void PlaceStructure() {
        _placementState?.OnAction(GetWorldToCellPosition(UtilsClass.GetMouseWorldPosition()));
        
        StopPlacing();
    }
    
    public void StartRemoving() {
        StopRemoving();
        
        _placementState = new RemovingState(_gridDatabase, _objectPlaceSystem);
        
        G.GameManager.OnClick += RemoveStructure;
        G.GameManager.OnCancel += StartRemoving;
    }

    private void RemoveStructure() {
        _placementState?.OnAction(GetWorldToCellPosition(UtilsClass.GetMouseWorldPosition()));
        
        StopRemoving();
    }
    
    public void StopRemoving() {
        if (_placementState == null) return;

        _placementState?.EndState();
        
        G.GameManager.OnClick -= RemoveStructure;
        G.GameManager.OnCancel -= StartRemoving;

        _placementState = null;
        
        G.GameManager.SetToolState(ToolState.Hand);
    }

    public IPlacementState GetPlacementState() => _placementState;
    
    public Vector2 GetCellToWorldPosition(Vector2Int position) {
        Vector3Int newPosition = new Vector3Int(position.x, position.y, 0);
        return new Vector2(grid.CellToWorld(newPosition).x, grid.CellToWorld(newPosition).y);
    }
    
    public Vector2Int GetWorldToCellPosition(Vector2 position) {
        Vector3 newPosition = new Vector3(position.x, position.y, 0);
        return new Vector2Int(grid.WorldToCell(newPosition).x, grid.WorldToCell(newPosition).y);
    }
}