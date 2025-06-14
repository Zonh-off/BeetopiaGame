using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TaskManager : MonoBehaviour, IProviderHandler {
    private SortedDictionary<int, Queue<ITask>> taskQueue = new();

    public void AddTask(ITask task) {
        int priority = task.Priority;
        
        if (!taskQueue.ContainsKey(priority)) {
            taskQueue[priority] = new Queue<ITask>();
        }
        taskQueue[priority].Enqueue(task);

        Debug.Log(task.ToString());
    }
    
    public ITask GetNextTask(BeeUnitBehaviour beeUnitBehaviour) {
        if (taskQueue.Count == 0) return null;

        foreach (var queue in taskQueue.Values)
        {
            foreach (var task in queue.Where(task => !task.IsAssigned()))
            {
                if (beeUnitBehaviour.IsGatheringMode && task is CollectWorldItemTask && task.AssignTo(beeUnitBehaviour)) {
                    queue.Dequeue();
                    return task;
                }

                if (beeUnitBehaviour.GatheredItemsCount >= 1 && task is DeliverItemTask && task.AssignTo(beeUnitBehaviour)) {
                    queue.Dequeue();
                    return task;
                }

                if (task.AssignTo(beeUnitBehaviour)) {
                    queue.Dequeue();
                    return task;
                }
            }
        }

        return null;
    }
}