using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


public class babyGolem : MonoBehaviour
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
    [Range(0, 20)][SerializeField] int startingHealth;

    [Header("-----Factors-----")]
    [SerializeField] int viewAngle;
    [SerializeField] int facePlayerSpeed;
    [SerializeField] int roamDistance;
    [SerializeField] float roamTimer;
    [SerializeField] int animSpeedTrans;
    [SerializeField] float projectileShootRate;
    [SerializeField] int projectileDistance;
    //[SerializeField] float meleeAttackRate;
    [SerializeField] float movementSpeed;
    [SerializeField] float combatStoppingDistance;
    //[SerializeField] float sprintSpeed;
    //[SerializeField] float meleeRange;


    bool isFighting;
    //bool isInMeleeRange;
    bool isThrowing;
    bool playerInRange;
    bool isRoaming;
    bool isSearching;
    bool isPlayingSteps;
    bool isDead = false;
    //bool sprintModActive;

    float angleToPlayer;
    float distanceToPlayer;
    float stoppingDistanceOriginal;
    Vector3 currentTarget;
    //[SerializeField] myTimer timer;
    //public float timeStart;
    //public float timeEnd;
    //float currentTimeOnTimer;

    Vector3 playerDir;
    Vector3 startingPosition;

    Color colorOrig;

    string currentAnimation = "";



    void Start()
    {
        HP = startingHealth;
        colorOrig = model.sharedMaterial.color;
        enemyManager.instance.updateEnemyCount(1);
        stoppingDistanceOriginal = agent.stoppingDistance;
        startingPosition = transform.position;
        updateHPBar();
        ChangeAnimation("Golem_idle");
        agent.speed = movementSpeed;
        //sprintSpeed = movementSpeed * 2;
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        rb.useGravity = false;

    }

    void Update()
    {
        distanceToPlayer = Vector3.Distance(this.transform.position, gameManager.instance.player.transform.position);

        //if (distanceToPlayer <= meleeRange)
        //{
        //    isInMeleeRange = true;
        //    agent.isStopped = true;
        //}
        //else if (distanceToPlayer >= meleeRange)
        //{
        //    isInMeleeRange = false;
        //    agent.isStopped = false;
        //}

        //if (isPursuing)
        //{
        //    agent.speed = sprintSpeed;
        //}
        //else if (!isPursuing)
        //{
        //    agent.speed = movementSpeed;
        //}

        //if (currentAnimation == "Golem_lookAround")
        //{
        //    stopPursuing();
        //}

        if (playerInRange && !canSeePlayer())
        {
            if (!isRoaming && agent.remainingDistance < 0.05f)
            {
                StartCoroutine(roam());
            }
        }
        else if (!playerInRange)
        {
            if (!isRoaming && agent.remainingDistance < 0.05f)
            {
                StartCoroutine(roam());
            }
        }

        if (isFighting)
        {
            agent.stoppingDistance = combatStoppingDistance;

        }

        Debug.DrawLine(headPos.transform.position, gameManager.instance.player.transform.position);

    }

    private void ChangeAnimation(string targetAnim, float crossFade = 0.2f, float switchThreshold = 0.8f)
    {
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);

        if (targetAnim == "Golem_runForward")
        {
            switchThreshold -= 0.35f;
        }
        if (targetAnim == "Golem_throwAttack" && currentAnimation == "Golem_throwAttack")
        {
            switchThreshold = .95f;
        }

        if (currentAnimation != targetAnim && info.normalizedTime >= switchThreshold)
        {
            currentAnimation = targetAnim;
            animator.CrossFade(targetAnim, crossFade);
            Debug.Log("****Playing the " + targetAnim + " animation****");
        }
    }

    IEnumerator roam()
    {
        if (isFighting)
        {
            yield break;

        }
        isRoaming = true;
        yield return new WaitForSeconds(roamTimer);
        ChangeAnimation("Golem_runForward");

        Vector3 randomDistance = Random.insideUnitSphere * roamDistance;
        randomDistance += startingPosition;

        NavMeshHit hit;
        NavMesh.SamplePosition(randomDistance, out hit, roamDistance, NavMesh.AllAreas);
        float apart = Vector3.Distance(hit.position, agent.transform.position);
        if (apart <= 3.75f)
        {
            NavMesh.SamplePosition(randomDistance, out hit, roamDistance, NavMesh.AllAreas);
            apart = Vector3.Distance(hit.position, agent.transform.position);
            if (apart <= 3.75f)
            {
                agent.SetDestination(startingPosition);
            }
            else
                agent.SetDestination(startingPosition);
        }
        else
        {
            agent.SetDestination(hit.position);
        }

        //isSearching = true;
        agent.isStopped = true;
        ChangeAnimation("Golem_lookAround");
        agent.speed = 0;
        Vector3 lastPos = agent.destination;
        stopMoving();
        yield return new WaitForSeconds(2.5f);
        agent.SetDestination(lastPos);

        agent.speed = movementSpeed;
        agent.isStopped = false;
        //isSearching = false;
        ChangeAnimation("Golem_runForward");

        agent.stoppingDistance = stoppingDistanceOriginal;
        isRoaming = false;
    }

    IEnumerator Searching()
    {
        isSearching = true;
        agent.isStopped = true;
        ChangeAnimation("Golem_lookAround");
        yield return new WaitForSeconds(2.5f);
        agent.isStopped = false;
        isSearching = false;
        ChangeAnimation("Golem_runForward");
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
                isFighting = true;
                //isPursuing = true;
                //isSearching = false;
                StopCoroutine(roam());
                StopCoroutine(Searching());
                agent.SetDestination(gameManager.instance.player.transform.position);
                //if (isInMeleeRange && distanceToPlayer <= meleeRange)
                //{
                //    stopPursuing();

                //    // perfrom melee
                //    if (!isStomping)
                //    {

                //        isStomping = true;
                //        ChangeAnimation("Golem_stompAttack");
                //        isStomping = false;
                //    }
                //    else if (isStomping)
                //    {
                //        ChangeAnimation("Golem_intimidateOne");
                //    }


                //}
                //if (!isInMeleeRange)
                //{
                //    ChangeAnimation("Golem_runForward");

                //}
                //agent.isStopped = false;
                //agent.SetDestination(gameManager.instance.player.transform.position);

                    //perform shooting/throwing
                if (!isThrowing)
                {
                    isThrowing = true;
                    ChangeAnimation("Golem_throwAttack");
                    agent.isStopped = true;

                    agent.isStopped = false;
                    isThrowing = false;
                }

                if (!isThrowing && currentAnimation == "Golem_throwAttack")
                {
                    ChangeAnimation("Golem_runForward");
                }
                




                if (agent.remainingDistance <= agent.stoppingDistance)
                    facePlayer();

                agent.stoppingDistance = stoppingDistanceOriginal;

                return true;
            }
            isFighting = false;
        }
        agent.stoppingDistance = stoppingDistanceOriginal;
        return false;

    }

    public void startPursuing()
    {
        agent.SetDestination(gameManager.instance.player.transform.position);
        //isPursuing = true;
        //agent.speed = sprintSpeed;
        agent.isStopped = false;
    }

    public void startRoaming()
    {
        isRoaming = true;
        agent.speed = movementSpeed;
        agent.isStopped = false;
    }

    public void stopPursuing()
    {
        agent.SetDestination(this.transform.position);
        //isPursuing = false;
        agent.speed = 0;
        agent.isStopped = true;
        ChangeAnimation("Golem_idle");
    }

    public void stopMoving()
    {
        agent.SetDestination(this.transform.position);
        agent.speed = 0;
        agent.isStopped = true;
    }


    void facePlayer()
    {
        Quaternion rot = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * facePlayerSpeed);
        Debug.Log(currentAnimation);

    }


    public void takeDamage(int amount)
    {
        if (isDead) return;
        //string prevAnim = currentAnimation;
        //Debug.Log(prevAnim + " was the last animation");
        //ChangeAnimation("Golem_takeDamage");


        HP -= amount;
        agent.SetDestination(gameManager.instance.player.transform.position);
        StopCoroutine(roam());
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

    IEnumerator Throw()
    {
        ChangeAnimation("Golem_throwAttack");
        agent.isStopped = true;
        isThrowing = true;
        //Vector3 direction = gameManager.instance.player.transform.position - projectilePos.transform.position;
        //direction.Normalize();
        //Quaternion projectileRotation = Quaternion.LookRotation(direction);
        //Instantiate(projectile, projectilePos.transform.position, projectileRotation);
        agent.isStopped = false;
        yield return new WaitForSeconds(projectileShootRate);
        isThrowing = false;
    }

    //IEnumerator Stomp()
    //{
    //    isStomping = true;
    //    agent.isStopped = true;
    //    agent.SetDestination(this.transform.position);
    //    ChangeAnimation("Golem_stompAttack");
    //    yield return new WaitForSeconds(1.5f);
    //    isStomping = false;
    //    agent.isStopped = false;

    //    if (distanceToPlayer >= meleeRange)
    //    {
    //        agent.SetDestination(gameManager.instance.player.transform.position);

    //    }
    //    ChangeAnimation("Golem_runForward");
    //    Debug.Log(currentAnimation);

    //}

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
            //Debug.Log("Player in shooting range");
            playerInRange = true;
        }
    }





    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Debug.Log("Player out of range");

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