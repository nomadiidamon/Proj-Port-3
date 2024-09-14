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

    public GameObject menuActive;
    public GameObject menuPause;
    [SerializeField] public GameObject menuWin;
    [SerializeField] public GameObject menuLose;
    public GameObject menuSettings;

    

    public GameObject playerSpawnPosition;
    
    public GameObject flashDamageScreen;
    public GameObject underwaterOverlay;
    public GameObject restoreHealthScreen;
    public GameObject increaseDamageScreen;
    public GameObject raiseSpeedScreen;

    public GameObject checkPointMenu;
    public bool CheckpointReached;

    public Image playersHealthPool;
    [SerializeField] TMP_Text RespawnCount;
    [SerializeField] TMP_Text uiPrompt;


    public GameObject player;
    public int worldGravity;


    public playerController playerScript;
    public int respawns;
    int respawnsOriginal;
    public int GetOriginalRespawnCount() {  return respawnsOriginal; }

    public bool isPaused;

    int enemyCount;

    private GameObject currentDoor;

    void Awake()
    {
        instance = this;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponentInParent<playerController>();
        respawnsOriginal = respawns;
        updateRespawnCount(0);
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
            if (menuActive == null)
            {
                statePause();
                menuActive = menuPause;
                menuActive.SetActive(isPaused);

            }

            else if (menuActive == menuPause)
            {
                stateUnpause();
            }

            else if (menuActive == menuSettings)
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
        menuActive.SetActive(isPaused);
        menuActive = null;
        

    }

    public void updateRespawnCount(int amount)
    {
        respawns += amount;
        RespawnCount.text = respawns.ToString("F0");
    }

    public void youLose()
    {
        Debug.Log("You Lose");
        if (isPaused)
        {
            isPaused = !isPaused;
            menuActive = null;
        }
        statePause();
        menuActive = menuLose;
        menuActive.SetActive(isPaused);

    }

    public void UpdateUIPrompt(string message, GameObject door)
    {
        currentDoor = door;
        uiPrompt.text = message;
        uiPrompt.enabled = !string.IsNullOrEmpty(message);
    }

    public void ClearUIPrompt(GameObject door) 
    {
    
        if (currentDoor == door)
        {

            uiPrompt.enabled = false;
            currentDoor = null;

        }

    
    }

    public bool IsUIPromptActive()
    {

        return uiPrompt.enabled;


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
