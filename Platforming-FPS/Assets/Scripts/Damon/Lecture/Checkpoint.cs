using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class Checkpoint : MonoBehaviour
{
    [SerializeField] Renderer model;
    [SerializeField] float flashTimer;
    [SerializeField] float activatedFlashSpeed;
    public bool CheckpointReached;

    Color colorOriginal;

    private void Start()
    {
        colorOriginal = model.material.color;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (GetComponent<CapsuleCollider>().bounds.Contains(gameManager.instance.player.transform.position) && CheckpointReached)
        {
            gameUIManager.instance.checkpointMenu.gameObject.SetActive(true);
        }
        else
        {
            if (other.CompareTag("Player") && gameManager.instance.playerSpawnPosition.transform.position != this.transform.position && GetComponent<CapsuleCollider>().bounds.Contains(gameManager.instance.player.transform.position))
            {
                gameManager.instance.playerSpawnPosition.transform.position = transform.position;

                if (gameObject.GetComponent<flashColor>() == null && gameObject.GetComponent<scrollUV>() == null)
                {
                    StartCoroutine(flashModel());
                    StartCoroutine(showCheckpointReachedMessage());
                }
                else
                {
                    gameObject.GetComponent<flashColor>().flashSpeed = activatedFlashSpeed;
                    gameObject.GetComponent<scrollUV>().horizontal = false;
                    gameObject.GetComponent<scrollUV>().vertical = true;
                    gameObject.GetComponent<scrollUV>().scrollSpeedY = -0.015f;
                    StartCoroutine(showCheckpointReachedMessage());
                }

                if (GetComponent<CapsuleCollider>().bounds.Contains(gameManager.instance.player.transform.position) && CheckpointReached)
                {
                    gameUIManager.instance.checkpointMenu.gameObject.SetActive(true);

                }
            }
        }
    }

    private void OnTriggerExit()
    {
        if (GetComponent<CapsuleCollider>().bounds!.Contains(gameManager.instance.player.transform.position) && CheckpointReached)
        {
            gameUIManager.instance.checkpointMenu.gameObject.SetActive(false);

        }
    }

    IEnumerator flashModel()
    {
        model.material.color = Color.red;
        gameUIManager.instance.checkpointReachedMessage.gameObject.SetActive(true);
        yield return new WaitForSeconds(flashTimer);
        gameUIManager.instance.checkpointReachedMessage.gameObject.SetActive(false);
        model.material.color = colorOriginal;
    }

    IEnumerator showCheckpointReachedMessage()
    {
        CheckpointReached = true;
        gameUIManager.instance.checkpointReachedMessage.SetActive(true);
        yield return new WaitForSeconds(flashTimer);
        gameUIManager.instance.checkpointReachedMessage.SetActive(false);

    }
}
