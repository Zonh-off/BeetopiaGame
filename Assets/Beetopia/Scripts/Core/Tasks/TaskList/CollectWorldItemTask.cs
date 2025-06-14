using System.Collections;
using UnityEngine;

public class CollectWorldItemTask : ITask {
    public int Priority => 3;
    public BeeUnitBehaviour assignedBee { get; set; }

    private WorldItem worldItem;
    private Vector3 worldItemPos;
    private bool isCompleted = false;

    public CollectWorldItemTask(WorldItem targetWorldItem) {
        worldItem = targetWorldItem;
    }

    public Vector3 GetTargetPosition() => worldItemPos;

    public IEnumerator Execute(BeeUnitBehaviour bee) {
        if (worldItem.GetItemSO() != null) {
            var itemSO = worldItem.GetItemSO();
            var deliveryTarget = Object.FindFirstObjectByType<StorageObject>();
            
            worldItemPos = worldItem.transform.position;
            
            yield return bee.CollectItem(worldItem, itemSO);
            worldItem.DestroySelf();

            uint amountCarrying = bee.GetItemStoredCount(itemSO);
            if (amountCarrying > 0) {
                yield return bee.DeliverItem(deliveryTarget, itemSO, amountCarrying);
            }

            isCompleted = true;
        }
    }

    public bool IsCompleted() => isCompleted;
    
    public bool AssignTo(BeeUnitBehaviour bee)
    {
        if (assignedBee == null) {
            assignedBee = bee;
            return true;
        }
        return false;
    }
    
    public bool IsAssigned() => assignedBee != null;
}
