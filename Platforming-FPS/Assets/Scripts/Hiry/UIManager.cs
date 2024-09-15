using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{

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




    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
