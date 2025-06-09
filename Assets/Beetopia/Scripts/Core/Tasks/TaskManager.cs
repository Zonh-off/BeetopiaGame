using System.Collections.Generic;
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

        if (beeUnitBehaviour.IsGatheringMode) {
            foreach (var queue in taskQueue.Values) {
                foreach (var task in queue) {
                    if (task is CollectWorldItemTask) {
                        queue.Dequeue();
                        return task;
                    }
                }
            }
        }
        
        if (beeUnitBehaviour.GatheredItemsCount >= 1) {
            foreach (var queue in taskQueue.Values) {
                foreach (var task in queue) {
                    if (task is DeliverItemTask) {
                        queue.Dequeue();
                        return task;
                    }
                }
            }
        }
        
        foreach (var kvp in taskQueue) {
            if (kvp.Value.Count > 0) {
                return kvp.Value.Dequeue();
            }
        }

        return null;
    }
}