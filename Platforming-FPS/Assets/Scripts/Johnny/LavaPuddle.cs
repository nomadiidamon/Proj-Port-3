using System.Collections;
using UnityEngine;

public class LavaPuddle : MonoBehaviour
{
    [SerializeField] int damageAmt = 2;
    [SerializeField] float poolDuration = 8f;
    [SerializeField] float damageDelay = 0.4f;

    private Coroutine damageCoroutine;

    private void Start()
    {
        tag = "LavaPuddle";
        Destroy(gameObject, poolDuration);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerFeet") && damageCoroutine == null)
        {
            damageCoroutine = StartCoroutine(ApplyDamage(other));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerFeet") && damageCoroutine != null)
        {
            StopCoroutine(damageCoroutine);
            damageCoroutine = null;
        }
    }

    private IEnumerator ApplyDamage(Collider playerFeet)
    {
        IDamage playerDmgComp = playerFeet.GetComponentInParent<IDamage>();

        while (playerDmgComp != null)
        {
            Debug.Log("Player is taking damage from lava every " + damageDelay + " seconds.");
            playerDmgComp.takeDamage(damageAmt);
            yield return new WaitForSecondsRealtime(damageDelay); 
        }

        damageCoroutine = null; 
    }
}