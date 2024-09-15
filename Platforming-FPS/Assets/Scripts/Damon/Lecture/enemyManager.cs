using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class enemyManager : MonoBehaviour
{

    UIManager uiManager;

    public static enemyManager instance;

    public int enemiesOriginal;
    public int enemiesRemaining;

    [SerializeField] TMP_Text enemyCountText;

    public GameObject player;
    public Vector3 playerPosition;

    void Awake()
    {
        instance = this;
        player = GameObject.FindWithTag("Player");
        playerPosition = player.transform.position;

    }

    void Update()
    {
        playerPosition = player.transform.position;
    }

    public void updateEnemyCount(int amount)
    {
        enemiesRemaining += amount;

        /// needs to be passed to a UI manager
        enemyCountText.text = enemiesRemaining.ToString("F0");
        ///


        if (enemiesRemaining <= 0)
        {
            Debug.Log("You Win");

            // you win!
            gameManager.instance.statePause();
            uiManager.menuActive = uiManager.menuWin;
            uiManager.menuActive.SetActive(gameManager.instance.isPaused);

        }
    }



}
