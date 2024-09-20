using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonFunctions : MonoBehaviour
{

    public void resume()
    {
        Debug.Log("Resumed");

        gameUIManager.instance.stateUnpause();
    }

    public void restart()
    {
        Debug.Log("Restarted");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        gameUIManager.instance.stateUnpause();
        //gameUIManager.instance.CheckpointReached = false;
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
        gameUIManager.instance.stateUnpause();
    }

    public void quit()
    {
        Debug.Log("Quitting");

        /*if (gameUIManager.instance.menuActive == gameUIManager.instance.menuLose)
        {
            gameManager.instance.playerScript.HP = gameManager.instance.playerScript.HPOrig;
            Debug.Log("Player's HP reset to original HP");
        }*/

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    //gameManager.instance.savePlayerData();
        Application.Quit();
#endif
    }

    public void quitInLevel()
    {
        gameUIManager.instance.menuPause.SetActive(false);

        gameUIManager.instance.menuActive = gameUIManager.instance.menuAreYouSure;
        gameUIManager.instance.menuAreYouSure.SetActive(true);
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

    public void closeAreYouSure()
    {
        gameUIManager.instance.menuAreYouSure.SetActive(false);
        gameUIManager.instance.menuActive = gameUIManager.instance.menuPause;
        gameUIManager.instance.menuPause.SetActive(true);
    }

    public void loadMainMenu()
    {
        //gameManager.instance.savePlayerData();

        SceneManager.LoadScene("Main Menu");
    }

    public void increaseHealthStat()
    {
        if (gameUIManager.instance.upgradeStats.currPlayerExp >= gameUIManager.instance.upgradeStats.pointsForHealth)
        {
            gameUIManager.instance.upgradeStats.currPlayerExp -= gameUIManager.instance.upgradeStats.pointsForHealth;
            gameUIManager.instance.upgradeStats.HpUpgrades += 1;
            gameUIManager.instance.upgradeStats.pointsForHealth += 
                gameUIManager.instance.upgradeStats.pointsForHealth * gameUIManager.instance.upgradeStats.HpUpgrades;

            gameUIManager.instance.upgradeStats.healthIncreaseAmount += (int)(gameUIManager.instance.upgradeStats.maxHP * gameManager.instance.playerScript.upgradePercentage);
            gameUIManager.instance.upgradeStats.maxHP += (int)(gameUIManager.instance.upgradeStats.maxHP * gameManager.instance.playerScript.upgradePercentage);

            gameUIManager.instance.pointsNeededForHealth.text = gameUIManager.instance.upgradeStats.pointsForHealth.ToString("F0");
            gameUIManager.instance.currentHealth.text = gameUIManager.instance.upgradeStats.maxHP.ToString("F0");
        }
    }

    public void increaseSpeedStat()
    {
        if (gameUIManager.instance.upgradeStats.currPlayerExp >= gameUIManager.instance.upgradeStats.pointsForSpeed)
        {
            gameUIManager.instance.upgradeStats.currPlayerExp -= gameUIManager.instance.upgradeStats.pointsForSpeed;
            gameUIManager.instance.upgradeStats.speedUpgrades += 1;
            gameUIManager.instance.upgradeStats.pointsForSpeed +=
                gameUIManager.instance.upgradeStats.pointsForSpeed * gameUIManager.instance.upgradeStats.speedUpgrades;

            gameUIManager.instance.upgradeStats.maxSpeed += (int)(gameUIManager.instance.upgradeStats.maxSpeed * gameManager.instance.playerScript.upgradePercentage);

            gameUIManager.instance.pointsNeededForSpeed.text = gameUIManager.instance.upgradeStats.pointsForSpeed.ToString("F0");
            gameUIManager.instance.currentSpeed.text = gameUIManager.instance.upgradeStats.maxSpeed.ToString("F0");
        }
    }

    public void increaseDamageStat()
    {
        if (gameUIManager.instance.upgradeStats.currPlayerExp >= gameUIManager.instance.upgradeStats.pointsForDamage)
        {
            gameUIManager.instance.upgradeStats.currPlayerExp -= gameUIManager.instance.upgradeStats.pointsForDamage;
            gameUIManager.instance.upgradeStats.damageUpgrades += 1;
            gameUIManager.instance.upgradeStats.pointsForDamage +=
                gameUIManager.instance.upgradeStats.pointsForDamage * gameUIManager.instance.upgradeStats.damageUpgrades;

                gameUIManager.instance.upgradeStats.maxDamage++;


            gameUIManager.instance.pointsNeededForDamage.text = gameUIManager.instance.upgradeStats.pointsForDamage.ToString("F0");
            gameUIManager.instance.currentDamage.text = gameUIManager.instance.upgradeStats.maxDamage.ToString("F0");
        }
    }

    public void cancelUpgrades()
    {
        resume();
    }

    public void confirmUpgrades()
    {
        gameManager.instance.playerScript.currentExperience = gameUIManager.instance.upgradeStats.currPlayerExp;


        gameManager.instance.playerScript.HPOrig = gameUIManager.instance.upgradeStats.maxHP;
        gameManager.instance.playerScript.HP += gameUIManager.instance.upgradeStats.healthIncreaseAmount;
        gameManager.instance.playerScript.SetNumberOfHealthUpgrades(gameUIManager.instance.upgradeStats.HpUpgrades);
        gameManager.instance.playerScript.numberOfPointsToUpgradeHealth = gameUIManager.instance.upgradeStats.pointsForHealth;

        gameManager.instance.playerScript.speed = gameUIManager.instance.upgradeStats.maxSpeed;
        gameManager.instance.playerScript.SetNumberOfSpeedUpgrades(gameUIManager.instance.upgradeStats.speedUpgrades);
        gameManager.instance.playerScript.numberOfPointsToUpgradeSpeed = gameUIManager.instance.upgradeStats.pointsForSpeed;

        gameManager.instance.playerScript.baseDamage = gameUIManager.instance.upgradeStats.maxDamage;
        gameManager.instance.playerScript.SetNumberOfDamageUpgrades(gameUIManager.instance.upgradeStats.damageUpgrades);
        gameManager.instance.playerScript.numberOfPointsToUpgradeDamage = gameUIManager.instance.upgradeStats.pointsForDamage;




        gameManager.instance.playerScript.updatePlayerUI();

        resume();
    }

}
