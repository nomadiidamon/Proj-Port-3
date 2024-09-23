using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class playerController : MonoBehaviour, IDamage
{
    [Header("-----Components-----")]
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreMask;
    [SerializeField] GameObject shield;
    [SerializeField] Transform playerCenter;
    [SerializeField] cameraController camController;


    [Header("-----Attributes-----")]
    [Range(0, 100)][SerializeField] public int HP;
    [Range(0, 100)][SerializeField] public int Stamina;
    [Range(0, 5)][SerializeField] public float staminaRechargeDelay;
    [Range(1, 50)][SerializeField] public int speed;
    [Range(0, 20)][SerializeField] public int baseDamage;
    [SerializeField] public float upgradePercentage;
    [SerializeField] public int currentExperience;
    [Range(1, 3)][SerializeField] int jumpMax;
    [Range(8, 20)][SerializeField] int jumpSpeed;
    [Range(15, 30)][SerializeField] int gravity;
    [SerializeField] float fallDeathLevel = -50f;
    [SerializeField] float footStepRate = 0.3f;


    private int numberOfHealthUpgrades;
    public int numberOfPointsToUpgradeHealth = 150;

    private int numberOfStaminaUpgrades;
    public int numberOfPointsToUpgradeStamina = 100;

    private int numberOfSpeedUpgrades;
    public int numberOfPointsToUpgradeSpeed = 300;

    private int numberOfDamageUpgrades;
    public int numberOfPointsToUpgradeDamage = 500;
    public bool justUpgradedDamage;



    //[Range(2, 10)][SerializeField] int sprintMod;

    [Header("-----Dodge-----")]
    [SerializeField] float dodgeSpeed = 20f;
    [SerializeField] float dodgeDuration = 0.2f;
    [SerializeField] float dodgeCooldown = 1f;
    [SerializeField] int dodgeCost = 1;

    [Header("-----Guns-----")]
    public List<gunStats> gunList = new List<gunStats>();
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
    [Range(3, 30)][SerializeField] int maxObjectsCreated;
    [Range(1, 3)][SerializeField] int maxAlliesCreated;
    public List<GameObject> objectsCreated;
    public List<GameObject> alliesCreated;
    Vector3 objectHeldOriginalSize;
    public float allyHeldAggroRange;

    private jsonManager JsonManager;

    public Transform GetPlayerCenter()
    {
        return playerCenter;
    }
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






    public int GetNumberOfHealthUpgrades()
    {
        return numberOfHealthUpgrades;
    }
    public void SetNumberOfHealthUpgrades(int amount)
    {
        numberOfHealthUpgrades += amount;
    }
    public int GetNumberOfStaminaUpgrades()
    {
        return numberOfStaminaUpgrades;
    }
    public void SetNumberOfStaminaUpgrades(int amount)
    {
        numberOfStaminaUpgrades += amount;
    }
    public int GetNumberOfSpeedUpgrades()
    {
        return numberOfSpeedUpgrades;
    }
    public void SetNumberOfSpeedUpgrades(int amount)
    {
        numberOfSpeedUpgrades += amount;
    }
    public int GetNumberOfDamageUpgrades()
    {
        return numberOfDamageUpgrades;
    }
    public void SetNumberOfDamageUpgrades(int amount)
    {
        numberOfDamageUpgrades += amount;
    }

    public void updateAllGunsInInventory()
    {
        if (justUpgradedDamage)
        {
            foreach (gunStats gun in GetGunList())
            {
                if (!gun.isShield && !gun.isBlast && !gun.isCreator)
                {
                    gun.increaseDamage(baseDamage);
                }
            }

            justUpgradedDamage = false;
            changeGun();
        }
    }



    Vector3 move;
    Vector3 playerVel;

    private bool isDodging = false;
    private float dodgeTime = 0f;
    private float prevDodgeTime = -100f;


    int jumpCount;

    public int HPOrig;
    public int StaminaOrig;

    public int GetOriginalHpAmount() { return HPOrig; }
    public int GetOriginalStaminaAmount() { return StaminaOrig; }
    //bool isSprinting;
    bool isShooting;
    bool isPlayingSteps;
    bool isRechargingStamina;

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
    
    void Awake()
    {
        
    }
    void Start()
    {
       
        camController = FindObjectOfType<cameraController>();
        if (camController == null)
        {
            Debug.Log("Camera controller not found");
        }
        

        JsonManager = FindObjectOfType<jsonManager>();
        LoadGuns();
        HPOrig = HP;
        StaminaOrig = Stamina;
        updatePlayerUI();
        spawnPlayer();
        myCollider = shield.GetComponent<BoxCollider>();
        myCollider.enabled = false;
    }

    void Dodge()
    {
        if (Input.GetButtonDown("Dodge") && Time.time > prevDodgeTime + dodgeCooldown && !isDodging && Stamina >= dodgeCost)
        {
            Stamina -= dodgeCost;
            updatePlayerUI();
            StartCoroutine(DoDodge());

            if (!isRechargingStamina)
            {
                StartCoroutine(StaminaRecharge());
            }
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
    IEnumerator StaminaRecharge()
    {
        isRechargingStamina = true;

        while (Stamina < StaminaOrig)
        {
            yield return new WaitForSeconds(staminaRechargeDelay);
            Stamina++;
            updatePlayerUI();
        }
        isRechargingStamina = false;
    }

    public void spawnPlayer()
    {
        HP = HPOrig;
        Stamina = StaminaOrig;
        updatePlayerUI();
        controller.enabled = false;
        transform.position = gameManager.instance.playerSpawnPosition.transform.position;
        controller.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);

        if (!gameUIManager.instance.isPaused)
        {
            movement();
            selectGun();
            Dodge();
        }
        //sprint();

        fallDeath();
        updateAllGunsInInventory();
    }

    void fallDeath()
    {
        if (transform.position.y < fallDeathLevel)
        {
            HP = 0;
            updatePlayerUI();
            gameUIManager.instance.isPaused = false;
            gameUIManager.instance.youLose();
            
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

        //Debug.Log("Triggering camera shake!");
        camController.TriggerCameraShake(.3f, .5f); // to demonstrate camera shake

        updatePlayerUI();
        StartCoroutine(flashDamage());

        // I'm dead!
        if (HP <= 0)
        {
            gameUIManager.instance.isPaused = false;
            gameUIManager.instance.youLose();
        }
    }

    IEnumerator flashDamage()
    {
        gameUIManager.instance.flashDamageScreen.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gameUIManager.instance.flashDamageScreen.SetActive(false);
    }

    IEnumerator RestoreHealthScreen()
    {
        gameUIManager.instance.restoreHealthScreen.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        gameUIManager.instance.restoreHealthScreen.SetActive(false);
    }

    IEnumerator IncreaseDamageScreen()
    {
        gameUIManager.instance.increaseDamageScreen.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        gameUIManager.instance.increaseDamageScreen.SetActive(false);
    }

    IEnumerator RaiseSpeedScreen()
    {
        gameUIManager.instance.raiseSpeedScreen.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        gameUIManager.instance.raiseSpeedScreen.SetActive(false);
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
        if (gameManager.instance != null)
        {
            gameUIManager.instance.updatePlayerUI ();
        }
    }

    public void getGunStats(gunStats gun)
    {
        audioManager.instance.PlayAud(audWeapPickup, audWeapPickupVol);

        if (gunList.Contains(gun))
        {
            return;
        }
        else
        {
            gunList.Add(gun);

        }


       

        selectedGun = gunList.Count - 1;
        shootDamage = gun.shootDamage;
        bullet.GetComponent<Damage>().SetDamageAmount(shootDamage);
        shootDist = gun.shootDistance;
        shootRate = gun.shootRate;
        isCreator = gun.isCreator;
        isHoldingShield = gun.isShield;
        bullet = gun.bullet;

        if (isCreator)
        {
            objectHeldContainer.gameObject.SetActive(true);
            gameUIManager.instance.ShowCreatorGunPrompt();
        }
        else
        {
            objectHeldContainer.gameObject.SetActive(false);
            gameUIManager.instance.HideCreatorGunPrompt();
        }

        gunModel.GetComponent<MeshFilter>().sharedMesh = gun.gunModel.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gun.gunModel.GetComponent<MeshRenderer>().sharedMaterial;

        //gunModel.transform.localScale = gun.gunScale;
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
        bullet = gunList[selectedGun].bullet;
        bullet.GetComponent<Damage>().SetDamageAmount(shootDamage);
        shootDist = gunList[selectedGun].shootDistance;
        shootRate = gunList[selectedGun].shootRate;
        isCreator = gunList[selectedGun].isCreator;
        isHoldingShield = gunList[selectedGun].isShield;

        if (isCreator)
        {
            objectHeldContainer.gameObject.SetActive(true);
            gameUIManager.instance.ShowCreatorGunPrompt();
            
        }
        else
        {
            objectHeldContainer.gameObject.SetActive(false);
            gameUIManager.instance.HideCreatorGunPrompt();

        }

        gunModel.GetComponent<MeshFilter>().sharedMesh = gunList[selectedGun].gunModel.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunList[selectedGun].gunModel.GetComponent<MeshRenderer>().sharedMaterial;

        audioManager.instance.PlayAud(gunList[selectedGun].switchSound[Random.Range(0, gunList[selectedGun].switchSound.Length)], gunList[selectedGun].switchVolume);

    }
    
    public void AddGun(gunStats gun)
    {
        if (!gunList.Contains(gun))
        {
            gunList.Add(gun);
        }
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
                if (hit.collider.CompareTag("Creatable"))
                {
                    audioManager.instance.PlayCopyObjectSound();
                }
                else if (hit.collider.CompareTag("Enemy"))
                {
                    audioManager.instance.PlayCopyEnemySound();
                }

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

    

    public void SaveGuns()
    {
        JsonManager.SaveGunList(gunList);
    }

    public void LoadGuns()
    {
        gunList = JsonManager.LoadGunList();

        if (gunList.Count > 0 && selectedGun >= 0 && selectedGun < gunList.Count)
        {
            var currentGun = gunList[selectedGun];
            shootRate = currentGun.shootRate;
            

        }


    }

    



    private void OnApplicationQuit()
    {
        SaveGuns();
    }

}