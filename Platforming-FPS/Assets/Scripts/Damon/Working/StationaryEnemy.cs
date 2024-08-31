using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StationaryEnemy : MonoBehaviour, IDamage
{
    [Header("-----Components-----")]
    [SerializeField] Renderer model;
    //[SerializeField] Animator animator;
    [SerializeField] Transform shootPos;
    [SerializeField] Transform headPos;
    [SerializeField] GameObject projectile;
    [SerializeField] GameObject muzzleFlash;
    [SerializeField] Image hpBar;


    [Header("-----Attributes-----")]
    [Range(0, 100)][SerializeField] int HP;
    [Range(0, 720)][SerializeField] int viewAngle;
    [Range(0, 500)][SerializeField] int facePlayerSpeed;
    [Range(0, 10)][SerializeField] int animSpeedTrans;
    [Range(0, 10)][SerializeField] float flashDamageTimer;
    [Range(0, 10)][SerializeField] float shootRate;

    bool isShooting;
    bool playerInRange;


    float angleToPlayer;
    GameObject player;
    Vector3 playerDir;

    Color colorOrig;
    private int HP_Original;


    void Start()
    {
        HP_Original = HP;
        colorOrig = model.material.color;

        gameManager.instance.updateGameGoal(1);
        player = gameManager.instance.player;

        updateHPBar();
    }

    void Update()
    {
        if (playerInRange)
        {
            Debug.DrawRay(shootPos.position, transform.forward * 10, Color.blue);
            if (canSeePlayer())
            { 
                Debug.Log("Shooting in direction: " + playerDir);
                if (!isShooting)
                    StartCoroutine(Shoot());

                facePlayer();
            }
        }
    }

    bool canSeePlayer()
    {
        Vector3 slightUp = new Vector3(0, 1.25f, 0);
        playerDir = (player.transform.position + slightUp) - headPos.position;
        Debug.DrawLine(headPos.position, player.transform.position + slightUp, Color.red);
        angleToPlayer = Vector3.Angle(playerDir, transform.position);

        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDir, out hit))
        {
            Debug.Log("In range");
            if (!hit.collider.CompareTag("Environment"))
            {
                return true;
            }
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
        HP -= amount;
        updateHPBar();
        StartCoroutine(flashRed());

        if (HP <= 0)
        {
            gameManager.instance.updateGameGoal(-1);
            Destroy(gameObject);
        }
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(flashDamageTimer);
        model.material.color = colorOrig;
    }

    IEnumerator Shoot()
    {
        isShooting = true;

        Quaternion bulletRotation = Quaternion.LookRotation(playerDir);
        StartCoroutine(flashMuzzle());
        Instantiate(projectile, shootPos.transform.position, bulletRotation);

        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    IEnumerator flashMuzzle()
    {
        muzzleFlash.SetActive(true);
        muzzleFlash.GetComponent<Light>().enabled = true;
        yield return new WaitForSeconds(.1f);
        muzzleFlash.GetComponent<Light>().enabled = false;
        muzzleFlash.SetActive(false);

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
        if (hpBar != null)
        {
            hpBar.fillAmount = (float)HP / HP_Original;
        }
    }


}