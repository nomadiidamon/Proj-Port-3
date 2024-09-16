using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class gameUIManager : MonoBehaviour
{
    public static gameUIManager instance;

    public GameObject menuActive;
    public GameObject menuPause;
    [SerializeField] public GameObject menuWin;
    [SerializeField] public GameObject menuLose;
    [SerializeField] public GameObject CreatorGunPrompt;

    public GameObject menuSettings;
    public GameObject checkPointMenu;
    public bool CheckpointReached;
    public Image playersHealthPool;
    public TMP_Text RespawnCount;
    public TMP_Text uiPrompt;


    public GameObject flashDamageScreen;
    public GameObject restoreHealthScreen;
    public GameObject increaseDamageScreen;
    public GameObject raiseSpeedScreen;
    public GameObject underwaterOverlay;
    playerController player;

    public bool isPaused;


    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    private void Update()
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
        isPaused = false;
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


    public void updateRespawnCount(int amount)
    {
        gameManager.instance.respawns += amount;
        gameUIManager.instance.RespawnCount.text = gameManager.instance.respawns.ToString("F0");
    }

    public void updatePlayerUI(float HP, float HPOrig)
    {
        playersHealthPool.fillAmount = HP / HPOrig;
    }

    public void UpdateUIPrompt(string message, GameObject door)
    {
        gameManager.instance.currentDoor = door;
        uiPrompt.text = message;
        uiPrompt.enabled = !string.IsNullOrEmpty(message);
    }

    public void ShowCreatorGunPrompt()
    {
        CreatorGunPrompt.SetActive(true);
    }

    public void HideCreatorGunPrompt()
    {
        CreatorGunPrompt.SetActive(false);
    }

    public void ClearUIPrompt(GameObject door)
    {

        if (gameManager.instance.currentDoor == door)
        {

            uiPrompt.enabled = false;
            gameManager.instance.currentDoor = null;

        }


    }

    public bool IsUIPromptActive()
    {

        return uiPrompt.enabled;


    }


}
