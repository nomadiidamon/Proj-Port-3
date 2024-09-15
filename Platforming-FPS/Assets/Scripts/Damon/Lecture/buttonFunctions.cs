using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonFunctions : MonoBehaviour
{

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
        gameUIManager.instance.CheckpointReached = false;
        gameManager.instance.respawns = gameManager.instance.GetOriginalRespawnCount();
    }

    public void respawn()
    {
        //if (!gameManager.instance.CheckpointReached || gameManager.instance.respawns == 0) { return; }
        if (gameManager.instance.respawns == 0)
        {
            return;
        }
        
        gameUIManager.instance.updateRespawnCount(-1);
        Debug.Log("Second Chance!");
        gameManager.instance.playerScript.spawnPlayer();
        gameManager.instance.stateUnpause();

    }

    public void quit()
    {
        Debug.Log("Quitting");

        if (gameUIManager.instance.menuActive == gameUIManager.instance.menuLose)
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




        gameUIManager.instance.menuPause.SetActive(false);

        gameUIManager.instance.menuActive = gameUIManager.instance.menuSettings;
        gameUIManager.instance.menuSettings.SetActive(true);
        
        
            
        
    }

    

    public void closeSettings()
    {
        gameUIManager.instance.menuSettings.SetActive(false);
        gameUIManager.instance.menuActive = gameUIManager.instance.menuPause;
        gameUIManager.instance.menuPause.SetActive(true);
    }

    public void loadMainMenu()
    {
        
        SceneManager.LoadScene("Main Menu");
    }



}
