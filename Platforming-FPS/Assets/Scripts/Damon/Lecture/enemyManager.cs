using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class enemyManager : MonoBehaviour
{


    public static enemyManager instance;

    public int enemiesOriginal;
    public int enemiesRemaining;

    [SerializeField] TMP_Text enemyCountText;
    
    public Transform playerPosition;

    void Awake()
    {
        instance = this;
        //playerPosition = gameManager.instance.playerScript.GetPlayerCenter();

    }

    void Update()
    {
        playerPosition = gameManager.instance.playerScript.GetPlayerCenter();
    }

    public void updateEnemyCount(int amount)
    {
        enemiesRemaining += amount;

        /// needs to be passed to a UI manager
        enemyCountText.text = enemiesRemaining.ToString("F0");
        ///


        youWin();

    }

    public void youWin()
    {
        if (enemiesRemaining <= 0)
        {
            Debug.Log("You Win");

            // you win!



            gameUIManager.instance.statePause();
            gameUIManager.instance.menuActive = gameUIManager.instance.menuWin;
            gameUIManager.instance.menuActive.SetActive(gameUIManager.instance.isPaused);

        }
    }



}
