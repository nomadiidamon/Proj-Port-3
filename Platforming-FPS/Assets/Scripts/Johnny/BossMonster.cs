using System.Collections;
using Unity.VisualScripting;
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
    [SerializeField] float pursueRange = 20f;
    public int bossMeleeAmount = 2;

    bool isJumping = false;
    bool isInMeleeRange;
    bool playerInRange;
    bool isRoaming;
    bool isSearching;
    bool isPursuing;
    bool isDead = false;
    bool isAttacking = false;
    bool isSecondPhase = false;

    float angleToPlayer;
    float distanceToPlayer;
    float stoppingDistanceOriginal;
    Vector3 currentTarget;
    Vector3 playerDir;
    Vector3 startingPosition;

    [Header("-----Patrol Points-----")]
    [SerializeField] Vector3 patrolPoint1;
    [SerializeField] Vector3 patrolPoint2;


    [Header("-----Lava Droplets-----")]
    [SerializeField] GameObject lavaDroplet;
    public float spawnRadius = 5f;
    public int numberOfDroplets = 10;
    public float ceilingHeight = 10f;

    private bool isPatrolPoint1 = true;
    public int GetCurrentHealth() { return HP; }

    void Start()
    {
        HP = startingHealth;

        
        startingPosition = transform.position;
        Vector3 tempPatrolPoint1 = startingPosition + new Vector3(0, 0, -roamDistance); // Left patrol point
        Vector3 tempPatrolPoint2 = startingPosition + new Vector3(0, 0, roamDistance);  // Right patrol point

        
        NavMeshHit hit;
        if (NavMesh.SamplePosition(tempPatrolPoint1, out hit, roamDistance, NavMesh.AllAreas))
        {
            patrolPoint1 = hit.position;
        }
        else
        {
            patrolPoint1 = startingPosition;
            Debug.LogWarning("PatrolPoint1 not on NavMesh.");
        }

        if (NavMesh.SamplePosition(tempPatrolPoint2, out hit, roamDistance, NavMesh.AllAreas))
        {
            patrolPoint2 = hit.position;
        }
        else
        {
            patrolPoint2 = startingPosition;
            Debug.LogWarning("PatrolPoint2 not on NavMesh.");
        }

        enemyManager.instance.updateEnemyCount(1);
        stoppingDistanceOriginal = agent.stoppingDistance;
        updateHPBar();
        animator.SetBool("SleepStart", true);
        agent.speed = movementSpeed;
        actualExpGiven = Random.Range(minExpGiven, maxExpGiven);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(patrolPoint1, 0.5f);
        Gizmos.DrawSphere(patrolPoint2, 0.5f);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(patrolPoint1, patrolPoint2);
    }

    private bool isPatrolling = true;
    private Coroutine patrolCoroutine;

    void Update()
    {
        if (isDead)
            return;

        if (isSecondPhase)
            return;

        distanceToPlayer = Vector3.Distance(this.transform.position, gameManager.instance.player.transform.position);

    
        if (isPatrolling && distanceToPlayer <= pursueRange)
        {
           
            SwitchToPursue();
            facePlayer();
        }
        else if (!isPatrolling && distanceToPlayer > pursueRange)
        {
          
            SwitchToPatrol();
            facePlayer();
        }


        if (!isSecondPhase)
        {
           
            if (distanceToPlayer <= pursueRange && !isJumping && !isDead && agent.isActiveAndEnabled)
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
                            animator.SetFloat("Walk", 0.2f);
                            agent.ResetPath();
                            agent.SetDestination(gameManager.instance.player.transform.position);
                        }
                    }
                }
                else if (!isAttacking)
                {
                    isInMeleeRange = true;
                    agent.isStopped = true; 
                    StartCoroutine(AttackPlayer());
                }
            }
            else if (distanceToPlayer > pursueRange)
            {
                agent.isStopped = true;
                animator.SetFloat("Walk", 0f); 
            }
        }
        else
        {
            
        }
    }

    private void SwitchToPursue()
    {
        isPatrolling = false; 
        if (patrolCoroutine != null)
        {
            StopCoroutine(patrolCoroutine); 
            patrolCoroutine = null; 
        }

        if (agent != null)
        {
            agent.speed = movementSpeed;
            agent.isStopped = false;
            agent.SetDestination(gameManager.instance.player.transform.position);
        }
    }

   
    private void SwitchToPatrol()
    {
        isPatrolling = true; 
        agent.isStopped = true; 
        animator.SetFloat("Walk", 0f); 

        if (patrolCoroutine == null) 
        {
            patrolCoroutine = StartCoroutine(Patrol());
        }
    }

    void PursuePlayer()
    {
        distanceToPlayer = Vector3.Distance(this.transform.position, gameManager.instance.player.transform.position);

        if (distanceToPlayer <= pursueRange && !isJumping && !isDead && agent.isActiveAndEnabled)
        {
            agent.isStopped = false;
            agent.SetDestination(gameManager.instance.player.transform.position);

            if (distanceToPlayer <= meleeRange && !isAttacking)
            {
                isInMeleeRange = true;
                agent.isStopped = true; 
                StartCoroutine(AttackPlayer());
            }
        }
        else if (distanceToPlayer > pursueRange)
        {
            isPatrolling = true;
        }
    }

    IEnumerator AttackPlayer()
    {
        if (isDead || !isInMeleeRange || isAttacking)
            yield break;

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

       
        distanceToPlayer = Vector3.Distance(this.transform.position, gameManager.instance.player.transform.position);
        if (distanceToPlayer <= meleeRange)
        {
            isInMeleeRange = true;
            agent.isStopped = true; 
        }
        else
        {
            isInMeleeRange = false;
            agent.isStopped = false;
            agent.SetDestination(gameManager.instance.player.transform.position);

            if (agent != null)
            {
                agent.speed = movementSpeed;
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
        playerDir = gameManager.instance.player.transform.position - headPos.position; 
        angleToPlayer = Vector3.Angle(playerDir, transform.forward); 

        if (angleToPlayer <= viewAngle) 
        {
            RaycastHit hit;
          
            if (Physics.Raycast(headPos.position, playerDir.normalized, out hit))
            {
                if (hit.collider.CompareTag("Player")) 
                {
                  
                    Quaternion rot = Quaternion.LookRotation(playerDir);
                    transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * facePlayerSpeed);
                }
            }
        }

        if (isSecondPhase)
        {
            RaycastHit hit;
       
            if (Physics.Raycast(headPos.position, playerDir.normalized, out hit))
            {
                if (hit.collider.CompareTag("Player")) 
                {
                  
                    Quaternion rot = Quaternion.LookRotation(playerDir);
                    transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * facePlayerSpeed);
                }
            }
        }
    }



    public void takeDamage(int amount)
    {
        if (isDead)
            return;

        HP = Mathf.Max(0, HP - amount);
        updateHPBar();

        // Check if in the second phase
        if (!isSecondPhase)
        {
            canSeePlayer(); // This will check if the boss should pursue
            agent.SetDestination(gameManager.instance.player.transform.position);
        }

        if (HP <= startingHealth / 2 && !isSecondPhase)
        {
            isSecondPhase = true;
            StartCoroutine(SecondPhase());
            Debug.Log("Boss entering second phase.");
        }

        if (HP <= 0)
        {
            isDead = true;
            enemyManager.instance.bossDefeated = true;
            enemyManager.instance.updateEnemyCount(-1);
            StartCoroutine(Die());
        }
    }

    IEnumerator SecondPhase()
    {
        Debug.Log("SecondPhase coroutine started.");

   
        agent.isStopped = true; 


        yield return new WaitForSeconds(1f);

        agent.SetDestination(startingPosition);
        agent.isStopped = false; 
        agent.stoppingDistance = 0;
        agent.speed = movementSpeed * 1.5f; 
        float stuckThreshold = 1f;
        float timeStuck = 0f;

       
        while (Vector3.Distance(transform.position, startingPosition) > 1f)
        {
            if (agent.velocity.magnitude < 0.1f)
            {
                timeStuck += Time.deltaTime;
                if (timeStuck >= stuckThreshold)
                {
                    Debug.Log("Boss is stuck, resetting destination.");
                    agent.SetDestination(startingPosition); 
                }
            }
            else
            {
                timeStuck = 0f; 
            }

            yield return null; 
        }

        Debug.Log("Boss has reached starting position.");

     
        agent.isStopped = true;

        StartCoroutine(Patrol());

        Debug.Log("Boss has started patrolling in second phase.");
    }

    IEnumerator Patrol()
    {
        Debug.Log("Patrol coroutine started.");

        while (!isDead && isSecondPhase)
        {
            Vector3 targetPatrolPoint = isPatrolPoint1 ? patrolPoint1 : patrolPoint2;

            agent.isStopped = false;
            agent.SetDestination(targetPatrolPoint);
            animator.SetFloat("Walk", 1f); 

            Debug.Log($"Patrolling to: {targetPatrolPoint}");

            while (Vector3.Distance(transform.position, targetPatrolPoint) > 1f)
            {
                yield return null;
            }
            facePlayer(); 

            //if (isDead || !isSecondPhase)
            //{
            //    yield break;
            //}

            
            Debug.Log($"Reached patrol point: {targetPatrolPoint}");

            animator.SetFloat("Walk", 0f);

            yield return new WaitForSeconds(2f);

            facePlayer();

            
            //animator.SetBool("Jump", true);
            animator.Play("Jump");
            //yield return new WaitForSeconds(1f);
            //animator.Play("Fly");
            yield return new WaitForSeconds(1f);
            //animator.SetBool("Land", true);
            animator.Play("Land");

            SpawnLavaDroplets();
            
           // yield return new WaitForSeconds(1f);


            isPatrolPoint1 = !isPatrolPoint1;
        }

        Debug.Log("Patrol coroutine ended.");
    }

    void SpawnLavaDroplets()
    {
        Vector3 playerPos = gameManager.instance.player.transform.position;

        for (int i = 0; i < numberOfDroplets; ++i)
        {
            Vector2 randomPos = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnDropletPos = new Vector3(playerPos.x + randomPos.x, ceilingHeight, playerPos.z + randomPos.y);

            RaycastHit hit;
            if (!Physics.Raycast(spawnDropletPos, Vector3.down, out hit, ceilingHeight))
            {
                Instantiate(lavaDroplet, spawnDropletPos, Quaternion.identity);
            }

            
            
        }

        
    }

    IEnumerator Die()
    {
        agent.isStopped = true;
        agent.enabled = false;

        //animator.SetBool("Die", true);
        animator.Play("Die");
        animator.applyRootMotion = false;

        yield return new WaitForSeconds(.5f);

        animator.enabled = false;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }

        if (other.CompareTag("LavaPuddle"))
        {
            return;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            agent.stoppingDistance = stoppingDistanceOriginal;

            if (agent != null)
            {
                agent.speed = movementSpeed;
                agent.isStopped = false;
                agent.SetDestination(gameManager.instance.player.transform.position);
            }
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