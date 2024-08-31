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

    Transform player;

    bool isNearDoor = false;
    bool isDoorActive = true;

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
                gameManager.instance.UpdateUIPrompt(promptMessage, gameObject);
            }
            if (Input.GetButtonDown(doorButton))
            {

                StartCoroutine(OpenDoor());
            }
        
        }

        else if (isNearDoor)
        {
            isNearDoor = false;
            gameManager.instance.ClearUIPrompt(gameObject);



        }

    }

    IEnumerator OpenDoor()
    {
        isDoorActive = false;
        door.SetActive(false);
        gameManager.instance.ClearUIPrompt(gameObject);
        yield return new WaitForSeconds(doorAppearTime);
        door.SetActive(true);
        isDoorActive = true;

        float distance = Vector3.Distance(player.position, door.transform.position);
        if (distance <= doorDistance)
        {

            gameManager.instance.UpdateUIPrompt(promptMessage, gameObject);

        }
        
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, doorDistance);
    }

}
