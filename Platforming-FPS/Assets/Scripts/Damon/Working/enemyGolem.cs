using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


public class enemyGolem : MonoBehaviour, IDamage
{
    [Header ("-----Components-----")]
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
    [SerializeField] float meleeAttackRate;
    [SerializeField] float movementSpeed;



    bool isInMeleeRange;
    bool isThrowing;
    bool isStomping;
    bool isSwiping;
    bool isDefending;
    bool playerInRange;
    bool isRoaming;
    bool isSearching;
    bool isPursuing;
    bool isPlayingSteps;
    bool isDead = false;
    bool sprintModActive;

    float angleToPlayer;
    float distanceToPlayer;
    float stoppingDistanceOriginal;

    Vector3 playerDir;
    Vector3 startingPosition;

    Color colorOrig;

    string currentAnimation = "";



    void Start()
    {
        HP = startingHealth;
        colorOrig = model.sharedMaterial.color;
        gameManager.instance.updateGameGoal(1);
        stoppingDistanceOriginal = agent.stoppingDistance;
        startingPosition = transform.position;
        updateHPBar();
        ChangeAnimation("Golem_idle");
        agent.speed = movementSpeed;


    }

    void Update()
    {
        distanceToPlayer = Vector3.Distance(this.transform.position, agent.destination);


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

        Debug.DrawLine(headPos.transform.position, gameManager.instance.player.transform.position);

        //if (currentAnimation == "Golem_runForward")
        //{
        
        //    agent.speed += movementSpeed;
        //    sprintModActive = true;
        //    if (distanceToPlayer <= 5.5f)
        //    {
        //        agent.speed -= movementSpeed;
        //    }
        //}
        //else
        //{
        //    agent.speed = movementSpeed;
        //}

    }

    private void ChangeAnimation(string targetAnim, float crossFade = 0.2f)
    {
        if (currentAnimation != targetAnim)
        {
            currentAnimation = targetAnim;
            animator.CrossFade(targetAnim, crossFade);
            Debug.Log("Playing the " + targetAnim + " animation");
        }
    }

    IEnumerator roam()
    {
        isRoaming = true;
        yield return new WaitForSeconds(roamTimer);
        ChangeAnimation("Golem_walkForward");
    
        agent.stoppingDistance = 0;
        Vector3 randomDistance = Random.insideUnitSphere * roamDistance;
        randomDistance += startingPosition;

        NavMeshHit hit;
        NavMesh.SamplePosition(randomDistance, out hit, roamDistance, 1);
        agent.SetDestination(hit.position);

        StartCoroutine(Searching());

        agent.stoppingDistance = stoppingDistanceOriginal;
        isRoaming = false;
    }

    IEnumerator Searching()
    {
        isSearching = true;
        //Debug.Log("looking around");

        agent.isStopped = true;
        ChangeAnimation("Golem_lookAround");
        AnimatorClipInfo[] info = animator.GetCurrentAnimatorClipInfo(0);
        yield return new WaitForSeconds(info[0].clip.length);
        agent.isStopped = false;
        isSearching = false;
        ChangeAnimation("Golem_walkForward");


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
                isPursuing = true;
                isSearching = false;
                StopCoroutine(roam());
                ChangeAnimation("Golem_runForward");
                Debug.Log("I see you!");
                agent.SetDestination(gameManager.instance.player.transform.position);

                if (isInMeleeRange && distanceToPlayer <= 1.75f)
                {

                    // perfrom melee
                    if (!isStomping)
                    {
                        Debug.Log("stomping starting");
                        StartCoroutine(Stomp());

                    }
                }
                
                if (!isInMeleeRange)
                {
                    //perform shooting/throwing
                    if (!isThrowing)
                        StartCoroutine(Throw());
                }



                if (agent.remainingDistance <= agent.stoppingDistance)
                    facePlayer();

                agent.stoppingDistance = stoppingDistanceOriginal;
                return true;
            }
            isPursuing = false;
        }
        return false;

    }


    void facePlayer()
    {
        Quaternion rot = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * facePlayerSpeed);
    }


    public void takeDamage(int amount)
    {
        if (isDead) return;
        string prevAnim = currentAnimation;
        Debug.Log(prevAnim + " was the last animation");
        ChangeAnimation("Golem_takeDamage");


        HP -= amount;
        agent.SetDestination(gameManager.instance.player.transform.position);
        StopCoroutine(roam());

        Debug.Log("Golem took " + amount + " damage");
        updateHPBar();
        StartCoroutine(flashRed());

        if (HP <= 0)
        {
            isDead = true;

            gameManager.instance.updateGameGoal(-1);

            agent.isStopped = true;
            animator.enabled = false;
            this.enabled = false;

            gameManager.instance.PlayAud(deathSound[Random.Range(0, deathSound.Length)], deathSoundVol);

            StartCoroutine(destroyAfterSound());
        }
        ChangeAnimation(prevAnim);
    }

    IEnumerator destroyAfterSound()
    {
        yield return new WaitForSeconds(deathSound[0].length);
        Destroy(gameObject);
    }


    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }

    IEnumerator Throw()
    {
        isThrowing = true;
        Vector3 direction = gameManager.instance.player.transform.position - projectilePos.transform.position;
        direction.Normalize();
        Quaternion projectileRotation = Quaternion.LookRotation(direction);
        Instantiate(projectile, projectilePos.transform.position, projectileRotation);

        yield return new WaitForSeconds(projectileShootRate);
        isThrowing = false;
    }

    IEnumerator Stomp()
    {
        isStomping = true;
        ChangeAnimation("Golem_stompAttack");
        yield return new WaitForSeconds(meleeAttackRate);

        isStomping = false;
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
            //Debug.Log("Player in shooting range");
            playerInRange = true;

            SphereCollider coll = GetComponent<SphereCollider>();
            if (distanceToPlayer <= coll.radius / 3)
            {
                isInMeleeRange = true;
            }
            else
            {
                isInMeleeRange = false;
            }

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
