using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class bossGolem : MonoBehaviour, IDamage, IDeflect
{

    [Header("-----Components-----")]
    [SerializeField] NavMeshAgent agent;
    //[SerializeField] Renderer model;
    [SerializeField] Animator animator;
    [SerializeField] Transform projectilePos;
    [SerializeField] Transform headPos;
    [SerializeField] Transform stompPosition;
    [SerializeField] ParticleSystem stompEffect;
    [SerializeField] GameObject projectile;
    [SerializeField] ParticleSystem bulletEffect;
    [SerializeField] Image hpbar;
    [SerializeField] SphereCollider bossArea;
    [SerializeField] GameObject deathDrop;
    [SerializeField] GameObject shield;
    [SerializeField] GameObject particleHitBoxStart;


    [Header("-----Attributes-----")]
    private int HP;
    [Range(0, 100)][SerializeField] int startingHealth;
    [SerializeField] public int maxExpGiven;
    [SerializeField] public int minExpGiven;
    public int actualExpGiven;

    [Header("-----Audio-----")]
    [SerializeField] AudioClip[] deathSound;
    [Range(0, 1)][SerializeField] float deathSoundVol;
    [SerializeField] AudioClip[] stompSound;
    [Range(0, 1)][SerializeField] float stompSoundVol;

    [Header("-----Factors-----")]
    [SerializeField] int viewAngle;
    [SerializeField] int facePlayerSpeed;
    [SerializeField] int roamDistance;
    [SerializeField] float roamTimer;
    [SerializeField] int animSpeedTrans;
    [SerializeField] float projectileShootRate;
    [SerializeField] int projectileDistance;
    [SerializeField] int stompDamage;
    [SerializeField] float meleeAttackRate;
    [SerializeField] float movementSpeed;
    [SerializeField] float sprintSpeed;
    [SerializeField] float meleeRange;
    [SerializeField] float deflectionTime;
    [SerializeField] float vulnerableTime;



    bool isFighting;
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
    bool secondPhase;

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

    public int GetCurrentHealth() { return HP; }    

    void Start()
    {
        HP = startingHealth;
        //colorOrig = model.material.color;
        enemyManager.instance.updateEnemyCount(1);
        stoppingDistanceOriginal = agent.stoppingDistance;
        startingPosition = transform.position;
        updateHPBar();
        ChangeAnimation("Golem_idle");
        agent.speed = movementSpeed;
        sprintSpeed = movementSpeed * 2;
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        rb.useGravity = false;
        isThrowing = false;
        actualExpGiven = Random.Range(minExpGiven, maxExpGiven);
    }


    void Update()
    {
        distanceToPlayer = Vector3.Distance(this.transform.position, gameManager.instance.player.transform.position);
        if (distanceToPlayer <= meleeRange)
        {
            isInMeleeRange = true;
            agent.isStopped = true;
        }
        else if (distanceToPlayer >= meleeRange)
        {
            isInMeleeRange = false;
            agent.isStopped = false;
        }

        if (playerInRange && !canSeePlayer())
        {
            ChangeAnimation("Golem_lookAround");

        }
        else if (!isInMeleeRange)
        {
            if (bossArea.bounds.Contains(gameManager.instance.player.transform.position))
            {

                ChangeAnimation("Golem_walkForward");
                agent.SetDestination(gameManager.instance.player.transform.position);
            }
       
        }

        Debug.DrawLine(headPos.transform.position, gameManager.instance.player.transform.position);

        if (HP < startingHealth / 2)
        {
            StartCoroutine(iAmDeflecting());

        }

    }

    private void ChangeAnimation(string targetAnim, float crossFade = 0.2f, float switchThreshold = 0.8f)
    {
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);

        if (targetAnim == "Golem_walkForward")
        {
            agent.speed = sprintSpeed;
            switchThreshold -= 0.3f;
        }
        else
        {
            agent.speed = movementSpeed;
        }


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
                //isFighting = true;
                //isPursuing = true;
                //isSearching = false;
                facePlayer();

                agent.SetDestination(gameManager.instance.player.transform.position);
                if (isInMeleeRange && distanceToPlayer <= meleeRange)
                {
                    stopPursuing();

                    // perfrom melee
                    if (!isStomping)
                    {

                        isStomping = true;
                        ChangeAnimation("Golem_stompAttack");
                        isStomping = false;
                    }
                    else if (isStomping)
                    {
                        ChangeAnimation("Golem_intimidateOne");
                    }


                }

                agent.isStopped = false;
                agent.SetDestination(gameManager.instance.player.transform.position);

                if (playerInRange)
                {
                    //perform shooting/throwing
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
            //isPursuing = false;
            //isFighting = false;
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

    IEnumerator Stomp()
    {
        isStomping = true;
        agent.isStopped = true;
        agent.SetDestination(this.transform.position);
        ChangeAnimation("Golem_stompAttack");
        yield return new WaitForSeconds(1.5f);
        isStomping = false;
        agent.isStopped = false;

        if (distanceToPlayer >= meleeRange)
        {
            agent.SetDestination(gameManager.instance.player.transform.position);

        }
        //ChangeAnimation("Golem_walkForward");
        Debug.Log(currentAnimation);

    }
    public void createStompEffect()
    {
        Instantiate(stompEffect, stompPosition.transform.position, this.transform.rotation);
        audioManager.instance.PlayAud(stompSound[Random.Range(0, stompSound.Length)], stompSoundVol);

    }

    public void turnOnStompHitBox()
    {
        BoxCollider coll = particleHitBoxStart.GetComponent<BoxCollider>();
        if (coll.bounds.Contains(gameManager.instance.player.transform.position))
        {
            Debug.Log("Inbounds");
            gameManager.instance.playerScript.takeDamage(stompDamage);
        }
    }

    public void stopPursuing()
    {
        agent.SetDestination(this.transform.position);
        isPursuing = false;
        agent.speed = 0;
        agent.isStopped = true;
        //ChangeAnimation("Golem_idle");
    }

    void facePlayer()
    {
        Quaternion rot = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * facePlayerSpeed);
        //Debug.Log(currentAnimation);

    }

    public void takeDamage(int amount)
    {
        if(isDefending)
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
            enemyManager.instance.bossDefeated = true;
            isDead = true;

            enemyManager.instance.updateEnemyCount(-1);

            agent.isStopped = true;
            animator.enabled = false;
            this.enabled = false;

            audioManager.instance.PlayAud(deathSound[Random.Range(0, deathSound.Length)], deathSoundVol);

            StartCoroutine(destroyAfterSound());
            Instantiate(deathDrop, gameObject.transform.position + (Vector3.up * 1.15f), gameObject.transform.rotation);
            gameUIManager.instance.updateExperienceCount(actualExpGiven);

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
        //model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        //model.material.color = colorOrig;
        //Debug.Log(currentAnimation);

    }


    public void createProjectile()
    {
        Vector3 direction = gameManager.instance.player.transform.position - projectilePos.transform.position;
        direction.Normalize();
        Quaternion bulletRotation = Quaternion.LookRotation(direction);
        Instantiate(projectile, projectilePos.transform.position, bulletRotation);
        bulletEffect = projectile.AddComponent<ParticleSystem>();

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















    public void endDeflecting()
    {
        CapsuleCollider deflect = GetComponent<CapsuleCollider>();
        isDefending = false;
        deflect.isTrigger = false;
        shield.SetActive(false);
    }

    public void startDeflecting()
    {
        onDeflectCollisionEnter();
        shield.SetActive(true);
    }

    private void onDeflectCollisionEnter ()
    {
        CapsuleCollider deflect = GetComponent<CapsuleCollider>();
        isDefending = true;

        deflect.isTrigger = true;
        Damage[] damageObjects = FindObjectsOfType<Damage>();
        
        foreach (Damage damage in damageObjects)
        {
            if (deflect.bounds.Contains(damage.transform.position) && isDefending)
            {
                Rigidbody rb = damage.GetComponent<Rigidbody>();
                Damage amounts = damage.GetComponent<Damage>();

                damage.transform.rotation = damage.transform.rotation * Quaternion.Euler(0.0f, 180f, 0.0f);
                rb.useGravity = false;
                rb.velocity = this.transform.forward * amounts.speed;
                int delfectDamage = amounts.GetDamageAmount();
                damage.tag = "Enemy Bullet";
                damage.gameObject.layer = 7;
                rb.excludeLayers = 0;
                SphereCollider coll = damage.GetComponent<SphereCollider>();
                coll.excludeLayers = 0;
                coll.includeLayers = 3;
                amounts.SetDamageAmount(delfectDamage);
            }
        }
    }

    IEnumerator iAmDeflecting()
    {

        {
            startDeflecting();

            // show shield effect

            yield return new WaitForSeconds(deflectionTime);


            endDeflecting();

            // turn off shield effect


            yield return new WaitForSeconds(vulnerableTime * 2);

        }

    }

}
