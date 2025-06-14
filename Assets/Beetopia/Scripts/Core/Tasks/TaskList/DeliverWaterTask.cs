using System.Collections;
using UnityEngine;

public class DeliverWaterTask : ITask {
    public int Priority => 5;
    public BeeUnitBehaviour assignedBee { get; set; }

    private BasePlacedObject targetObject;
    private BasePlacedObject waterWellObject;
    private bool isCompleted = false;

    public DeliverWaterTask(BasePlacedObject targetObject) {
        this.targetObject = targetObject;
    }

    public Vector3 GetTargetPosition() => targetObject.transform.position;

    public IEnumerator Execute(BeeUnitBehaviour bee) {
        var targetComponent = targetObject.GetComponent<IHarvest>();
        waterWellObject = Object.FindFirstObjectByType<WaterWellObject>();

        if (!targetComponent.HasWater() && waterWellObject != null) {
            yield return bee.MoveTo(waterWellObject.transform.position);
            
            yield return bee.MoveTo(GetTargetPosition());

            bool isPoured = targetComponent.PourWater();
            if (isPoured) {
                isCompleted = true;
            }
        } else {
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