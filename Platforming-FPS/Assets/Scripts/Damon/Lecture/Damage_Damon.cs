using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UIElements;

public class Damage : MonoBehaviour
{
    [SerializeField] enum damageType { bullet, stationary, impact, enemyBullet }
    [SerializeField] damageType type;
    [SerializeField] Rigidbody rb;
    [SerializeField] ParticleSystem targetHitEffect;


    [SerializeField] int damageAmount;
    public int GetDamageAmount() { return damageAmount; }
    public void SetDamageAmount(int amount) { damageAmount = amount; }

    [SerializeField] float damageDelay;
    [SerializeField] int speed;
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
            damage.takeDamage(damageAmount);
        }
        if (type == damageType.bullet)
        {
            Instantiate(gameManager.instance.playerScript.GetGunList()[gameManager.instance.playerScript.selectedGun].hitEffect, this.transform.position, Quaternion.identity);
            if (gameManager.instance.playerScript.isCreator)
            {
                if (other.CompareTag("Ground"))
                {
                    GameObject groundObject = gameManager.instance.playerScript.objectHeld;
                    float halfHeight = groundObject.transform.localScale.y / 2;

                    //GameObject thing2 = Instantiate(thing, new Vector3(this.transform.position.x, thing.transform.localScale.y * 1.5f, this.transform.position.z),
                    //    Quaternion.Euler(0, 0, this.transform.rotation.z));
                    //thing2.transform.LookAt(gameManager.instance.player.transform);
                    
                    GameObject newGroundObject = Instantiate(groundObject, new Vector3(this.transform.position.x, halfHeight, this.transform.position.z),
                        Quaternion.Euler(0, 0, this.transform.rotation.z));
                    newGroundObject.transform.LookAt(gameManager.instance.player.transform);

                }
                else
                { 
                    GameObject wallObject = gameManager.instance.playerScript.objectHeld;

                   GameObject newWallObject = Instantiate(gameManager.instance.playerScript.objectHeld,
                       this.transform.position - new Vector3(0, 0, gameManager.instance.playerScript.objectHeld.transform.localScale.z / 2), this.transform.rotation);
                    newWallObject.transform.rotation = Quaternion.AngleAxis(180, Vector3.up);
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
                SetDamageAmount(delfectDamage);

            }
            else
            {
                Instantiate(targetHitEffect, this.transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }

    }
}
