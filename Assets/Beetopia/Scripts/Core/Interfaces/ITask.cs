using System.Collections;
using UnityEngine;

public interface ITask {
    int Priority { get; }
    BeeUnitBehaviour assignedBee { get; set; }
    Vector3 GetTargetPosition();
    IEnumerator Execute(BeeUnitBehaviour bee);
    bool IsCompleted();
    public bool AssignTo(BeeUnitBehaviour bee);
    bool IsAssigned();
}