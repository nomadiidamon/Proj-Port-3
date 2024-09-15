using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonFunctions : MonoBehaviour
{
    UIManager uiManager;

    public void resume()
    {
        Debug.Log("Resumed");

        gameManager.instance.stateUnpause();
    }

    public void restart()
    {
        Debug.Log("Restarted");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        gameManager.instance.stateUnpause();
        uiManager.CheckpointReached = false;
        gameManager.instance.respawns = gameManager.instance.GetOriginalRespawnCount();
    }

    public void respawn()
    {
        //if (!gameManager.instance.CheckpointReached || gameManager.instance.respawns == 0) { return; }
        if (gameManager.instance.respawns == 0)
        {
            return;
        }
        
        gameManager.instance.updateRespawnCount(-1);
        Debug.Log("Second Chance!");
        gameManager.instance.playerScript.spawnPlayer();
        gameManager.instance.stateUnpause();

    }

    public void quit()
    {
        Debug.Log("Quitting");

        if (uiManager.menuActive == uiManager.menuLose)
        {
            gameManager.instance.playerScript.HP = gameManager.instance.playerScript.HPOrig;
            Debug.Log("Player's HP reset to original HP");
        }

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    //gameManager.instance.savePlayerData();
        Application.Quit();
#endif
    }

    public void openSettings()
    {

        
        

            uiManager.menuPause.SetActive(false);

            uiManager.menuActive = uiManager.menuSettings;
            uiManager.menuSettings.SetActive(true);
        
        
            
        
    }

    

    public void closeSettings()
    {
        uiManager.menuSettings.SetActive(false);
        uiManager.menuActive = uiManager.menuPause;
        uiManager.menuPause.SetActive(true);
    }

    public void loadMainMenu()
    {
        
        SceneManager.LoadScene("Main Menu");
    }



}
