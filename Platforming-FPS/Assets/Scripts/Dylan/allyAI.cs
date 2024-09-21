using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;
using UnityEngine.UI;

public class allyAI : MonoBehaviour, IDamage
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Renderer model;
    [SerializeField] Animator animator;
    [SerializeField] Transform shootPos;
    [SerializeField] Transform headPos;
    //[SerializeField] Collider meleeCollider;

    private int HP;
    [SerializeField] int startingHealth;
    [SerializeField] int faceEnemySpeed;

    [SerializeField] Image hpbar;
    [SerializeField] int animSpeedTrans;
    [SerializeField] float deathDelay;

    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;
    //lecture shoot angle
    //[SerializeField] int shootAngle;

    [Header("-----Audio-----")]
    [SerializeField] AudioSource audSource;
    [SerializeField] AudioClip[] deathSound;
    [Range(0, 1)][SerializeField] float deathSoundVol;
    [SerializeField] AudioClip[] shootSound;
    [Range(0, 1)][SerializeField] float shootSoundVol;
    [SerializeField] AudioClip[] hurtSound;
    [Range(0, 1)][SerializeField] float hurtSoundVol;

    bool isShooting;
    bool isPlayingSteps;
    private bool isDead = false;

    float angleToPlayer;

    Vector3 enemyDirection;
    Vector3 startingPosition;

    Vector3 enemyPosition;

    Color colorOrig;

    // Start is called before the first frame update
    void Start()
    {
        HP = startingHealth;
        colorOrig = model.material.color;
        startingPosition = transform.position;
        updateHPBar();
    }

    // Update is called once per frame
    void Update()
    {
        float agentSpeed = agent.velocity.normalized.magnitude;
        animator.SetFloat("Speed", Mathf.Lerp(animator.GetFloat("Speed"), agentSpeed, Time.deltaTime * animSpeedTrans));

        Debug.Log(canSeeEnemy());

        if (canSeeEnemy())
        {
            agent.SetDestination(enemyPosition);
            if (!isShooting)
                StartCoroutine(shoot());

            if (agent.remainingDistance <= agent.stoppingDistance)
                faceEnemy();
        }
        else
        {
            agent.stoppingDistance = 3;
            agent.SetDestination(gameManager.instance.player.transform.position);
        }
    }

    int getHealth()
    {
        return HP;
    }

    bool canSeeEnemy()
    {
        enemyAI[] enemies = FindObjectsOfType<enemyAI>();
        if (enemies.Length > 0)
        {
            RaycastHit hit;
            for (int enemyIndex = 0; enemyIndex < enemies.Length; enemyIndex++)
            {
                enemyPosition = enemies[enemyIndex].GetComponent<CapsuleCollider>().ClosestPoint(shootPos.position);
                enemyDirection = enemyPosition - headPos.position;

                Physics.Raycast(headPos.position, enemyDirection, out hit, gameManager.instance.playerScript.allyHeldAggroRange);
                if (hit.collider != null)
                {
                    if (hit.collider.CompareTag("Enemy"))
                    {
                        Debug.DrawRay(headPos.position, enemyDirection);
                        return true;
                    }
                }
            }
        }
        return false;

    }


    void faceEnemy()
    {
        Quaternion rot = Quaternion.LookRotation(enemyDirection);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceEnemySpeed);
    }

    public void takeDamage(int amount)
    {
        if (isDead) return;

        HP -= amount;
        playHurtAudio(hurtSound[Random.Range(0, hurtSound.Length)], hurtSoundVol);

        agent.SetDestination(gameManager.instance.player.transform.position);

        updateHPBar();
        StartCoroutine(flashRed());

        if (HP <= 0)
        {
            isDead = true;

            agent.isStopped = true;
            animator.enabled = false;
            this.enabled = false;

            playDeathAudio(deathSound[Random.Range(0, deathSound.Length)], deathSoundVol);

            StartCoroutine(destroyDelay());
        }
    }

    IEnumerator destroyDelay()
    {
        if (isShooting)
        {
            isShooting = false;
        }
        animator.SetTrigger("Dead");
        yield return new WaitForSeconds(deathDelay);
        agent.isStopped = true;
        animator.enabled = false;
        this.enabled = false;
        gameManager.instance.playerScript.alliesCreated.Remove(gameObject);
        Destroy(gameObject);
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }

    IEnumerator shoot()
    {
        isShooting = true;
        animator.SetTrigger("Shoot");
        //createProjectile();
        //Vector3 direction = gameManager.instance.player.transform.position - shootPos.transform.position;
        //direction.Normalize();
        //Quaternion bulletRotation = Quaternion.LookRotation(direction);
        //Instantiate(bullet, shootPos.transform.position, bulletRotation);

        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    public void createProjectile()
    {
        if (GetComponent<enemyAI>() == null)
        {
            Vector3 direction = enemyPosition - shootPos.transform.position;
        direction.Normalize();
        Quaternion bulletRotation = Quaternion.LookRotation(direction);

            playShootAudio(shootSound[Random.Range(0, shootSound.Length)], shootSoundVol);
            Instantiate(bullet, shootPos.transform.position, bulletRotation);
        }
    }

    //public void weaponColliderOn()
    //{
    //    meleeCollider.enabled = true;
    //}

    //public void weaponColliderOff()
    //{
    //    meleeCollider.enabled = false;

    //}

    void updateHPBar()
    {
        if (hpbar != null)
        {
            hpbar.fillAmount = (float)HP / startingHealth;
        }
    }

    public void playDeathAudio(AudioClip sound, float vol)
    {
        audSource.PlayOneShot(sound, vol);
    }
    public void playShootAudio(AudioClip sound, float vol)
    {
        audSource.PlayOneShot(sound, vol);
    }
    public void playHurtAudio(AudioClip sound, float vol)
    {
        audSource.PlayOneShot(sound, vol);
    }
}

