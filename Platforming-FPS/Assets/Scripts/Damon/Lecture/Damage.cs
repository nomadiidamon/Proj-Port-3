using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UIElements;

public class Damage : MonoBehaviour
{
    [SerializeField] enum damageType { bullet, stationary, melee, enemyBullet }
    [SerializeField] damageType type;
    [SerializeField] Rigidbody rb;
    [SerializeField] ParticleSystem targetHitEffect;

    [Header("-----Audio-----")]
    [SerializeField] AudioClip[] damageMissAud;
    [SerializeField] AudioClip[] damageHitAud;

    [SerializeField] int damageAmount;
    public int GetDamageAmount() { return damageAmount; }
    public void SetDamageAmount(int amount) { damageAmount = amount; }

    [SerializeField] float damageDelay;
    [SerializeField] public int speed;
    [SerializeField] int destroyTime;

    bool isDamageable = true;
    int originalLayer;
    int immune = 9;                // layer 9 is immune
    bool iHitShield = false;

    // Start is called before the first frame update
    void Start()
    {
        originalLayer = gameManager.instance.player.layer;
        if (type == damageType.bullet || type == damageType.enemyBullet)
        {
            rb.velocity = transform.forward * speed;
            Destroy(gameObject, destroyTime);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        if (other.isTrigger || other.transform == transform.parent) //!isDamageable) 
        {
            return;
        }

        if (other.CompareTag("Environment"))
        {
            Instantiate(targetHitEffect, this.transform.position, Quaternion.identity);
            Destroy(gameObject);
            return;
        }

        IDamage damage = other.GetComponent<IDamage>();

        if (damage != null)
        {
            audioManager.instance.PlayAud(damageHitAud[Random.Range(0, damageHitAud.Length)], 1);
            damage.takeDamage(damageAmount);
        }
        else
        {
            audioManager.instance.PlayAud(damageMissAud[Random.Range(0, damageMissAud.Length)], 1);
        }

        if (type == damageType.bullet)
        {
            Instantiate(gameManager.instance.playerScript.GetGunList()[gameManager.instance.playerScript.selectedGun].hitEffect, this.transform.position, Quaternion.identity);  

            if (gameManager.instance.playerScript.isCreator && gameManager.instance.playerScript.objectHeld != null)
            {
                if ((gameManager.instance.playerScript.objectHeld.CompareTag("Creatable") && gameManager.instance.playerScript.objectsCreated.Count < gameManager.instance.playerScript.GetMaxObjectsCreated())
                    ||
                    (gameManager.instance.playerScript.objectHeld.CompareTag("Ally") && gameManager.instance.playerScript.alliesCreated.Count < gameManager.instance.playerScript.GetMaxAlliesCreated()))
               { 
                    RaycastHit hit;
                Physics.Raycast(gameManager.instance.playerScript.GetShootPosition().position, gameManager.instance.playerScript.GetShootPosition().forward, out hit, 100, gameManager.instance.player.layer);
                    Debug.Log(hit.point);
                    if (other.CompareTag("Ground"))
                    {
                        GameObject groundObject = gameManager.instance.playerScript.objectHeld;

                        GameObject newGroundObject = Instantiate(groundObject, hit.point, Quaternion.identity);

                        Vector3 directionToPlayer = gameManager.instance.playerScript.GetPlayerCenter().position - newGroundObject.transform.position;

                        directionToPlayer.y = 0;

                        newGroundObject.transform.localScale = gameManager.instance.playerScript.GetObjectHeldOriginalSize();

                        newGroundObject.transform.rotation = Quaternion.LookRotation(directionToPlayer);

                        gameManager.instance.playerScript.addToCreatedLists(newGroundObject);                   // add object to list to enforce max created objects

                        gameManager.instance.playerScript.enableGameObject(newGroundObject);
                    }
                    else
                    {
                        GameObject wallObject = gameManager.instance.playerScript.objectHeld;

                        GameObject newWallObject = Instantiate(gameManager.instance.playerScript.objectHeld, hit.point, this.transform.rotation);

                        newWallObject.transform.localScale = gameManager.instance.playerScript.GetObjectHeldOriginalSize();

                        newWallObject.transform.rotation = Quaternion.LookRotation(hit.normal);

                        gameManager.instance.playerScript.addToCreatedLists(newWallObject);                       // add object to list to enforce max created objects

                        gameManager.instance.playerScript.enableGameObject(newWallObject);
                    }
                    audioManager.instance.PlayPasteObjectSound();
                }
            }
            Destroy(gameObject);
        }
        if (type == damageType.enemyBullet)
        {
            if (other.CompareTag("Shield"))
            {
                this.transform.SetParent(gameManager.instance.player.transform);
                this.transform.rotation = this.transform.rotation * Quaternion.Euler(0.0f, 180f, 0.0f);
                this.rb.velocity = transform.forward * speed;
                int delfectDamage = this.GetDamageAmount();
                this.tag = "Player Bullet";
                this.gameObject.layer = 3;
                this.rb.excludeLayers = 0;
                SphereCollider coll = this.GetComponent<SphereCollider>();
                coll.excludeLayers = 0;
                SetDamageAmount(delfectDamage);

            }
            else
            {
                Instantiate(targetHitEffect, this.transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }

    }
    //public void lowerObject(Transform objectHit, float speedDown, LayerMask groundLayer)
    //{
    //    Debug.Log(LayerMask.LayerToName(groundLayer));
    //    RaycastHit groundHit;
    //    if (Physics.Raycast(objectHit.position, Vector3.down, out groundHit, Mathf.Infinity, groundLayer))
    //    {
    //        Vector3 targetPosition = new Vector3(objectHit.position.x, groundHit.point.y, objectHit.position.z);

    //        objectHit.position = Vector3.Lerp(objectHit.position, targetPosition, speedDown * Time.deltaTime);
    //    }
    //}
}
