using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


public class babyGolem : MonoBehaviour, IDamage
{

    [Header("-----Components-----")]
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Renderer model;
    [SerializeField] Animator animator;
    [SerializeField] Transform projectilePos;
    [SerializeField] Transform headPos;
    [SerializeField] AudioClip[] deathSound;
    [Range(0, 1)][SerializeField] float deathSoundVol;
    [SerializeField] Image hpbar;
    [SerializeField] GameObject projectile;


    [Header("-----Attributes-----")]
    private int HP;
    [Range(0, 100)][SerializeField] int startingHealth;
    [SerializeField] public int maxExpGiven;
    [SerializeField] public int minExpGiven;
    public int actualExpGiven;

    [Header("-----Factors-----")]
    [SerializeField] int viewAngle;
    [SerializeField] int facePlayerSpeed;
    [SerializeField] int roamDistance;
    [SerializeField] float roamTimer;
    [SerializeField] float projectileShootRate;
    [SerializeField] int projectileDistance;
    [SerializeField] float meleeAttackRate;
    [SerializeField] float movementSpeed;
    [SerializeField] float meleeRange;



    bool isFighting;
    //bool isInMeleeRange;
    bool isThrowing;
    bool isDefending;
    bool playerInRange;
    bool isRoaming;
    bool isSearching;
    //bool isPursuing;
    bool isDead = false;


    float angleToPlayer;
    float distanceToPlayer;
    float stoppingDistanceOriginal;
    Vector3 currentTarget;


    Vector3 playerDir;
    Vector3 startingPosition;

    Color colorOrig;

    string currentAnimation = "";


    void Start()
    {
        HP = startingHealth;
        colorOrig = model.material.color;
        enemyManager.instance.updateEnemyCount(1);
        stoppingDistanceOriginal = agent.stoppingDistance;
        startingPosition = transform.position;
        updateHPBar();
        ChangeAnimation("Golem_idle");
        agent.speed = movementSpeed;
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        rb.useGravity = false;
        isThrowing = false;




    }


    void Update()
    {
        distanceToPlayer = Vector3.Distance(this.transform.position, gameManager.instance.player.transform.position);
        if (distanceToPlayer <= meleeRange)
        {
            //isInMeleeRange = true;
            agent.isStopped = true;
        }
        else if (distanceToPlayer > meleeRange)
        {
            //isInMeleeRange = false;
            agent.isStopped = false;
        }

        if (playerInRange && canSeePlayer())
        {
            if (distanceToPlayer > meleeRange)
            {
                ChangeAnimation("Golem_runForward");
                agent.SetDestination(gameManager.instance.player.transform.position);
            }
        }
        else if (playerInRange && !canSeePlayer())
        {
            ChangeAnimation("Golem_lookAround");

        }

        Debug.DrawLine(headPos.transform.position, gameManager.instance.player.transform.position);
    }

    private void ChangeAnimation(string targetAnim, float crossFade = 0.2f, float switchThreshold = 0.8f)
    {
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);

        if (currentAnimation != targetAnim && info.normalizedTime >= switchThreshold)
        {
            currentAnimation = targetAnim;
            animator.CrossFade(targetAnim, crossFade);
            Debug.Log("****Playing the " + targetAnim + " animation****");
        }
    }


    bool canSeePlayer()
    {
        playerDir = gameManager.instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDir, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= viewAngle)
            {

                facePlayer();

                agent.SetDestination(gameManager.instance.player.transform.position);

                if (playerInRange)
                {
                    if (!isThrowing)
                    {
                        StartCoroutine(Throw());
                    }
                }

                if (agent.remainingDistance <= agent.stoppingDistance)
                    facePlayer();

                agent.stoppingDistance = stoppingDistanceOriginal;

                return true;
            }

        }
        return false;

    }

    IEnumerator Throw()
    {
        ChangeAnimation("Golem_throwAttack");
        agent.isStopped = true;
        isThrowing = true;
        Vector3 prevDestination = agent.destination;
        agent.SetDestination(this.transform.position);
        agent.speed = 0;
        Vector3 direction = gameManager.instance.player.transform.position - projectilePos.transform.position;
        direction.Normalize();
        yield return new WaitForSeconds(projectileShootRate);
        agent.SetDestination(prevDestination);
        agent.isStopped = false;
        agent.speed = movementSpeed;
        isThrowing = false;
    }

    public void stopPursuing()
    {
        agent.SetDestination(this.transform.position);
        //isPursuing = false;
        agent.speed = 0;
        agent.isStopped = true;
    }

    void facePlayer()
    {
        Quaternion rot = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * facePlayerSpeed);

    }

    public void takeDamage(int amount)
    {
        if (isDefending)
        {
            return;
        }

        if (isDead) return;
        string prevAnim = currentAnimation;
        ChangeAnimation("Golem_takeDamage");


        HP -= amount;
        agent.SetDestination(gameManager.instance.player.transform.position);
        canSeePlayer();
        Debug.Log("Golem took " + amount + " damage");
        updateHPBar();
        StartCoroutine(flashRed());

        if (HP <= 0)
        {
            isDead = true;

            enemyManager.instance.updateEnemyCount(-1);

            agent.isStopped = true;
            animator.enabled = false;
            this.enabled = false;

            audioManager.instance.PlayAud(deathSound[Random.Range(0, deathSound.Length)], deathSoundVol);

            StartCoroutine(destroyAfterSound());
        }

        ChangeAnimation(prevAnim);
    }


    IEnumerator destroyAfterSound()
    {
        yield return new WaitForSeconds(deathSound[0].length / 2);
        GameObject rubble = projectile;
        Rigidbody rb = rubble.GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.velocity = Vector3.zero;
        Instantiate(rubble, this.transform.position, Quaternion.identity);
        Instantiate(rubble, this.transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

    IEnumerator flashRed()
    {
        Debug.Log("Flashing from damage");
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
        Debug.Log(currentAnimation);

    }


    public void createProjectile()
    {
        Vector3 direction = gameManager.instance.player.transform.position - projectilePos.transform.position;
        direction.Normalize();
        Quaternion bulletRotation = Quaternion.LookRotation(direction);
        Instantiate(projectile, projectilePos.transform.position, bulletRotation);
    }

    private void OnTriggerEnter(Collider other)
    {
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
            agent.stoppingDistance = 0;
        }
    }

    void updateHPBar()
    {
        if (hpbar != null)
        {
            hpbar.fillAmount = (float)HP / startingHealth;
        }
    }


}