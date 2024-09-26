using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;
using UnityEngine.UI;

public class parasiteAI : MonoBehaviour, IDamage
{
    [Header("-----Stats-----")]
    [SerializeField] int startingHealth;
    [SerializeField] int facePlayerSpeed;
    [SerializeField] float shootRate;
    [SerializeField] float swipeRate;

    [Header("-----Body-----")]
    [SerializeField] Renderer model;
    [SerializeField] Animator animator;
    [SerializeField] Transform shootPos;
    [SerializeField] Transform headPos;
    [SerializeField] AudioSource audEnemy;
    [SerializeField] GameObject bullet;
    [SerializeField] Image hpbar;
    string currentAnimation;

    private int HP;

    [SerializeField] int animSpeedTrans;
    [SerializeField] Transform targetPosition;

    //lecture shoot angle
    //[SerializeField] int shootAngle;

    [SerializeField] AudioClip[] deathSound;
    [Range(0, 1)][SerializeField] float deathSoundVol;

    bool isShooting;
    bool isSwiping;
    bool isIdle = false;
    private bool isDead = false;

    Vector3 playerDir;

    Color colorOrig;

    // Start is called before the first frame update
    void Start()
    {
        HP = startingHealth;
        colorOrig = model.material.color;
        enemyManager.instance.updateEnemyCount(1);
        updateHPBar();
    }

    // Update is called once per frame
    void Update()
    {
        playerDir = targetPosition.position - headPos.position;          // look for player always

        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDir, out hit))
        {
            // Debug.DrawRay(headPos.position, playerDir);
            // Debug.Log("Parasite sees: " + hit.collider.name);
            if (hit.collider.CompareTag("Player") && !isIdle)
            {
                //if (!isSwiping && hit.distance < 5)          // SWIPE OFF
                //{                                            // SWIPE OFF
                //    StartCoroutine(swipe());                 // SWIPE OFF
                //}                                            // SWIPE OFF
                //else if (!isShooting && !isSwiping)          // SWIPE OFF
                //{                                            // SWIPE OFF
                        StartCoroutine(shoot());               // SWIPE OFF
                //}                                            // SWIPE OFF

                facePlayer();
            }
        }
    }

    int getHealth()
    {
        return HP;
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

        Debug.Log("Parasite " + amount + " damage");
        updateHPBar();
        StartCoroutine(flashRed());

        if (HP <= 0)
        {
            isDead = true;

            enemyManager.instance.updateEnemyCount(-1);

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
        shootPos.LookAt(targetPosition.position);

        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }
    IEnumerator swipe()
    {
        isSwiping = true;
        animator.SetTrigger("Swipe");
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        isSwiping = false;
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

    public void createProjectile()
    {
        Vector3 direction = gameManager.instance.player.transform.position - shootPos.transform.position;
        direction.Normalize();
        Quaternion bulletRotation = Quaternion.LookRotation(direction);
        //Debug.Log("Pew!");
        Instantiate(bullet, shootPos.transform.position, bulletRotation);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(this.GetComponent<SphereCollider>());
            StartCoroutine(flyUp());
        }
    }
    IEnumerator flyUp()
    {
        Vector3 flyUp = new Vector3(200, 150, 0);
        float flyUpSpeed = 100;
        float timer = 0f;

        while (timer < flyUpSpeed)
        {
            timer += Time.deltaTime;

            isIdle = true;
            animator.ResetTrigger("Shoot");
            animator.SetTrigger("Idle");
            transform.position = Vector3.Lerp(transform.position, flyUp, timer / flyUpSpeed);
            yield return null;
            animator.ResetTrigger("Idle");
            isIdle = false;
        }
    }
}

