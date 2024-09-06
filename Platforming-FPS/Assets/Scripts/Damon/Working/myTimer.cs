using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class myTimer : MonoBehaviour
{
    public float startingTime;
    public float targetTime;
    public float currentTime;

    public bool countUp;
    public bool timeElapsed;
    private bool isPaused;

    void Start()
    {
        if (countUp)
        {
            currentTime = startingTime;
        }
        else if (!countUp)
        {
            currentTime = targetTime;
        }
        isPaused = false;
    }

    void Update()
    {
        if (countUp)
        {
            if (!isPaused)
            {
                currentTime += Time.deltaTime;
            
                if (currentTime >= targetTime)
                {
                    timeElapsed = true;
                    //ResetTimer();
                }
            }
        }
        else if (!countUp)
        {
            if (!isPaused)
            {
                currentTime -= Time.deltaTime;

                if (currentTime <= startingTime)
                {
                    timeElapsed = true;
                    //ResetTimer();
                }
            }
        }
    }

    public void ResetTimer()
    {
        if (countUp)
        {
            currentTime = startingTime;
        }
        else if (!countUp)
        {
            currentTime = targetTime;
        }
        StartCoroutine(waitToTrigger());
        timeElapsed = false;

    }

    public void PauseTimer()
    {
        isPaused = true;
    }

    public void UnpauseTimer()
    {
        isPaused = false;
    }

    IEnumerator waitToTrigger()
    {
        yield return new WaitForSeconds(0.25f);
    }

}
