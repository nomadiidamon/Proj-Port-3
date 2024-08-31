using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] Renderer model;
    [SerializeField] float flashTimer;

    Color colorOriginal;

    private void Start()
    {
        colorOriginal = model.material.color;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && gameManager.instance.playerSpawnPosition.transform.position != this.transform.position && GetComponent<CapsuleCollider>().bounds.Contains(gameManager.instance.player.transform.position))
        {
            gameManager.instance.playerSpawnPosition.transform.position = transform.position;
            StartCoroutine(flashModel());
        }
    }

    IEnumerator flashModel()
    {
        model.material.color = Color.red;
        gameManager.instance.checkPointMenu.gameObject.SetActive(true);
        yield return new WaitForSeconds(flashTimer);
        gameManager.instance.checkPointMenu.gameObject.SetActive(false);
        model.material.color = colorOriginal;
    }
}
