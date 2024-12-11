using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTeleport : MonoBehaviour
{
    [SerializeField] private string sceneName;
    [SerializeField] enemyManager enemyManager;
    [SerializeField] GameObject spawnPrefab;
    [SerializeField] Transform spawnTeleLocation;
    public AsyncLoader loader;

    private bool hasSpawned = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            if (other.gameObject.CompareTag("PlayerFeet"))
            {
                SceneManager.LoadScene(sceneName);
            }
        }
    }

    private void Update()
    {
        if (enemyManager != null && !hasSpawned)
        {
            if (enemyManager.enemiesRemaining <= 0)
            {
                Instantiate(spawnPrefab, spawnTeleLocation.position, spawnTeleLocation.rotation);
                hasSpawned = true;
            }
        }
    }
}
