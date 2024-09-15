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

    public bool isPaused;

    int enemyCount;

    public GameObject currentDoor;

    void Awake()
    {
        instance = this;
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
        if (Input.GetButtonDown("Cancel"))
        {
            if (gameUIManager.instance.menuActive == null)
            {
                statePause();
                gameUIManager.instance.menuActive = gameUIManager.instance.menuPause;
                gameUIManager.instance.menuActive.SetActive(isPaused);

            }

            else if (gameUIManager.instance.menuActive == gameUIManager.instance.menuPause)
            {
                stateUnpause();
            }

            else if (gameUIManager.instance.menuActive == gameUIManager.instance.menuSettings)
            {

                stateUnpause();


            }


        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

        
        if (isPaused)
        {
            stateUnpause();
        }

    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void statePause()
    {
        Debug.Log("Paused");
        isPaused = !isPaused;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void stateUnpause()
    {
       
        
        Debug.Log("Unpaused");
        isPaused = !isPaused;
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        gameUIManager.instance.menuActive.SetActive(isPaused);
        gameUIManager.instance.menuActive = null;
        

    }

    

    public void youLose()
    {
        Debug.Log("You Lose");
        if (isPaused)
        {
            isPaused = !isPaused;
            gameUIManager.instance.menuActive = null;
        }
        statePause();
        gameUIManager.instance.menuActive = gameUIManager.instance.menuLose;
        gameUIManager.instance.menuActive.SetActive(isPaused);

    }

    

    

    /*public void savePlayerData()
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

        playerController playerScript = GetComponent<playerController>(); 
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

    private void OnApplicationQuit()
    {
        savePlayerData();
    }
    */


}
