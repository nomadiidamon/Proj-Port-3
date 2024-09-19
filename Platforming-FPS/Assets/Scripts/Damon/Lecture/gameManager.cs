using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;
    public QuestTracker questTracker;

    public GameObject playerSpawnPosition;

    public GameObject player;
    public int worldGravity;
    public playerController playerScript;
    public int respawns;
    int respawnsOriginal;
    public int GetOriginalRespawnCount() {  return respawnsOriginal; }


    int enemyCount;

    public GameObject currentDoor;



    void Awake()
    {
        instance = this;
        Time.timeScale = 1;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponentInParent<playerController>();
        respawnsOriginal = respawns;
        gameUIManager.instance.updateRespawnCount(respawns);
        playerScript.updatePlayerUI();
        playerSpawnPosition = GameObject.FindWithTag("Player Spawn Position");
        worldGravity = instance.playerScript.GetGravity();                          // setting resting gravity of the world

        questTracker = new QuestTracker();

        Quest questTest = new Quest(1337);
        questTracker.AddQuest(questTest);
    }

    

    public void CompleteQuest(int questId)
    {
        questTracker.CompleteQuest(questId);
    }

    public void AbandonQuest(int questId)
    {
        questTracker.AbandonQuest(questId);
    }

    public int GetCompleteQuestCount()
    {
        return questTracker.GetCompletedQuestCount();
    }

    public bool IsQuestComplete(int questId)
    {
        return questTracker.IsQuestCompleted(questId); 
    }

    void Update()
    {

    }



    

    

    public void savePlayerData()
    {

        playerController playerScript = GetComponent<playerController>();
        if (player != null)
        {
            playerData data = new playerData(playerScript, respawns); 
            data.savePlayer(); 
        }
        else
        {
            Debug.LogError("playerController not found on player");
        }
    }

    public IEnumerator loadPlayerData()
    {

        yield return new WaitForEndOfFrame();

        playerController playerScript = player.GetComponent<playerController>(); 
        if (playerScript != null)
        {
            playerData data = new playerData(playerScript, respawns); 
            data.loadPlayer(playerScript); 
        }
        else
        {
            Debug.LogError("playerController not found on playerGameObject.");
        }
      }



    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(loadPlayerData());
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnApplicationQuit()
    {
        if (player != null)
        {
            savePlayerData();
        }
        else
        {
            Debug.LogWarning("Player was null on application quit, skipping save.");
        }
    }
    


}
