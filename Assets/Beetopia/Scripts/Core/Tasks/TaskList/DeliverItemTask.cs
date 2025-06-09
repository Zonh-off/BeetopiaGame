using System.Collections;
using UnityEngine;

public class DeliverItemTask : ITask {
    public int Priority => 2;
    
    private BasePlacedObject collectTargetObject;
    private BasePlacedObject deliveryTargetObject;
    private bool isCompleted = false;
    private ItemSO itemSO;
    private uint amountRequired; 

    public DeliverItemTask(BasePlacedObject collectTargetObject, BasePlacedObject deliveryTargetObject, ItemSO itemSO, uint amountRequired) {
        this.deliveryTargetObject = deliveryTargetObject;
        this.collectTargetObject = collectTargetObject;
        this.itemSO = itemSO;
        this.amountRequired = amountRequired;
    }

    public Vector3 GetTargetPosition() => deliveryTargetObject.transform.position;

    public IEnumerator Execute(BeeUnitBehaviour workerBeeUnitBehaviour) {
        int amountDelivered = 0;

        while (amountDelivered < amountRequired) {
            int toCollect = Mathf.Min((int)workerBeeUnitBehaviour.GetCarryCapacity(), (int)(amountRequired - amountDelivered));
            if (toCollect <= 0) break;

            uint collected = workerBeeUnitBehaviour.LastCollectedAmount;
            
            yield return workerBeeUnitBehaviour.CollectItem(collectTargetObject, itemSO, (uint)toCollect);

            if (collected == 0) {
                Debug.LogWarning("BeeUnitBehaviour failed to collect anything.");
                break;
            }

            yield return workerBeeUnitBehaviour.DeliverItem(deliveryTargetObject, itemSO, collected);

            amountDelivered += (int)collected;
        }

        isCompleted = amountDelivered >= amountRequired;
    }



    public bool IsCompleted() => isCompleted;
}