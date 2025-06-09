using System;
using UnityEngine;

public class CropObject : BasePlacedObject, IItemStorage, IHarvest {
    public event Action<IItemStorage> OnItemStorageCountChanged;
    public static event Action<CropObject> OnCropPhaseChanged;
    public static event Action<CropObject> OnStatusIndicatorTriggered;
    
    [SerializeField] private CropSO.CropPhase[] cropPhasesList;

    [SerializeField] private float waterTime;
    [SerializeField] private float maxNectarProduceTime = 5f;
    
    private ItemSO nectar;
    private ItemSO harvest;
    private uint _storedNectarCount;
    private int yieldAmount;
 
    private float _currentNectarProduceTime = 5f;
    private CropSO.CropPhase _currentCropPhase;
    
    private float _currentPhaseGrowingTime;
    private float _currentWaterTime = 0;
    
    private bool hasWater = false;
    private bool isLastPhase = false;
    private bool hasWaterTask = false;
    
    public CropSO.CropPhase currentCropPhase => _currentCropPhase;
    public uint storedNectarCount => _storedNectarCount;
    
    public override void Setup(BasePlaceableSO basePlaceableSO) {
        base.Setup(basePlaceableSO);
        
        var crop = (CropSO)basePlaceableSO;
        harvest = crop.harvest;
        
        nectar = crop.nectar;
        cropPhasesList = crop.cropPhasesList;
        
        _storedNectarCount = 0;
        
#if UNITY_EDITOR
        UpdatePhase(cropPhasesList.Length - 1);
        yieldAmount = 1;
#else
        yieldAmount = crop.yieldAmount;
        UpdatePhase(0);
#endif
    }
    
    private void Update() {
        UpdateTimers();
        CheckAndUpdateWaterCondition();
        CheckAndUpdatePhase();
        CheckAndUpdateNectar();
    }
    
    private void UpdatePhase(int phaseIndex) {
        _currentCropPhase = cropPhasesList[phaseIndex];
        isLastPhase = _currentCropPhase.phase == cropPhasesList.Length - 1;
#if UNITY_EDITOR
        _currentPhaseGrowingTime = 1f;
#else
        _currentPhaseGrowingTime = _currentCropPhase.time;
#endif
        OnCropPhaseChanged?.Invoke(this);
    }

    private void UpdateTimers() {
        // Check if the flower has water then decrease timers
        if (hasWater && !isLastPhase) {
            _currentWaterTime -= Time.deltaTime;
            _currentPhaseGrowingTime -= Time.deltaTime;
        }
    }

    private void CheckAndUpdateWaterCondition() {
        // Check water condition
        if (_currentWaterTime < 0f) {
            hasWater = false;
            if (!hasWaterTask) {
                G.TaskManager.AddTask(new DeliverWaterTask(this));
                hasWaterTask = true;
            }
            OnStatusIndicatorTriggered?.Invoke(this);
        }
        else {
            hasWater = true;
            OnStatusIndicatorTriggered?.Invoke(this);
        }
    }

    private void CheckAndUpdateNectar() {
        // Check nectar produce condition
        if (isLastPhase && _storedNectarCount <= 0) {
            _currentNectarProduceTime -= Time.deltaTime;
            if (_currentNectarProduceTime <= 0f) {
                if (yieldAmount <= 0) {
                    WorldItem.Create(harvest, transform.position);
                    G.DataManager.GridDatabase.RemoveGridObject(GetGridPosition());
                    Destroy(gameObject);
                }
                else {
                    G.TaskManager.AddTask(new GatherItemTask(this));
                    _storedNectarCount = 1;
                    _currentNectarProduceTime = maxNectarProduceTime;
                    yieldAmount--;
                    OnStatusIndicatorTriggered?.Invoke(this);
                    OnItemStorageCountChanged?.Invoke(this);
                }
            }
        }
    }

    private void CheckAndUpdatePhase() {
        // Check phase
        if (!isLastPhase) {
            // Check growing time than update phase, timer, visual
            if (_currentPhaseGrowingTime <= 0f) {
                int nextPhaseIndex = _currentCropPhase.phase + 1;
                UpdatePhase(nextPhaseIndex);
            }
        }
    }

    public bool HasHarvest() => _storedNectarCount >= 1;
    public bool HasWater() => hasWater;
    public bool HasWaterTask() => hasWaterTask;
    public float GetRemainingProduceTime() => _currentNectarProduceTime;
    
    public int GetItemStoredCount(ItemSO filterItemSO) { throw new NotImplementedException(); }

    uint IItemStorage.GetItemStoredCount(ItemSO filterItemSO) { throw new NotImplementedException(); }

    public bool TryGetStoredItem(ItemSO[] filterItemSO, out ItemSO itemSO) {
        if (ItemSO.IsItemSOInFilter(G.GameAssets.itemSO_Refs.any, filterItemSO) ||
            ItemSO.IsItemSOInFilter(nectar, filterItemSO)) {
            // If filter matches any or filter matches this itemType
            if (_storedNectarCount > 0) {
                _storedNectarCount--;
                itemSO = nectar;
                _currentNectarProduceTime = maxNectarProduceTime;
                OnItemStorageCountChanged?.Invoke(this);
                // TODO: TriggerGridObjectChanged();
                return true;
            } else {
                itemSO = null;
                return false;
            }
        } else {
            itemSO = null;
            return false;
        }
    }

    public bool TryStoreItem(ItemSO itemSO) {
        throw new NotImplementedException();
    }

    public ItemSO[] GetItemSOThatCanStore() {
        throw new NotImplementedException();
    }
    
    public bool PourWater() {
        if (!HasWater()) {
            _currentWaterTime = waterTime;
            hasWaterTask = false;
            return true;
        }

        return false;
    }
    
    public bool CollectHarvest() {
        if (HasHarvest()) {
            Vector2 offset = new Vector2(transform.position.x, transform.position.y + 0.5f);
            WorldItem.Create(nectar, offset);
            _storedNectarCount = 0;
            OnItemStorageCountChanged?.Invoke(this);
            return true;
        }

        return false;
    }
}