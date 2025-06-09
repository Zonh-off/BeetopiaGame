using System.Collections.Generic;
using UnityEngine;

public enum QuestType {
    GatherNectar,
    ConvertNectar,
    DeliverItem,
}

[System.Serializable]
public class Quest {
    public string title;
    public string description;

    public QuestType questType;
    public List<QuestItem> targetItemList;

    public uint rewardCoins;
    public List<RewardQuestItem> rewardItemList;

    [System.Serializable]
    public class QuestItem {
        public ItemSO item;
        public int requiredAmount;
        public int currentAmount;
    }

    [System.Serializable]
    public class RewardQuestItem {
        public ItemSO item;
        public int amount;
    }

    public bool IsCompleted {
        get {
            bool isCompleted = false;
            foreach (var targetItem in targetItemList) {
                if (targetItem.currentAmount >= targetItem.requiredAmount) {
                    isCompleted = true;
                }
            }

            return isCompleted;
        }
    }

    public void Progress(ItemSO item, uint amount) {
        foreach (var targetItem in targetItemList) {
            if (ItemSO.IsItemSOInFilter(targetItem.item, new ItemSO[] { item })) {
                if (targetItem.currentAmount <= targetItem.requiredAmount) {
                    targetItem.currentAmount += (int)amount;
                    targetItem.currentAmount = Mathf.Min(targetItem.currentAmount, targetItem.requiredAmount);
                }
            }
        }
    }
}