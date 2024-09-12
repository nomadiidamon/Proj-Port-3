using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    [SerializeField] int gravity;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.instance.playerScript.SetGravity(gravity, 5);
            gameManager.instance.playerScript.isSwimming = true;
            gameManager.instance.underwaterOverlay.SetActive(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.instance.playerScript.SetGravity(gameManager.instance.worldGravity);
            gameManager.instance.playerScript.isSwimming = false;
            gameManager.instance.underwaterOverlay.SetActive(false);
        }
    }
}
