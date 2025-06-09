using System.Collections;
using UnityEngine;

public class GatherItemTask : ITask {
    public int Priority => 3;
    
    private BasePlacedObject targetObject;
    private bool isCompleted = false;

    public GatherItemTask(BasePlacedObject targetObject) {
        this.targetObject = targetObject;
    }

    public Vector3 GetTargetPosition() => targetObject.transform.position;

    public IEnumerator Execute(BeeUnitBehaviour workerBeeUnitBehaviour) {
        yield return new WaitForSeconds(0);
        
        var targetComponent = targetObject.GetComponent<IHarvest>();
        if (targetComponent.HasHarvest()) {
            
            targetComponent.CollectHarvest();
            isCompleted = true;
        }
    }

    public bool IsCompleted() => isCompleted;
}