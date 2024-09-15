using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class gameUIManager : MonoBehaviour
{
    public static gameUIManager instance;

    public GameObject menuActive;
    public GameObject menuPause;
    [SerializeField] public GameObject menuWin;
    [SerializeField] public GameObject menuLose;
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



    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
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
