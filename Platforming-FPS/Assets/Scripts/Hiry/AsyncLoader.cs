using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;

public class AsyncLoader : MonoBehaviour
{

    [Header("Menu Screen")]
    [SerializeField] GameObject loadingScreen;
    [SerializeField] GameObject menuMain;
    [SerializeField] GameObject creditMenu;

    public GameObject menuLevel;


    [Header("Slider")]
    [SerializeField] Slider loadingSlider;

    public void LoadLevelBtn(string levelToLoad)
    {
        menuMain.SetActive(false);
        loadingScreen.SetActive(true);

        StartCoroutine(LoadLevelAsync(levelToLoad));
    }
    

    IEnumerator LoadLevelAsync(string levelToLoad)
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(levelToLoad);
        while (!loadOperation.isDone)
        {
            float progressValue = Mathf.Clamp01(loadOperation.progress / 0.9f);
            loadingSlider.value = progressValue;
            yield return null;

            if (gameUIManager.instance != null && gameUIManager.instance.isPaused)
            {
                gameUIManager.instance.stateUnpause();
            }

        }




    }


    public void openLevelMenu()
    {
        menuMain.SetActive(false);
        menuLevel.SetActive(true);


    }

    public void back()
    {
        if (menuLevel.activeSelf)
        {
            menuLevel.SetActive(false);
        }
        else if (creditMenu.activeSelf)
        {
            creditMenu.SetActive(false);
        }
        menuMain.SetActive(true);
    }




    public void OnCreditPress()
    {
        menuMain.SetActive(false);
        creditMenu.SetActive(true);
    }


}
