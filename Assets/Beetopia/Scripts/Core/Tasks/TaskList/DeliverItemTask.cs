using System.Collections;
using UnityEngine;

public class DeliverItemTask : ITask {
    public int Priority => 2;
    public BeeUnitBehaviour assignedBee { get; set; }

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

    public IEnumerator Execute(BeeUnitBehaviour bee) {
        int amountDelivered = 0;

        while (amountDelivered < amountRequired) {
            int toCollect = Mathf.Min((int)bee.GetCarryCapacity(), (int)(amountRequired - amountDelivered));
            if (toCollect <= 0) break;

            uint collected = bee.LastCollectedAmount;
            
            yield return bee.CollectItem(collectTargetObject, itemSO, (uint)toCollect);

            if (collected == 0) {
                Debug.LogWarning("BeeUnitBehaviour failed to collect anything.");
                break;
            }

            yield return bee.DeliverItem(deliveryTargetObject, itemSO, collected);

            amountDelivered += (int)collected;
        }

        isCompleted = amountDelivered >= amountRequired;
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