using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp_Dylan : MonoBehaviour
{
    [SerializeField] enum pickUpType { health, speed, damage , equipment}              // types of pickups
    [SerializeField] pickUpType type;
    [SerializeField] Rigidbody rbPickup;                                   // rigid body needed for collision detection

    Color color = Color.white;           // default pick up color

    float rotationSpeed = 1f;
    Vector3 rotationAxis = Vector3.up;


    // Start is called before the first frame update
    void Start()
    {
        switch (type)
        {
            case pickUpType.health:
                {
                    color = Color.red;
                    rotationSpeed = 75f;
                    rotationAxis = Vector3.up;
                    break;
                }
            case pickUpType.speed:
                {
                    color = Color.blue;
                    rotationSpeed = 75f;
                    rotationAxis = Vector3.forward;
                    break;
                }
            case pickUpType.damage:
                {
                    color = Color.yellow;
                    rotationSpeed = 75f;
                    rotationAxis = Vector3.forward;
                    break;
                }
            case pickUpType.equipment:
                {
                    rotationSpeed = 75f;
                    rotationAxis = Vector3.up;
                    return;
                }
            default:
                break;
        }
        SetObjectColor(color);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime);

    }
    void SetObjectColor(Color color)
    {
        Renderer renderer = GetComponent<Renderer>();        // get access to the renderer on the pickup game object
        renderer.material.color = color;                     // set color to new color
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }
        SphereCollider coll = this.GetComponent<SphereCollider>();
        if (other.CompareTag("Player") && coll.bounds.Contains(gameManager.instance.player.transform.position))
        {
            switch (type)
            {
                case pickUpType.health:
                    {

                        if (gameManager.instance.playerScript.GetHealth() < gameManager.instance.playerScript.GetOriginalHpAmount())
                        {
                            gameManager.instance.playerScript.SetHealth(gameManager.instance.playerScript.GetHealth() + 1);
                            gameManager.instance.playerScript.updatePlayerUI();
                            Destroy(gameObject);
                        }
                        break;
                    }
                case pickUpType.speed:
                    {
                        
                        gameManager.instance.playerScript.SetSpeed(gameManager.instance.playerScript.GetSpeed() + 3);
                        gameManager.instance.playerScript.RaiseSpeed();
                        Destroy(gameObject);
                        break;
                    }
                case pickUpType.damage:
                    {
                        gameManager.instance.playerScript.SetDamage(gameManager.instance.playerScript.GetDamage() + 1);
                        //gameManager.instance.playerScript.GetComponent<Damage>().SetDamageAmount(gameManager.instance.playerScript.GetComponent<Damage>().GetDamageAmount() + 1);
                        gameManager.instance.playerScript.IncreaseDamage();
                        Destroy(gameObject);
                        break;
                    }
                case pickUpType.equipment:
                    {
                        return;
                    }
                default:
                    break;

            }
        }
    }
}