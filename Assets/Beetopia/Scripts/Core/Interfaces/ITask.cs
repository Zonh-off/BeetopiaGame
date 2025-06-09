using System.Collections;
using UnityEngine;

public interface ITask {
    Vector3 GetTargetPosition();
    IEnumerator Execute(BeeUnitBehaviour workerBeeUnitBehaviour);
    bool IsCompleted(); 
    int Priority { get; }
}