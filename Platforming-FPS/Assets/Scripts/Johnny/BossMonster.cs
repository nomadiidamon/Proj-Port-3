using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class bossMonster : MonoBehaviour, IDamage
{

    [Header("-----Components-----")]
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator animator;
    [SerializeField] Transform headPos;
    [SerializeField] Image hpbar;
    [SerializeField] SphereCollider bossArea;
    [SerializeField] GameObject deathDrop;
    [SerializeField] enemyManager emanager;


    [Header("-----Attributes-----")]
    private int HP;
    [Range(0, 100)][SerializeField] int startingHealth;
    [SerializeField] public int maxExpGiven;
    [SerializeField] public int minExpGiven;
    public int actualExpGiven;

    [Header("-----Audio-----")]
    [SerializeField] AudioClip[] deathSound;
    [Range(0, 1)][SerializeField] float deathSoundVol;

    [Header("-----Factors-----")]
    [SerializeField] int viewAngle;
    [SerializeField] int facePlayerSpeed;
    [SerializeField] int roamDistance;
    [SerializeField] float roamTimer;
    [SerializeField] float meleeAttackRate;
    [SerializeField] float movementSpeed;
    [SerializeField] float meleeRange;
    public int bossMeleeAmount = 2;

    bool isJumping = false;
    bool isInMeleeRange;
    bool playerInRange;
    bool isRoaming;
    bool isSearching;
    bool isPursuing;
    bool isDead = false;
    bool isAttacking = false;

    float angleToPlayer;
    float distanceToPlayer;
    float stoppingDistanceOriginal;
    Vector3 currentTarget;
    Vector3 playerDir;
    Vector3 startingPosition;

    public int GetCurrentHealth() { return HP; }

    void Start()
    {
        HP = startingHealth;
        
        //colorOrig = model.material.color;
        enemyManager.instance.updateEnemyCount(1);
        stoppingDistanceOriginal = agent.stoppingDistance;
        startingPosition = transform.position;
        updateHPBar();
        animator.SetBool("SleepStart", true);
        agent.speed = movementSpeed;
        actualExpGiven = Random.Range(minExpGiven, maxExpGiven);
    }


    void Update()
    {
        distanceToPlayer = Vector3.Distance(this.transform.position, gameManager.instance.player.transform.position);

        // Check if the agent should be walking or idle
        if (!isJumping && !isDead && agent.isActiveAndEnabled)
        {
            if (distanceToPlayer > meleeRange)
            {
                isInMeleeRange = false;
                agent.isStopped = false;
                agent.SetDestination(gameManager.instance.player.transform.position);

                if (agent.remainingDistance > agent.stoppingDistance)
                {
                    float velocityMagnitude = agent.velocity.magnitude;

                    if (velocityMagnitude > 0.1f)
                    {
                        float animationSpeedMultiplier = velocityMagnitude / movementSpeed;
                        animator.SetFloat("Walk", animationSpeedMultiplier);
                    }
                    else
                    {
                        // Prevent getting stuck in very slow motion by re-assigning destination
                        animator.SetFloat("Walk", 0.2f);
                        agent.ResetPath(); // Force reset path to avoid getting stuck
                        agent.SetDestination(gameManager.instance.player.transform.position);
                    }
                }
            }
            else if (!isAttacking)
            {
                isInMeleeRange = true;
                agent.isStopped = true; // Stop agent if in melee range
                StartCoroutine(AttackPlayer());
            }
        }
    }

    

    IEnumerator AttackPlayer()
    {
        

        if (isDead || !isInMeleeRange || isAttacking) yield break;

       
        isAttacking = true;
        agent.isStopped = true;
        animator.SetBool("Hit", true);

        yield return new WaitForSeconds(0.6f);

        if (Vector3.Distance(this.transform.position, gameManager.instance.player.transform.position) <= meleeRange)
        {
            IDamage playerDamageComponent = gameManager.instance.player.GetComponent<IDamage>();
            if (playerDamageComponent != null)
            {
                playerDamageComponent.takeDamage(bossMeleeAmount); 
            }
        }

    
        yield return new WaitForSeconds(meleeAttackRate);
        isAttacking = false;
        animator.SetBool("Hit", false);

        if (!isInMeleeRange)
        {
            agent.isStopped = false;

            agent.SetDestination(gameManager.instance.player.transform.position);

            if (agent.velocity.magnitude > 0.1)
            {
                animator.SetFloat("Walk", agent.velocity.magnitude / movementSpeed);
            }
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
                if (isInMeleeRange && distanceToPlayer <= meleeRange)
                {
                    stopPursuing();
                }

                agent.isStopped = false;
                agent.SetDestination(gameManager.instance.player.transform.position);

                if (playerInRange)
                {
                   
                }

                if (agent.remainingDistance <= agent.stoppingDistance)
                    facePlayer();

                agent.stoppingDistance = stoppingDistanceOriginal;

                return true;
            }
        
        }
        return false;

    }

    public void stopPursuing()
    {
        agent.SetDestination(this.transform.position);
        isPursuing = false;
        agent.speed = 0;
        agent.isStopped = true;
        animator.SetFloat("Walk", 0);

    }

    void facePlayer()
    {
        Quaternion rot = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * facePlayerSpeed);
    }

    public void takeDamage(int amount)
    {
        if (isDead) return;

        HP = Mathf.Max(0, HP - amount);
        updateHPBar();

      
        if (!isAttacking)
        {
            
            canSeePlayer();
            agent.SetDestination(gameManager.instance.player.transform.position);
        }

        if (HP <= 0)
        {
            isDead = true;
            enemyManager.instance.updateEnemyCount(-1);
            StartCoroutine(Die());
        }
    }

    IEnumerator Die()
    {
       
        agent.isStopped = true;
        agent.enabled = false;

       
        animator.SetBool("Die", true);
        animator.applyRootMotion = false;

        yield return new WaitForSeconds(.5f); 

       
        animator.enabled = false;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }
   
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);

        
        
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
