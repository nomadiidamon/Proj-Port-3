using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class LavaGun : MonoBehaviour
{
    public gunStats gunStats;
    public Transform laserPoint;
    public float chargeDuration = 2f;

    private bool isCharging;
    private GameObject currentLaser;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse1))
        {
            StartCharging();
        }

        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            StopCharging();
        }
        
    }

    private void StartCharging()
    {
        if (!isCharging)
        {
            isCharging = true;
            StartCoroutine(ChargeLaser());
        }
    }

    private void StopCharging()
    {
        if (isCharging)
        {
            isCharging = false;
            StopCoroutine(ChargeLaser());

            if (currentLaser != null)
            {
                Destroy(currentLaser);
            }
        }
    }

    private IEnumerator ChargeLaser()
    {
        currentLaser = Instantiate(gunStats.bullet, laserPoint.position, laserPoint.rotation);
        currentLaser.SetActive(false);

        float chargeTime = 0f;

        while (chargeTime < chargeDuration)
        {
            chargeTime += Time.deltaTime;
            yield return null;
        }

        FireLaser();
    }

    private void FireLaser()
    {
        if (currentLaser != null)
        {
            currentLaser.SetActive(true);
            StartCoroutine(DeactivateLaser(currentLaser, 2f));
        }
    }

    private IEnumerator DeactivateLaser(GameObject laser, float duration)
    {
        yield return new WaitForSeconds(duration);
        laser.SetActive(false);
    }
}
