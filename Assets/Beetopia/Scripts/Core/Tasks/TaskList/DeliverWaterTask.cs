using System.Collections;
using UnityEngine;

public class DeliverWaterTask : ITask {
    public int Priority => 5;
    
    private BasePlacedObject targetObject;
    private BasePlacedObject waterWellObject;
    private bool isCompleted = false;

    public DeliverWaterTask(BasePlacedObject targetObject) {
        this.targetObject = targetObject;
    }

    public Vector3 GetTargetPosition() => targetObject.transform.position;

    public IEnumerator Execute(BeeUnitBehaviour workerBeeUnitBehaviour) {
        var targetComponent = targetObject.GetComponent<IHarvest>();
        waterWellObject = Object.FindFirstObjectByType<WaterWellObject>();

        if (!targetComponent.HasWater() && waterWellObject != null) {
            yield return workerBeeUnitBehaviour.MoveTo(waterWellObject.transform.position);
            
            yield return workerBeeUnitBehaviour.MoveTo(GetTargetPosition());

            bool isPoured = targetComponent.PourWater();
            if (isPoured) {
                isCompleted = true;
            }
        } else {
            isCompleted = true;
        }
    }
    
    

    public bool IsCompleted() => isCompleted;
}