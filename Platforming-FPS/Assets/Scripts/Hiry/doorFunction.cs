using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class doorFunction : MonoBehaviour
{

    [SerializeField] GameObject door;
    [SerializeField] float doorAppearTime;
    [SerializeField] float doorDistance;
    [SerializeField] string doorButton = "Interact";
    [SerializeField] string promptMessage = "Press E to Open";
    [SerializeField] bool isBossDoor;
    [SerializeField] GameObject boss;
    [SerializeField] Transform playerMovePos;

    Transform player;

    bool isNearDoor = false;
    bool isDoorActive = true;
    bool isDoorLocked = false;

    // Start is called before the first frame update
    void Start()
    {

        player = GameObject.FindWithTag("Player").transform;

    }

    // Update is called once per frame
    void Update()
    {
        
        float distance = Vector3.Distance(player.position, door.transform.position);
        if (distance <= doorDistance && isDoorActive) 
        {
            if (!isNearDoor)
            {
                isNearDoor = true;
                if (!isDoorLocked)
                {
                    gameUIManager.instance.UpdateUIPrompt(promptMessage, gameObject);

                }
            }
            if (Input.GetButtonDown(doorButton))
            {
                if (!isBossDoor)
                {
                    StartCoroutine(OpenDoor());
                }
                if (isBossDoor)
                {
                    if (!isDoorLocked)
                    {
                        gameManager.instance.playerScript.GetComponent<CharacterController>().enabled = false;
                        door.GetComponent<BoxCollider>().enabled = false;
                        gameManager.instance.playerScript.transform.position =
                            Vector3.Lerp(gameManager.instance.playerScript.transform.position, playerMovePos.position, Time.time);
                        gameManager.instance.playerScript.GetComponent<CharacterController>().enabled = true;
                        door.GetComponent<BoxCollider>().enabled = true;
                        isDoorLocked = true;


                    }

                }

            }
        }
        if (boss.GetComponent<bossGolem>() != null)
        {
            if (boss.GetComponent<bossGolem>().GetCurrentHealth() <= 0)
            {
                Destroy(gameObject);

            }
        }

        else if (isNearDoor)
        {
            isNearDoor = false;
            gameUIManager.instance.ClearUIPrompt(gameObject);



        }



    }

    IEnumerator OpenDoor()
    {
        isDoorActive = false;
        door.SetActive(false);
        gameUIManager.instance.ClearUIPrompt(gameObject);
        yield return new WaitForSeconds(doorAppearTime);
        door.SetActive(true);
        isDoorActive = true;

        float distance = Vector3.Distance(player.position, door.transform.position);
        if (distance <= doorDistance)
        {

            gameUIManager.instance.UpdateUIPrompt(promptMessage, gameObject);

        }
        
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, doorDistance);
    }

}
