using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class BeeUnitBehaviour : MonoBehaviour, IItemStorage {
    private enum AIState { Waiting, LookingForTask, MovingToTarget, Action }

    private uint maxCarryingAmount;
    private float speed;
    private AIState state = AIState.LookingForTask;
    
    private Vector3 idleTargetPosition;
    private float idleWaitTime = 0.1f;
    private float idleMoveRadius = 3f;
    
    private ITask currentTask;

    private readonly ItemStackList carryingItemStackList = new();
    public uint GatheredItemsCount { get; set; }

    public uint GetCarryCapacity() => maxCarryingAmount;

    public void Setup(uint carryCapacity, float movementSpeed) {
        maxCarryingAmount = carryCapacity;
        speed = movementSpeed;
        StartCoroutine(StateMachine());
    }

    private void Update() {
        if (currentTask != null)
            Debug.DrawLine(transform.position, currentTask.GetTargetPosition(), Color.red);
    }

    private IEnumerator StateMachine() {
        while (true) {
            yield return new WaitForSeconds(0.5f);
            switch (state) {
                case AIState.Waiting:
                    idleTargetPosition = GetRandomNearbyPosition(FindFirstObjectByType<StorageObject>().gameObject.transform.position, idleMoveRadius);
                    yield return MoveTo(idleTargetPosition);
                    yield return new WaitForSeconds(idleWaitTime);
                    state = AIState.LookingForTask;
                    break;
                case AIState.LookingForTask:
                    currentTask ??= G.TaskManager.GetNextTask(this);
                    state = currentTask != null ? AIState.Action : AIState.Waiting;
                    break;

                case AIState.MovingToTarget:
                    if (currentTask == null) {
                        state = AIState.LookingForTask;
                        break;
                    }
                    yield return MoveTo(currentTask.GetTargetPosition());
                    state = AIState.Action;
                    break;
                case AIState.Action:
                    if (currentTask != null) {
                        yield return currentTask.Execute(this);
                        state = currentTask.IsCompleted() ? AIState.Waiting : AIState.Action;
                        if (state == AIState.Waiting) currentTask = null;
                    }
                    break;
            }
        }
    }

    public bool IsGatheringMode { get; set; }
    public uint LastCollectedAmount { get; private set; }
    
    public IEnumerator CollectItem(BasePlacedObject target, ItemSO itemSO, uint amount) {
        if (!TryGetStorage(target, out var storage)) yield break;
        IsGatheringMode = true;
        yield return MoveTo(target.transform.position);
        yield return PlayActionAnimation();
        LastCollectedAmount = TryCollectItems(storage, itemSO, amount);
        IsGatheringMode = false;
    }
    
    public IEnumerator CollectItem(WorldItem target, ItemSO itemSO) {
        yield return MoveTo(target.transform.position);
        yield return PlayActionAnimation();
        if (!TryStoreItem(itemSO)) Debug.LogWarning("[BeeUnit] Failed to collect world item: inventory full?");
    }

    public IEnumerator DeliverItem(BasePlacedObject target, ItemSO itemSO, uint amount = 1) {
        if (!TryGetStorage(target, out var storage)) yield break;
        yield return MoveTo(target.transform.position);
        TryDeliverItems(storage, itemSO, amount);
        
        yield return PlayActionAnimation();
    }

    private uint TryCollectItems(IItemStorage storage, ItemSO itemSO, uint amount) {
        uint collected = 0;
        for (int i = 0; i < amount; i++) {
            if (storage.TryGetStoredItem(new ItemSO[] { itemSO }, out ItemSO item)) {
                TryStoreItem(item);
                collected++;
            } else {
                break;
            }
        }
        return collected;
    }
    
    private void TryDeliverItems(IItemStorage storage, ItemSO itemSO, uint amount) {
        Debug.Log($"[BeeUnit] Delivering {amount} x {itemSO.name} to {storage}");
        for (int i = 0; i < amount; i++) {
            if (TryGetStoredItem(new[] { itemSO }, out var item)) {
                storage.TryStoreItem(item);
            } else break;
        }
    }

    private static bool TryGetStorage(Component target, out IItemStorage storage) {
        storage = target.GetComponent<IItemStorage>();
        return storage != null;
    }

    public IEnumerator MoveTo(Vector3 position) {
        float duration = Vector3.Distance(transform.position, position) / Mathf.Max(speed, 0.01f);
        yield return transform.DOMove(position, duration).SetEase(Ease.Linear).WaitForCompletion();
    }

    private Vector3 GetRandomNearbyPosition(Vector3 origin, float radius) {
        Vector2 offset = Random.insideUnitCircle * radius;
        return origin + new Vector3(offset.x, offset.y, 0f);
    }
    
    public object PlayActionAnimation()
    {
        return transform
           .DOScale(0.2f, 0.2f)
           .SetEase(Ease.InOutQuad)
           .SetLoops(2, LoopType.Yoyo)
           .WaitForCompletion();
    }

    public void DestroySelf() => Destroy(gameObject);

#region Storage
    public event Action<IItemStorage> OnItemStorageCountChanged;

    public uint GetItemStoredCount(ItemSO filterItemSO) =>
        carryingItemStackList.GetItemStoredCount(filterItemSO);

    public bool TryGetStoredItem(ItemSO[] filterItemSO, out ItemSO itemSO) {
        var itemStack = carryingItemStackList.GetFirstItemStackWithFilter(filterItemSO);
        if (itemStack is { amount: > 0 }) {
            itemStack.amount--;
            GatheredItemsCount--;
            itemSO = itemStack.itemSO;
            OnItemStorageCountChanged?.Invoke(this);
            return true;
        }

        itemSO = null;
        return false;
    }

    public bool TryStoreItem(ItemSO itemSO) {
        if (carryingItemStackList.CanAddItemToItemStack(itemSO)) {
            carryingItemStackList.AddItemToItemStack(itemSO);
            OnItemStorageCountChanged?.Invoke(this);
            return true;
        }
        return false;
    }

    public ItemSO[] GetItemSOThatCanStore() => new[] { G.GameAssets.itemSO_Refs.any };
    public ItemStackList GetItemStackList() => carryingItemStackList;
#endregion
}


/*
using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class BeeUnitBehaviour : MonoBehaviour, IItemStorage {
    private enum AIState {
        Waiting,
        LookingForTask,
        MovingToTarget,
        Action
    }

    public Sprite sprite;
    
    private uint maxCarryingAmount;
    private float speed;
    
    private AIState state;
    private Transform targetTransform;
    private ItemStackList carryingItemStackList;
    private uint gatheredItemsCount = 0;
    private ITask currentTask;
    private ItemSO grabFilterItemSO;
    private bool isGatheringMode = false;
    
    public bool IsGatheringMode { get; private set; } = false;
    public uint GatheredItemsCount { get; private set; }
    
    public uint GetCarryCapacity() => maxCarryingAmount;
    
    public void Setup(uint maxCarryingAmount, float speed) {
        this.maxCarryingAmount = maxCarryingAmount;
        this.speed = speed;
        
        carryingItemStackList = new ItemStackList();
        currentTask = null;
        state = AIState.LookingForTask;
        grabFilterItemSO = G.GameAssets.itemSO_Refs.any;
        StartCoroutine(StateMachine());
    }

    private void Update() {
        if (currentTask != null) {
            Debug.DrawLine(transform.position, currentTask.GetTargetPosition(), Color.red);
        }
    }

    private IEnumerator StateMachine() {
        while (true) {
            switch (state) {
                case AIState.Waiting:
                    yield return new WaitForSeconds(.5f);
                    state = AIState.LookingForTask;
                    break;

                case AIState.LookingForTask:
                    yield return new WaitForSeconds(.5f);
                    if (currentTask == null) {
                        currentTask = G.TaskManager.GetNextTask(this);
                        if (currentTask != null)
                        {
                            Debug.Log($"{this} : {currentTask}");
                            state = AIState.Action;
                        }
                    }
                    break;

                case AIState.MovingToTarget:
                    if (currentTask == null) {
                        state = AIState.LookingForTask;
                        break;
                    }
                    
                    transform.DOMove(currentTask.GetTargetPosition(), speed).OnComplete(() => {
                        state = AIState.Action;
                    });

                    yield return new WaitForSeconds(speed);
                    break;

                case AIState.Action:
                    if (currentTask != null) {
                        yield return StartCoroutine(currentTask.Execute(this));
                        
                        if (currentTask.IsCompleted()) {
                            currentTask = null;
                            state = AIState.Waiting;
                        }
                        else {
                            currentTask.Execute(this);
                        }
                    }
                    
                    break;

                default:
                    throw new System.ArgumentOutOfRangeException();
            }

            yield return null;
        }
    }
    
    // Collect item from storage of placed object
    public IEnumerator CollectItem(BasePlacedObject target, ItemSO itemSO, uint amount) {
        var storage = target.GetComponent<IItemStorage>();
        if (storage == null) {
            yield break;
        }
        
        yield return MoveTo(target.transform.position);

        int collectedAmount = 0;
    
        for (int i = 0; i < amount; i++) {
            if (storage.TryGetStoredItem(new ItemSO[] { itemSO }, out ItemSO collectedItem)) {
                TryStoreItem(collectedItem);
                collectedAmount++;
                Debug.Log("Collected");
            } else {
                break;
            }
        }
    }
    
    // Collect dropped item in world
    public IEnumerator CollectItem(WorldItem target, ItemSO itemSO) {
        yield return MoveTo(target.transform.position);
    
        bool stored = TryStoreItem(itemSO);
        if (!stored) {
            Debug.LogWarning("[BeeUnitBehaviour] Failed to collect world item: inventory full?");
        }
    }

    
    // Delivery item to target
    public IEnumerator DeliverItem(BasePlacedObject target, ItemSO itemSO, uint amount = 1) {
        var storage = target.GetComponent<IItemStorage>();
        if (storage == null) {
            yield break;
        }
        
        yield return MoveTo(target.transform.position);

        int deliveredAmount = 0;
        
        Debug.Log($"[BeeUnitBehaviour] Trying to deliver {amount}x {itemSO.name} to {target.name}");

        for (int i = 0; i < amount; i++) {
            if (TryGetStoredItem(new ItemSO[] { itemSO }, out ItemSO itemToDeliver)) {
                storage.TryStoreItem(itemToDeliver);
                deliveredAmount++;
            } else {
                break;
            }
        }
    }

    public IEnumerator MoveTo(Vector3 position) {
        float distance = Vector3.Distance(transform.position, position);
        float duration = distance / Mathf.Max(speed, 0.01f);

        Tween moveTween = transform.DOMove(position, duration).SetEase(Ease.Linear);
        yield return moveTween.WaitForCompletion();
    }

    
    public void DestroySelf() {
        Destroy(gameObject);
    }
    
#region Storage
    public event Action<IItemStorage> OnItemStorageCountChanged;
    
    public uint GetItemStoredCount(ItemSO filterItemSO) {
        return carryingItemStackList.GetItemStoredCount(filterItemSO);
    }

    public bool TryGetStoredItem(ItemSO[] filterItemSO, out ItemSO itemSO) {
        ItemStack itemStack = carryingItemStackList.GetFirstItemStackWithFilter(filterItemSO);
        if (itemStack != null && itemStack.amount > 0) {
            itemStack.amount--;
            gatheredItemsCount--;
            itemSO = itemStack.itemSO;
            OnItemStorageCountChanged?.Invoke(this);
            return true;
        } else {
            itemSO = null;
            return false;
        }
    }

    public bool TryStoreItem(ItemSO itemSO) {
        if (carryingItemStackList.CanAddItemToItemStack(itemSO)) {
            carryingItemStackList.AddItemToItemStack(itemSO);
            OnItemStorageCountChanged?.Invoke(this);
            
            return true;
        } else {
            return false;
        }
    }

    public ItemSO[] GetItemSOThatCanStore() {
        return new ItemSO[] { G.GameAssets.itemSO_Refs.any };
    }
    
    public ItemStackList GetItemStackList() {
        return carryingItemStackList;
    }
#endregion
}   
*/