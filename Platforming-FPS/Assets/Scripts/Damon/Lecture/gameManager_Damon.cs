using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;

    [SerializeField] AudioSource audPlayer;

    public GameObject menuActive;
    public GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
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
    [SerializeField] TMP_Text enemyCountText;
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

    // Start is called before the first frame update
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
    }

    // Update is called once per frame
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

    public void updateGameGoal(int amount)
    {
        enemyCount += amount;
        enemyCountText.text = enemyCount.ToString("F0");

        if (enemyCount <= 0)
        {
            Debug.Log("You Win");

            // you win!
            statePause();
            menuActive = menuWin;
            menuActive.SetActive(isPaused);

        }
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


    public void PlayAud(AudioClip sound, float vol)
    {
        audPlayer.PlayOneShot(sound, vol);
    }

}
