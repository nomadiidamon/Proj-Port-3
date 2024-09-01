using System.Collections;
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
    [Range(0, 10)][SerializeField] int startingHealth;

    [Header("-----Factors-----")]
    [SerializeField] int viewAngle;
    [SerializeField] int facePlayerSpeed;
    [SerializeField] int roamDistance;
    [SerializeField] int roamTimer;
    [SerializeField] int animSpeedTrans;
    [SerializeField] float projectileShootRate;
    [SerializeField] int projectileDistance;


    bool isInMeleeRange;
    bool isThrowing;
    bool isDefending;
    bool playerInRange;
    bool isRoaming;
    bool isPlayingSteps;
    bool isDead = false;

    float angleToPlayer;
    float stoppingDistanceOriginal;

    Vector3 playerDir;
    Vector3 startingPosition;

    Color colorOrig;


    void Start()
    {
        HP = startingHealth;
        colorOrig = model.sharedMaterial.color;
        gameManager.instance.updateGameGoal(1);
        stoppingDistanceOriginal = agent.stoppingDistance;
        startingPosition = transform.position;
        updateHPBar();
    }

    void Update()
    {
        float agentSpeed = agent.velocity.normalized.magnitude;

        if (playerInRange && !canSeePlayer())
        {
            if (!isRoaming && agent.remainingDistance < 0.05f)
                StartCoroutine(roam());
        }
        else if (!playerInRange)
        {
            if (!isRoaming && agent.remainingDistance < 0.05f)
                StartCoroutine(roam());
        }
    }

    IEnumerator roam()
    {
        isRoaming = true;
        yield return new WaitForSeconds(roamTimer);

        agent.stoppingDistance = 0;
        Vector3 randomDistance = Random.insideUnitSphere * roamDistance;
        randomDistance += startingPosition;

        NavMeshHit hit;
        NavMesh.SamplePosition(randomDistance, out hit, roamDistance, 1);
        agent.SetDestination(hit.position);

        isRoaming = false;
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
                Debug.Log("I see you!");

                agent.SetDestination(gameManager.instance.player.transform.position);
                if (!isThrowing)
                    StartCoroutine(Throw());

                if (agent.remainingDistance <= agent.stoppingDistance)
                    facePlayer();

                agent.stoppingDistance = stoppingDistanceOriginal;
                return true;
            }
        }
        agent.stoppingDistance = 0;
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
 
        yield return new WaitForSeconds(projectileShootRate);
        isThrowing = false;
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
            Debug.Log("Player in range");

            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player out of range");

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
