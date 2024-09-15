using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;

public class playerController : MonoBehaviour, IDamage
{
    [Header("-----Components-----")]
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreMask;
    [SerializeField] GameObject shield;
    [SerializeField] Transform playerCenter;

    public Transform GetPlayerCenter()
    {
        return playerCenter;
    }

    [Header("-----Attributes-----")]
    [Range(0, 100)][SerializeField] public int HP;
    [Range(1, 50)][SerializeField] int speed;
    //[Range(2, 10)][SerializeField] int sprintMod;
    [Range(1, 3)][SerializeField] int jumpMax;
    [Range(8, 20)][SerializeField] int jumpSpeed;
    [Range(15, 30)][SerializeField] int gravity;
    [SerializeField] float fallDeathLevel = -50f;
    [SerializeField] float footStepRate = 0.3f;

    [Header("-----Dodge-----")]
    [SerializeField] float dodgeSpeed = 20f;
    [SerializeField] float dodgeDuration = 0.2f;
    [SerializeField] float dodgeCooldown = 1f;

    [Header("-----Guns-----")]
    [SerializeField] List<gunStats> gunList = new List<gunStats>();
    [SerializeField] GameObject gunModel;
    [SerializeField] GameObject muzzleFlash;
    [SerializeField] GameObject deflectionFlash;
    [SerializeField] GameObject bullet;
    [SerializeField] Transform shootPos;
    [SerializeField] int shootDamage;
    [SerializeField] int shootDist;
    [SerializeField] float shootRate;
    [SerializeField] float deflectionSpeed;
    [SerializeField] float blastForce;


    [Header("-----Sounds-----")]
    [SerializeField] AudioClip[] audJump;
    [Range(0, 1)][SerializeField] float audJumpVol;
    [SerializeField] AudioClip[] audHurt;
    [Range(0, 1)][SerializeField] float audHurtVol;
    [SerializeField] AudioClip[] audSteps;
    [Range(0, 1)][SerializeField] float audStepsVol;
    [SerializeField] AudioClip audWeapPickup;
    [Range(0, 1)][SerializeField] float audWeapPickupVol;

    [Header("-----Creator Gun-----")]
    public GameObject objectHeld;           // object ready to shoot
    [SerializeField] Transform objectHeldContainer;
    [Range(3, 10)][SerializeField] int maxObjectsCreated;
    [Range(1, 3)][SerializeField] int maxAlliesCreated;
    public List<GameObject> objectsCreated;
    public List<GameObject> alliesCreated;
    Vector3 objectHeldOriginalSize;
    public float allyHeldAggroRange;

    public Transform GetShootPosition()
    {
        return shootPos;
    }
    public int GetMaxObjectsCreated()
    {
        return maxObjectsCreated;
    }
    public int GetMaxAlliesCreated()
    {
        return maxAlliesCreated;
    }

    Vector3 move;
    Vector3 playerVel;

    private bool isDodging = false;
    private float dodgeTime = 0f;
    private float prevDodgeTime = -100f;


    int jumpCount;
    public int HPOrig;
    public int GetOriginalHpAmount() { return HPOrig; }
    //bool isSprinting;
    bool isShooting;
    bool isPlayingSteps;

    //public bool sprintToggle;
    //bool sprintingPressed;

    public int selectedGun;
    public bool isCreator;
    public bool isSwimming;
    public bool isDeflecting;
    public bool isHoldingShield;
    private BoxCollider myCollider;

    public Vector3 GetObjectHeldOriginalSize()
    {
        return objectHeldOriginalSize;
    }

    public List<gunStats> GetGunList() { return gunList; }

    // Start is called before the first frame update
    void Start()
    {
        HPOrig = HP;
        updatePlayerUI();
        spawnPlayer();
        myCollider = shield.GetComponent<BoxCollider>();
        myCollider.enabled = false;
    }

    void Dodge()
    {
        if (Input.GetButtonDown("Dodge") && Time.time > prevDodgeTime + dodgeCooldown && !isDodging)
        {
            StartCoroutine(DoDodge());
        }
    }

    IEnumerator DoDodge()
    {
        isDodging = true;
        dodgeTime = dodgeDuration;
        prevDodgeTime = Time.time;

        Vector3 dodgeDirection = move.normalized;

        while (dodgeTime > 0)
        {
            controller.Move(dodgeDirection * dodgeSpeed * Time.deltaTime);
            dodgeTime -= Time.deltaTime;
            yield return null;
        }

        isDodging = false;
    }

    public void spawnPlayer()
    {
        HP = HPOrig;
        updatePlayerUI();
        controller.enabled = false;
        transform.position = gameManager.instance.playerSpawnPosition.transform.position;
        controller.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);

        if (!gameManager.instance.isPaused)
        {
            movement();
            selectGun();
            Dodge();
        }
        //sprint();

        fallDeath();
    }

    void fallDeath()
    {
        if (transform.position.y < fallDeathLevel)
        {
            HP = 0;
            updatePlayerUI();
            gameManager.instance.isPaused = false;
            gameManager.instance.youLose();
            
        }
    }



    void movement()
    {
        if (controller.isGrounded)
        {
            jumpCount = 0;
            playerVel = Vector3.zero;
        }

        move = Input.GetAxis("Vertical") * transform.forward +
               Input.GetAxis("Horizontal") * transform.right;
        controller.Move(move * speed * Time.deltaTime);

        if (isSwimming && Input.GetButton("Jump"))
        {
            playerVel.y = jumpSpeed;
        }
        else if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            jumpCount++;
            playerVel.y = jumpSpeed;
            audioManager.instance.PlayAud(audJump[Random.Range(0, audJump.Length)], audJumpVol);
        }

        if (Input.GetButtonDown("Save Object") && isCreator)
        {
            saveObjectBullet();
        }

        if (Input.GetButtonDown("Reset Created Objects"))
        {
            clearObjectsCreated();
        }

        controller.Move(playerVel * Time.deltaTime);
        playerVel.y -= gravity * Time.deltaTime;

        if (gunList.Count < 1)
        {

        }
        else
        {
            if (Input.GetButton("Shoot") && !isShooting)
                StartCoroutine(shoot());

        }

        if (controller.isGrounded && move.magnitude > 0.3f && !isPlayingSteps)
        {
            StartCoroutine(playStep());
        }
    }

    IEnumerator playStep()
    {
        isPlayingSteps = true;

        audioManager.instance.PlayAud(audSteps[Random.Range(0, audSteps.Length)], audStepsVol);
        //if (!isSprinting)
        //{
        //    yield return new WaitForSeconds(0.5f);
        //}
        //else
        //{
        yield return new WaitForSeconds(footStepRate);
        //}

        isPlayingSteps = false;

    }

    //void sprint()
    //{
    //    if (sprintToggle)
    //    {
    //        if (Input.GetButtonDown("Sprint"))
    //        {
    //            if (isSprinting)
    //            {
    //                isSprinting = false;
    //            }
    //            else if (!isSprinting)
    //            {
    //                isSprinting = true;
    //            }
    //        }
    //        if (Input.GetButtonDown("Sprint") && isSprinting)
    //        {
    //            speed *= sprintMod;
    //        }
    //        else if (Input.GetButtonDown("Sprint") && !isSprinting)
    //        {
    //            speed /= sprintMod;
    //        }
    //    }
    //    else
    //    {
    //        if (Input.GetButtonDown("Sprint"))
    //        {
    //            speed *= sprintMod;
    //            isSprinting = true;
    //        }
    //        else if (Input.GetButtonUp("Sprint"))
    //        {
    //            speed /= sprintMod;
    //            isSprinting = false;
    //        }
    //    }
    //}

    //public void SetSprintToggle(bool toggle)
    //{
    //    sprintToggle = toggle;

    //    if (!sprintToggle && isSprinting)
    //    {
    //        speed /= sprintMod;
    //        isSprinting = false;
    //    }
    //}

    IEnumerator shoot()
    {
        if (!gunList[selectedGun].isShield && !gunList[selectedGun].isBlast)
        {
            isShooting = true;

            StartCoroutine(flashMuzzle());
            audioManager.instance.PlayAud(gunList[selectedGun].shootSound[Random.Range(0, gunList[selectedGun].shootSound.Length)], gunList[selectedGun].shootVolume);
            Instantiate(bullet, shootPos.position, shootPos.rotation);
            yield return new WaitForSeconds(shootRate);
            isShooting = false;

        }
        else if (gunList[selectedGun].isShield)
        {
            isDeflecting = true;
            isShooting = true;

            StartCoroutine(Deflecting(myCollider));
            StartCoroutine(flashDeflection());
            bool isPlayingDeflection = true;
            audioManager.instance.PlayAud(gunList[selectedGun].shootSound[Random.Range(0, gunList[selectedGun].shootSound.Length)], gunList[selectedGun].shootVolume);
            if (isPlayingDeflection)
            {
                yield return new WaitForSeconds(0.5f);
                isPlayingDeflection = false;
            }
            isShooting = false;
            isDeflecting = false;
            yield return new WaitForSeconds(0);
        }

        else if (gunList[selectedGun].isBlast)
        {
            isShooting = true;
            gunBlast();
            StartCoroutine(flashMuzzle());
            audioManager.instance.PlayAud(gunList[selectedGun].shootSound[Random.Range(0, gunList[selectedGun].shootSound.Length)], gunList[selectedGun].shootVolume);
            yield return new WaitForSeconds(shootRate);
            isShooting = false;

        }

    }

    IEnumerator Deflecting(BoxCollider coll)
    {
        coll.enabled = true;
        //Debug.Log("Collider is active!");

        yield return new WaitForSeconds(shootRate);
        coll.enabled = false;
        //Debug.Log("Collider is off!");

    }

    IEnumerator flashMuzzle()
    {
        muzzleFlash.SetActive(true);
        yield return new WaitForSeconds(.05f);
        muzzleFlash.SetActive(false);
    }

    IEnumerator flashDeflection()
    {

        deflectionFlash.SetActive(true);
        yield return new WaitForSeconds(.05f);
        deflectionFlash.SetActive(false);
    }

    public void takeDamage(int amount)
    {
        if (isDeflecting)
        {
            amount = 0;
            return;
        }

        HP -= amount;
        //Debug.Log("Ouch!");
        if (HP < 0) { HP = 0; }

        audioManager.instance.PlayAud(audHurt[Random.Range(0, audHurt.Length)], audHurtVol);

        updatePlayerUI();
        StartCoroutine(flashDamage());

        // I'm dead!
        if (HP <= 0)
        {
            gameManager.instance.isPaused = false;
            gameManager.instance.youLose();
        }
    }

    IEnumerator flashDamage()
    {
        gameManager.instance.flashDamageScreen.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gameManager.instance.flashDamageScreen.SetActive(false);
    }

    IEnumerator RestoreHealthScreen()
    {
        gameManager.instance.restoreHealthScreen.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        gameManager.instance.restoreHealthScreen.SetActive(false);
    }

    IEnumerator IncreaseDamageScreen()
    {
        gameManager.instance.increaseDamageScreen.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        gameManager.instance.increaseDamageScreen.SetActive(false);
    }

    IEnumerator RaiseSpeedScreen()
    {
        gameManager.instance.raiseSpeedScreen.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        gameManager.instance.raiseSpeedScreen.SetActive(false);
    }

    public void RestoreHealth()
    {
        updatePlayerUI();
        StartCoroutine(RestoreHealthScreen());
    }

    public void IncreaseDamage()
    {
        StartCoroutine(IncreaseDamageScreen());
    }

    public void RaiseSpeed()
    {
        StartCoroutine(RaiseSpeedScreen());
    }

    public void updatePlayerUI()
    {
        gameManager.instance.playersHealthPool.fillAmount = (float)HP / HPOrig;
    }

    public void getGunStats(gunStats gun)
    {
        audioManager.instance.PlayAud(audWeapPickup, audWeapPickupVol);
        gunList.Add(gun);
        selectedGun = gunList.Count - 1;
        shootDamage = gun.shootDamage;
        bullet.GetComponent<Damage>().SetDamageAmount(shootDamage);
        shootDist = gun.shootDistance;
        shootRate = gun.shootRate;
        isCreator = gun.isCreator;
        isHoldingShield = gun.isShield;

        if (isCreator)
        {
            objectHeldContainer.gameObject.SetActive(true);
        }
        else
        {
            objectHeldContainer.gameObject.SetActive(false);
        }

        gunModel.GetComponent<MeshFilter>().sharedMesh = gun.gunModel.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gun.gunModel.GetComponent<MeshRenderer>().sharedMaterial;

        gunModel.transform.localScale = gun.gunScale;
    }

    void selectGun()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && selectedGun < gunList.Count -1)
        {
            selectedGun++;
            changeGun();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && selectedGun > 0)
        {
            selectedGun--;
            changeGun();
        }
    }

    void changeGun()
    {
        shootDamage = gunList[selectedGun].shootDamage;
        bullet.GetComponent<Damage>().SetDamageAmount(shootDamage);
        shootDist = gunList[selectedGun].shootDistance;
        shootRate = gunList[selectedGun].shootRate;
        isCreator = gunList[selectedGun].isCreator;
        isHoldingShield = gunList[selectedGun].isShield;

        if (isCreator)
        {
            objectHeldContainer.gameObject.SetActive(true);
        }
        else
        {
            objectHeldContainer.gameObject.SetActive(false);
        }

        gunModel.GetComponent<MeshFilter>().sharedMesh = gunList[selectedGun].gunModel.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunList[selectedGun].gunModel.GetComponent<MeshRenderer>().sharedMaterial;

        audioManager.instance.PlayAud(gunList[selectedGun].switchSound[Random.Range(0, gunList[selectedGun].switchSound.Length)], gunList[selectedGun].switchVolume);

    }

    public int GetHealth()
    {
        return HP;
    }
    public void SetHealth(int amount)
    {
        HP = amount;
    }
    public int GetSpeed()
    {
        return speed;
    }
    public void SetSpeed(int amount)
    {
        speed = amount;
    }
    public int GetDamage()
    {
        return shootDamage;
    }
    public void SetDamage(int amount)
    {
        shootDamage = amount;
    }
    public int GetGravity()
    {
        return gravity;
    }
    public void SetGravity(int amount)
    {
        gravity = amount;
        jumpSpeed = amount / 2;
    }
    public void SetGravity(int gravityAmount, int jumpSpeedAmount)
    {
        gravity = gravityAmount;
        jumpSpeed = jumpSpeedAmount;
    }

    public void saveObjectBullet()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 50, ~ignoreMask))
        {
            Debug.Log(hit.collider.name);
            if (hit.collider.CompareTag("Creatable") || hit.collider.CompareTag("Enemy"))
            {
                if (objectHeld)
                {
                    Destroy(objectHeld);    // if an object is already held, destroy it for the next one to replace it
                }
                objectHeld = Instantiate(hit.collider.gameObject, objectHeldContainer.position, Quaternion.identity);            // create a copy of the chosen object and assign it to objectHeld
                objectHeldOriginalSize = objectHeld.transform.localScale;                    // save the original size of the copied object before its shrunken to fit in the object container
                objectHeld.transform.parent = objectHeldContainer;                           // set the parent of the objectHeld to the conainter so it stays in it
                enemyAI enemyAI = objectHeld.GetComponent<enemyAI>();
                if (enemyAI != null)
                {
                    if (hit.collider.CompareTag("Enemy"))
                    {
                        objectHeld.tag = "Ally";
                    }
                    allyHeldAggroRange = enemyAI.GetComponent<SphereCollider>().radius;
                    Destroy(enemyAI);
                }
                disableGameObject(objectHeld);
                objectHeld.transform.localScale *= 0.1f;
            }
        }
    }

    public void disableGameObject(GameObject gameObject)
    {
        foreach (Component component in gameObject.GetComponents<Component>())
        {
            if (component is Behaviour behaviour)
            { behaviour.enabled = false; }
        }
        foreach (Collider collider in gameObject.GetComponents<Collider>())
        {
            collider.enabled = false;
        }
    } 
    public void enableGameObject(GameObject gameObject)
    {
        foreach (Component component in gameObject.GetComponents<Component>())
        {
            if (component is Behaviour behaviour && component is not enemyAI)
            { 
                behaviour.enabled = true; 
            }
        }
        foreach (Collider collider in gameObject.GetComponents<Collider>())
        {
            collider.enabled = true;
        }
    }

    void gunBlast()
    {
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 backwardDirection = -cameraForward.normalized;

        playerVel += backwardDirection * blastForce;

        if (controller.isGrounded)
        {
            playerVel.y = Mathf.Max(playerVel.y, 0.5f); 
        }

        StartCoroutine(pushback(backwardDirection));
    }

    IEnumerator pushback(Vector3 direction)
    {
        float pushDuration = 0.2f;
        float elapsedTime = 0f;
        while (elapsedTime < pushDuration)
        {
            controller.Move(direction * (blastForce / 10) * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    public void addToCreatedLists(GameObject createdObject)
    {
        if (createdObject.CompareTag("Ally"))
        {
            if (alliesCreated.Count < maxAlliesCreated)
            alliesCreated.Add(createdObject);
        }
        else
        {           
            if (objectsCreated.Count < maxObjectsCreated)
            objectsCreated.Add(createdObject);              
        }
    }
    public void clearObjectsCreated()
    {
        foreach (GameObject objectCreated in objectsCreated)
        {
            Destroy(objectCreated);
        }
        objectsCreated.Clear();
    }

    public gunStats getGunName(string gunName)
    {
        foreach (gunStats gun in gunList)
        {
            if (gun.gunName.Equals(gunName, System.StringComparison.OrdinalIgnoreCase))
            {
                return gun;
            }
        }
        Debug.LogWarning($"Gun with name {gunName} not found!");
        return null;
    }

}