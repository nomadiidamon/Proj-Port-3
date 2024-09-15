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
    [SerializeField] AudioSource audEnemy;

    private int HP;
    [SerializeField] int startingHealth;
    [SerializeField] int faceEnemySpeed;

    [SerializeField] Image hpbar;
    [SerializeField] int animSpeedTrans;

    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;
    //lecture shoot angle
    //[SerializeField] int shootAngle;

    [SerializeField] AudioClip[] deathSound;
    [Range(0, 1)][SerializeField] float deathSoundVol;

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
        enemyPosition = FindAnyObjectByType<enemyAI>().GetComponent<CapsuleCollider>().ClosestPoint(shootPos.position);

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
        enemyDirection = enemyPosition - headPos.position;

        RaycastHit hit;
        Physics.Raycast(headPos.position, enemyDirection, out hit, gameManager.instance.playerScript.allyHeldAggroRange);
        Debug.DrawRay(headPos.position, enemyDirection);
        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                return true;
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
        agent.SetDestination(gameManager.instance.player.transform.position);

        Debug.Log("Soldier took " + amount + " damage");
        updateHPBar();
        StartCoroutine(flashRed());

        if (HP <= 0)
        {
            isDead = true;

            agent.isStopped = true;
            animator.enabled = false;
            this.enabled = false;

            playDeathAudio(deathSound[Random.Range(0, deathSound.Length)], deathSoundVol);

            StartCoroutine(destroyAfterSound());
        }
    }

    IEnumerator destroyAfterSound()
    {
        yield return new WaitForSeconds(deathSound[0].length);
        Destroy(gameObject);
        gameManager.instance.playerScript.alliesCreated.Remove(gameObject);
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
            //Debug.Log("Pew!");
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
        audEnemy.PlayOneShot(sound, vol);
    }

}

