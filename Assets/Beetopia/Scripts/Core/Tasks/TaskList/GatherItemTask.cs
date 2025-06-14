using System.Collections;
using UnityEngine;

public class GatherItemTask : ITask {
    public int Priority => 3;
    public BeeUnitBehaviour assignedBee { get; set; }

    private BasePlacedObject targetObject;
    private bool isCompleted = false;

    public GatherItemTask(BasePlacedObject targetObject) {
        this.targetObject = targetObject;
    }

    public Vector3 GetTargetPosition() => targetObject.transform.position;

    public IEnumerator Execute(BeeUnitBehaviour bee) {
        yield return bee.MoveTo(targetObject.transform.position);
        yield return bee.PlayActionAnimation();

        var targetComponent = targetObject.GetComponent<IItemStorage>();
        if (targetComponent != null && targetComponent is IHarvest harvest && harvest.HasHarvest())
        {
            if (targetComponent.TryGetStoredItem(new[] { G.GameAssets.itemSO_Refs.any }, out var item))
            {
                if (bee.TryStoreItem(item))
                {
                    var deliveryTarget = Object.FindFirstObjectByType<StorageObject>();
                    yield return bee.DeliverItem(deliveryTarget, item, 1);

                    isCompleted = true;
                }
                else
                {
                    Debug.LogWarning("[Bee] Failed to store nectar – inventory full?");
                }
            }
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