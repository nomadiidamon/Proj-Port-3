using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : MonoBehaviour
{
    MeteorShower meteorShower;
    [SerializeField] ParticleSystem meteorHitEffect;
    [SerializeField] AudioSource audSource;
    [SerializeField] AudioClip[] meteorImpactAud;
    [SerializeField] AudioClip[] meteorDamageAud;

    private void Start()
    {
        meteorShower = FindObjectOfType<MeteorShower>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }

        IDamage damage = other.GetComponent<IDamage>();
        if (damage != null)
        {
            damage.takeDamage(meteorShower.GetMeteorSpeed());
            audioManager.instance.PlayAud(meteorDamageAud[Random.Range(0, meteorDamageAud.Length)], meteorShower.meteorAudio);
        }
        else
        {
            audioManager.instance.PlayAud(meteorImpactAud[Random.Range(0, meteorImpactAud.Length)], meteorShower.meteorAudio);
        }
        Instantiate(meteorHitEffect, gameObject.transform.position, Quaternion.identity);
        Destroy(gameObject);
        meteorShower.currentNumberOfMeteors--;
    }
}
