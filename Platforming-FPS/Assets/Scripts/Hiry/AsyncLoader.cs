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

            //if (gameManager.instance != null && gameManager.instance.isPaused) {
               // gameManager.instance.stateUnpause();
            //}

        }




    }


    public void openLevelMenu()
    {
        menuMain.SetActive(false);
        menuLevel.SetActive(true);


    }

    public void back()
    {
        menuLevel.SetActive(false);
        menuMain.SetActive(true);
    }

    

   


}
