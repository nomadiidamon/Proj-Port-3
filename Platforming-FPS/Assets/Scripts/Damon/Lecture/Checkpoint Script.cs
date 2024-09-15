using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointScript : MonoBehaviour
{
    [SerializeField] Transform respawnPoint;
    [SerializeField] Collider area;
    Transform player;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        gameUIManager.instance.CheckpointReached = false;
    }

    // Update is called once per frame
    void Update()
    {

        OnTriggerEnter(gameManager.instance.player.GetComponent<Collider>());

    }

    private void OnTriggerEnter(Collider other)
    {
        if (area.bounds.Contains(player.position))
        {
            Debug.Log("Checkpoint Reached!");
            gameUIManager.instance.CheckpointReached = true;
        }

    }
}
