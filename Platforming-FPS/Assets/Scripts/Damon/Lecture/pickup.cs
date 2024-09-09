using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickup : MonoBehaviour
{

    [SerializeField] gunStats gun;
    //public BoxCollider myCollider;


    private void OnTriggerEnter(Collider other)
    {
        SphereCollider coll = this.GetComponent<SphereCollider>();
        if (other.CompareTag("Player") && coll.bounds.Contains(gameManager.instance.player.transform.position))
        {
            audioManager.instance.PlayAud(gun.pickupSound[Random.Range(0, gun.pickupSound.Length)], gun.pickupVolume);
            gameManager.instance.playerScript.getGunStats(gun);
            Destroy(gameObject);
        }
    }


}
