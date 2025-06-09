using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour, IProviderHandler {
    public List<Quest> AllQuests = new();
    public int currentQuestIndex = 0;

    public Quest CurrentQuest => AllQuests[currentQuestIndex];

    public void ProgressQuest(ItemSO item, uint amount) {
        var quest = CurrentQuest;
        if (!quest.IsCompleted) {
            quest.Progress(item, amount);
            if (quest.IsCompleted) {
                GrantReward(quest);
                AdvanceToNextQuest();
            }
        }
    }

    private void GrantReward(Quest quest) {
        G.DataManager.AddCoins(quest.rewardCoins);
        if (quest.rewardItemList != null)
            foreach (var rewardItem in quest.rewardItemList) {
                G.DataManager.TryStoreItem(rewardItem.item, (uint)rewardItem.amount);
            }
    }

    private void AdvanceToNextQuest() {
        if (currentQuestIndex + 1 < AllQuests.Count)
            currentQuestIndex++;
        else
            Debug.Log("All quests are completed");
    }
}
