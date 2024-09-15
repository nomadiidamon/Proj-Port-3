using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] Renderer model;
    [SerializeField] float flashTimer;
    [SerializeField] float activatedFlashSpeed;

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

            if (gameObject.GetComponent<flashColor>() == null && gameObject.GetComponent<scrollUV>() == null)
            {
                StartCoroutine(flashModel());
                StartCoroutine(showCheckPointMenu());
            }
            else
            {
                gameObject.GetComponent<flashColor>().flashSpeed = activatedFlashSpeed;
                gameObject.GetComponent<scrollUV>().horizontal = false;
                gameObject.GetComponent<scrollUV>().vertical = true;
                gameObject.GetComponent<scrollUV>().scrollSpeedY = -0.015f;
                StartCoroutine(showCheckPointMenu());
            }
        }
    }

    IEnumerator flashModel()
    {
        model.material.color = Color.red;
        gameUIManager.instance.checkPointMenu.gameObject.SetActive(true);
        yield return new WaitForSeconds(flashTimer);
        gameUIManager.instance.checkPointMenu.gameObject.SetActive(false);
        model.material.color = colorOriginal;
    }

    IEnumerator showCheckPointMenu()
    {
        gameUIManager.instance.CheckpointReached = true;
        gameUIManager.instance.checkPointMenu.SetActive(true);
        yield return new WaitForSeconds(flashTimer);
        gameUIManager.instance.checkPointMenu.SetActive(false);

    }
}
