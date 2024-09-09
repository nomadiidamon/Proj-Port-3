using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class flyingEnemyAI : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;
    [SerializeField] Transform shootPos;
    [SerializeField] Transform headPos;
    [SerializeField] Image hpbar;
    [SerializeField] AudioSource audEnemy;

    private int HP;
    [SerializeField] int startingHealth;
    [SerializeField] int viewAngle;
    [SerializeField] float facePlayerSpeed;
    [SerializeField] float shootRate;
    [SerializeField] GameObject bullet;
    [SerializeField] LayerMask detectionLayerMask;

    [SerializeField] AudioClip[] deathSound;
    [Range(0, 1)][SerializeField] float deathSoundVol;

    bool isShooting;
    bool playerInRange;
    private bool isDead = false;

    Vector3 playerDir;
    Color colorOrig;

    // Start is called before the first frame update
    void Start()
    {
        HP = startingHealth;
        colorOrig = model.material.color;
        gameManager.instance.updateGameGoal(1);
        updateHPBar();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange)
        {
            if (canSeePlayer())
            {
                // Continuously face the player
                facePlayer();

                // Start shooting if not already shooting
                if (!isShooting)
                {
                    StartCoroutine(shoot());
                }
            }
        }
    }

    bool canSeePlayer()
    {
        playerDir = gameManager.instance.player.transform.position - headPos.position;
        float angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        if (angleToPlayer <= viewAngle)
        {
            if (Physics.Raycast(headPos.position, playerDir, out RaycastHit hit, Mathf.Infinity, detectionLayerMask))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    return true; // Player is detected
                }
            }
        }
        return false; // Player not detected
    }

    void facePlayer()
    {
        Vector3 directionToPlayer = (gameManager.instance.player.transform.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);

        Debug.Log($"Current Rotation: {transform.rotation.eulerAngles}");
        Debug.Log($"Target Rotation: {targetRotation.eulerAngles}");

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, facePlayerSpeed * Time.deltaTime);
    }

    IEnumerator shoot()
    {
        isShooting = true;
        createProjectile();
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    public void createProjectile()
    {
        Vector3 direction = gameManager.instance.player.transform.position - shootPos.transform.position;
        direction.Normalize();
        Quaternion bulletRotation = Quaternion.LookRotation(direction);
        Instantiate(bullet, shootPos.transform.position, bulletRotation);
    }

    public void takeDamage(int amount)
    {
        if (isDead) return;

        HP -= amount;
        Debug.Log("Soldier took " + amount + " damage");
        updateHPBar();
        StartCoroutine(flashRed());

        if (HP <= 0)
        {
            isDead = true;
            gameManager.instance.updateGameGoal(-1);
            this.enabled = false;
            playDeathAudio(deathSound[Random.Range(0, deathSound.Length)], deathSoundVol);
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
        }
    }

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

