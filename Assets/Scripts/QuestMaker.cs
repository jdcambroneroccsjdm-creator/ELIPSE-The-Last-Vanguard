
using UnityEngine;
using TMPro;

[System.Serializable]
public class QuestData
{
    [Header("Quest Settings")]
    public string questName = "Defeat Enemies";
    public int requiredKills = 5;

    [Header("Separate Barrier")]
    public GameObject questBarrier;
}

public class QuestMaker : MonoBehaviour
{
    [Header("Quest List")]
    public QuestData[] quests;

    [Header("Quest UI")]
    public TMP_Text questText;

    private int currentQuestIndex = 0;
    private int currentKills = 0;
    private bool allQuestsCompleted = false;

    private void Start()
    {
        // Disable all barriers first
        foreach (QuestData quest in quests)
        {
            if (quest.questBarrier != null)
            {
                quest.questBarrier.SetActive(false);
            }
        }

        if (quests.Length > 0)
        {
            StartCurrentQuest();
        }
        else
        {
            allQuestsCompleted = true;

            if (questText != null)
                questText.text = "NO QUESTS AVAILABLE!";
        }
    }

    public void AddKill()
    {
        if (allQuestsCompleted) return;

        currentKills++;

        CheckQuest();

        if (!allQuestsCompleted)
            UpdateQuestUI();

        Debug.Log("Quest Progress: " +
            currentKills + "/" +
            quests[currentQuestIndex].requiredKills);
    }

    private void CheckQuest()
    {
        if (currentKills >= quests[currentQuestIndex].requiredKills)
        {
            Debug.Log("Quest Completed: " +
                quests[currentQuestIndex].questName);

            // Disable completed quest barrier
            if (quests[currentQuestIndex].questBarrier != null)
            {
                quests[currentQuestIndex].questBarrier.SetActive(false);
            }

            // Move to next quest
            currentQuestIndex++;

            if (currentQuestIndex >= quests.Length)
            {
                allQuestsCompleted = true;

                if (questText != null)
                    questText.text = "ALL QUESTS COMPLETED!";

                Debug.Log("All Quests Completed!");
                return;
            }

            // Reset kills for next quest
            currentKills = 0;

            StartCurrentQuest();
        }
    }

    private void StartCurrentQuest()
    {
        QuestData quest = quests[currentQuestIndex];

        // Activate only the current quest barrier
        if (quest.questBarrier != null)
        {
            quest.questBarrier.SetActive(true);
        }

        UpdateQuestUI();

        Debug.Log("Started Quest: " + quest.questName);
    }

    private void UpdateQuestUI()
    {
        if (questText == null || allQuestsCompleted) return;

        QuestData quest = quests[currentQuestIndex];

        questText.text = quest.questName + ": " +
            currentKills + "/" + quest.requiredKills;
    }

    public void CompleteQuest()
    {
        if (allQuestsCompleted) return;

        currentKills = quests[currentQuestIndex].requiredKills;

        CheckQuest();

        if (!allQuestsCompleted)
            UpdateQuestUI();
    }
}
