using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurretControl : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;
    [SerializeField] Transform shootPos;

    [SerializeField] int health;
    private int startingHealth;

    [SerializeField] public int maxExpGiven;
    [SerializeField] public int minExpGiven;
    public int actualExpGiven;

    [SerializeField] int viewAngle;
    [SerializeField] int facePlayerSpeed;
    [SerializeField] Canvas hpFrame;
    [SerializeField] Image hpbar;
    [SerializeField] int damage;
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;
    [SerializeField] GameObject muzzleFlash;


    bool isShooting;
    bool playerInRange;

    float angleToPlayer;
    Vector3 playerDir;
    Color colorOriginal;
    Vector3 playerPos;

    // Start is called before the first frame update
    void Start()
    {
        startingHealth = health;
        colorOriginal = model.material.color;
        UpdateHealthBar();
        enemyManager.instance.updateEnemyCount(1);
        bullet.GetComponent<Damage>().SetDamageAmount(damage);
        actualExpGiven = Random.Range(minExpGiven, maxExpGiven);

    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange && canSeePlayer())
        {
            
        }
        healthBarFacePlayer();
    }

    void healthBarFacePlayer()
    {
        playerPos = enemyManager.instance.playerPosition.position;
        Quaternion rot = Quaternion.LookRotation(playerPos);

        hpbar.canvas.transform.rotation = Quaternion.Lerp(hpbar.transform.rotation, rot, Time.deltaTime * facePlayerSpeed);


    }

    void UpdateHealthBar()
    {
        if (hpbar != null)
        {
            hpbar.fillAmount = (float)health / startingHealth;
        }
    }

    public void takeDamage(int amount)
    {
        Debug.Log("Turret took " + amount + " damage");
        health -= amount;

        UpdateHealthBar();  // Update the health bar after taking damage

        StartCoroutine(flashRed());

        if (health <= 0)
        {
            enemyManager.instance.updateEnemyCount(-1);

            Destroy(gameObject);
            gameUIManager.instance.updateExperienceCount(actualExpGiven);
        }
    }

    bool canSeePlayer()
    {
        playerDir = gameManager.instance.player.transform.position - shootPos.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        RaycastHit hit;
        if (Physics.Raycast(shootPos.position, playerDir, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= viewAngle)
            {

                facePlayer();
                if (!isShooting)
                {
                    StartCoroutine(shoot());
                }

                return true;
            }
                
        }

        return false;
    }

    IEnumerator flashRed()
    {
        Debug.Log("Flashing cause I got shot!");
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.5f);
        model.material.color = colorOriginal;
    }

    IEnumerator shoot()
    {
        isShooting = true;

        Vector3 direction = gameManager.instance.player.transform.position - shootPos.transform.position;
        direction.Normalize();
        
        Quaternion bulletRotation = Quaternion.LookRotation(direction);
        Instantiate(bullet, shootPos.transform.position, bulletRotation);

        StartCoroutine(flashMuzzle());

        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    IEnumerator flashMuzzle()
    {
        muzzleFlash.SetActive(true);
        yield return new WaitForSeconds(.05f);
        muzzleFlash.SetActive(false);
    }

    void facePlayer()
    {
        Quaternion rot = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * facePlayerSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger entered by " + other.name + " with tag " + other.tag);
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
