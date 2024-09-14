using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Quest
{
    public int questID { get; private set; }
    public bool isComplete { get; private set; }
    public bool isAbandoned { get; private set; }

    public Quest(int id)
    {
        questID = id;
        isComplete = false;
        isAbandoned = false;
    }

    public void CompleteQuest()
    {
        isComplete = true;
    }

    public void AbandonQuest()
    {
        isAbandoned = true;
    }
}

public class QuestTracker
{
    private Dictionary<int, Quest> activeQuests = new Dictionary<int, Quest>();
    private Dictionary<int, Quest> completedQuests = new Dictionary<int, Quest>();
    private Dictionary<int, Quest> abandonedQuests = new Dictionary<int, Quest>();

    private int completedQuestCount = 0;

 
    public void AddQuest(Quest quest)
    {
        if (!activeQuests.ContainsKey(quest.questID))
        {
            activeQuests.Add(quest.questID, quest);
        }
    }

    
    public void CompleteQuest(int questID)
    {
        if (activeQuests.TryGetValue(questID, out Quest quest) && !quest.isComplete)
        {
            quest.CompleteQuest();
            activeQuests.Remove(questID);
            completedQuests.Add(questID, quest);
            completedQuestCount++; 
        }
    }


    public void AbandonQuest(int questID)
    {
        if (activeQuests.TryGetValue(questID, out Quest quest) && !quest.isAbandoned)
        {
            quest.AbandonQuest();
            activeQuests.Remove(questID);
            abandonedQuests.Add(questID, quest);
        }
    }

  
    public void ClearCompleted()
    {
        completedQuests.Clear();
        completedQuestCount = 0;
    }


    public Quest GetCompletedQuest(int questID)
    {
        return completedQuests.TryGetValue(questID, out Quest quest) ? quest : null;
    }

  
    public bool IsQuestCompleted(int questID)
    {
        return completedQuests.ContainsKey(questID);
    }


    public int GetCompletedQuestCount()
    {
        return completedQuestCount;
    }
}

public static class EventManager
{
    public static event Action<int> OnQuestCompleted;

    public static void QuestCompleted(int questID)
    {
        OnQuestCompleted?.Invoke(questID);
        gameManager.instance.CompleteQuest(questID);
    }
}

public class QuestEventListener : MonoBehaviour
{
    public QuestTracker questTracker;

    void OnEnable()
    {
        EventManager.OnQuestCompleted += OnQuestCompleted;
    }

    void OnDisable()
    {
        EventManager.OnQuestCompleted -= OnQuestCompleted;
    }

    void OnQuestCompleted(int questID)
    {
        if (questTracker != null)
        {

            questTracker.CompleteQuest(questID);
        }
    }
}
