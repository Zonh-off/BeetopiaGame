using System.Collections;
using UnityEngine;

public class CollectWorldItemTask : ITask {
    public int Priority => 3;

    private WorldItem worldItem;
    private Vector3 worldItemPos;
    private bool isCompleted = false;

    public CollectWorldItemTask(WorldItem targetWorldItem) {
        worldItem = targetWorldItem;
    }

    public Vector3 GetTargetPosition() => worldItemPos;

    public IEnumerator Execute(BeeUnitBehaviour workerBeeUnitBehaviour) {
        if (worldItem.GetItemSO() != null) {
            var itemSO = worldItem.GetItemSO();
            var deliveryTarget = Object.FindFirstObjectByType<StorageObject>();
            
            worldItemPos = worldItem.transform.position;
            
            yield return workerBeeUnitBehaviour.CollectItem(worldItem, itemSO);
            worldItem.DestroySelf();

            uint amountCarrying = workerBeeUnitBehaviour.GetItemStoredCount(itemSO);
            if (amountCarrying > 0) {
                yield return workerBeeUnitBehaviour.DeliverItem(deliveryTarget, itemSO, amountCarrying);
            }

            isCompleted = true;
        }
    }

    public bool IsCompleted() => isCompleted;
}
