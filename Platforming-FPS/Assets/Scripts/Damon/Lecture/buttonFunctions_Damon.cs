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
        gameManager.instance.CheckpointReached = false;
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

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }

    public void openSettings()
    {

        
        

            gameManager.instance.menuPause.SetActive(false);

            gameManager.instance.menuActive = gameManager.instance.menuSettings;
            gameManager.instance.menuSettings.SetActive(true);
        
        
            
        
    }

    public void closeSettings()
    {
        gameManager.instance.menuSettings.SetActive(false);
        gameManager.instance.menuActive = gameManager.instance.menuPause;
        gameManager.instance.menuPause.SetActive(true);
    }



}
