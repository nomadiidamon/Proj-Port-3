using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class gameUIManager : MonoBehaviour
{
    public static gameUIManager instance;

    public GameObject menuActive;
    public GameObject menuPause;
    [SerializeField] public GameObject menuWin;
    [SerializeField] public GameObject menuLose;
    [SerializeField] public GameObject menuAreYouSure;
    [SerializeField] public GameObject CreatorGunPrompt;
    [SerializeField] public GameObject UpgradeMenu;

    public GameObject menuSettings;
    public GameObject checkpointReachedMessage;
    public GameObject checkpointMenu;
    public Image playersHealthPool;
    public Image playersStaminaPool;
    public TMP_Text RespawnCount;
    public TMP_Text ExperienceCount;
    public TMP_Text uiPrompt;

    public TMP_Text currentHealth;
    public TMP_Text pointsNeededForHealth;
    public TMP_Text currentStamina;
    public TMP_Text pointsNeededForStamina;
    public TMP_Text currentSpeed;
    public TMP_Text pointsNeededForSpeed;
    public TMP_Text currentDamage;
    public TMP_Text pointsNeededForDamage;

    public GameObject flashDamageScreen;
    public GameObject restoreHealthScreen;
    public GameObject increaseDamageScreen;
    public GameObject raiseSpeedScreen;
    public GameObject underwaterOverlay;
    playerController player;

    public bool isPaused;


    public class upgradeManager
    {
        public int currPlayerExp;

        public int maxHP;
        public int HpUpgrades;
        public int pointsForHealth;
        public int healthIncreaseAmount;
        
        public int maxStamina;
        public int StaminaUpgrades;
        public int pointsForStamina;
        public int staminaIncreaseAmount;

        public int maxSpeed;
        public int speedUpgrades;
        public int pointsForSpeed;
        public int speedIncreaseAmount;

        public int maxDamage;
        public int damageUpgrades;
        public int pointsForDamage;
        public int damageIncreaseAmount;
    }

    public upgradeManager upgradeStats;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (gameUIManager.instance.menuActive == null)
            {
                statePause();
                gameUIManager.instance.menuActive = gameUIManager.instance.menuPause;
                gameUIManager.instance.menuActive.SetActive(isPaused);

            }

            else if (gameUIManager.instance.menuActive == gameUIManager.instance.menuPause)
            {
                stateUnpause();
            }

            else if (gameUIManager.instance.menuActive == gameUIManager.instance.menuSettings)
            {

                stateUnpause();


            }
        }

        if (CheckForUpgradeMenu())
        {
            updateUpgradeMenu();
        }

    }

    public bool CheckForUpgradeMenu()
    {
            if (checkpointMenu.activeSelf && Input.GetButtonDown("Interact"))
            {
                Debug.Log("opening the menu");
                checkpointMenu.gameObject.SetActive(false);
                statePause();
                menuActive = UpgradeMenu;
                menuActive.SetActive(true);
                return true;
            }
            return false;
    }

    public void updateUpgradeMenu()
    {
        upgradeStats = new upgradeManager();
        upgradeStats.currPlayerExp = gameManager.instance.playerScript.currentExperience;

        upgradeStats.maxHP = gameManager.instance.playerScript.HPOrig;
        upgradeStats.HpUpgrades = gameManager.instance.playerScript.GetNumberOfHealthUpgrades();
        upgradeStats.pointsForHealth = gameManager.instance.playerScript.numberOfPointsToUpgradeHealth;

        upgradeStats.maxStamina = gameManager.instance.playerScript.StaminaOrig;
        upgradeStats.StaminaUpgrades = gameManager.instance.playerScript.GetNumberOfStaminaUpgrades();
        upgradeStats.pointsForStamina = gameManager.instance.playerScript.numberOfPointsToUpgradeStamina;

        upgradeStats.maxSpeed = gameManager.instance.playerScript.speed;
        upgradeStats.speedUpgrades = gameManager.instance.playerScript.GetNumberOfSpeedUpgrades();
        upgradeStats.pointsForSpeed = gameManager.instance.playerScript.numberOfPointsToUpgradeSpeed;

        upgradeStats.maxDamage = gameManager.instance.playerScript.baseDamage;
        upgradeStats.damageUpgrades = gameManager.instance.playerScript.GetNumberOfDamageUpgrades();
        upgradeStats.pointsForDamage = gameManager.instance.playerScript.numberOfPointsToUpgradeDamage;



        pointsNeededForHealth.text = upgradeStats.pointsForHealth.ToString("F0");
        pointsNeededForSpeed.text = upgradeStats.pointsForSpeed.ToString("F0");
        pointsNeededForDamage.text = upgradeStats.pointsForDamage.ToString("F0");

        currentHealth.text = upgradeStats.maxHP.ToString("F0");
        currentSpeed.text = upgradeStats.maxSpeed.ToString("F0");
        currentDamage.text = upgradeStats.maxDamage.ToString("F0");



        gameManager.instance.playerScript.updatePlayerUI();

    }




    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

        

        if (isPaused)
        {
            stateUnpause();
        }

    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void statePause()
    {
        Debug.Log("Paused");
        isPaused = true;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void stateUnpause()
    {


        Debug.Log("Unpaused");
        isPaused = false;
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        gameUIManager.instance.menuActive.SetActive(isPaused);
        gameUIManager.instance.menuActive = null;


    }



    public void youLose()
    {
        Debug.Log("You Lose");
        if (isPaused)
        {
            isPaused = !isPaused;
            gameUIManager.instance.menuActive = null;
        }
        statePause();
        gameUIManager.instance.menuActive = gameUIManager.instance.menuLose;
        gameUIManager.instance.menuActive.SetActive(isPaused);

    }


    public void updateRespawnCount(int amount)
    {
        gameManager.instance.respawns += amount;
        gameUIManager.instance.RespawnCount.text = gameManager.instance.respawns.ToString("F0");
    }

    public void updateExperienceCount(int amount)
    {
        gameManager.instance.playerScript.currentExperience += amount;
        ExperienceCount.text = gameManager.instance.playerScript.currentExperience.ToString("F0");
    }

    public void updatePlayerUI()
    {
        playersHealthPool.fillAmount = (float)gameManager.instance.playerScript.HP / gameManager.instance.playerScript.HPOrig;
        playersStaminaPool.fillAmount = (float)gameManager.instance.playerScript.Stamina / gameManager.instance.playerScript.StaminaOrig;
        gameUIManager.instance.RespawnCount.text = gameManager.instance.respawns.ToString("F0");
        ExperienceCount.text = gameManager.instance.playerScript.currentExperience.ToString("F0");
    }


    public void UpdateUIPrompt(string message, GameObject door)
    {
        gameManager.instance.currentDoor = door;
        uiPrompt.text = message;
        uiPrompt.enabled = !string.IsNullOrEmpty(message);
    }

    public void ShowCreatorGunPrompt()
    {
        CreatorGunPrompt.SetActive(true);
    }

    public void HideCreatorGunPrompt()
    {
        CreatorGunPrompt.SetActive(false);
    }



    public void ClearUIPrompt(GameObject door)
    {

        if (gameManager.instance.currentDoor == door)
        {

            uiPrompt.enabled = false;
            gameManager.instance.currentDoor = null;

        }


    }

    public bool IsUIPromptActive()
    {

        return uiPrompt.enabled;


    }


}
