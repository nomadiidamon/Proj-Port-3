using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]

public class gunStats : ScriptableObject
{
    public string gunName;
    public GameObject gunModel;
    //private BoxCollider collider;
    public int shootDamage;
    public float shootRate;
    public int shootDistance;
    public int ammoCur, ammoMax;

    public Vector3 gunScale = Vector3.one;

    public GameObject bullet;

    public ParticleSystem hitEffect;
    public AudioClip[] shootSound;
    public float shootVolume;
    public AudioClip[] pickupSound;
    public float pickupVolume;
    public AudioClip[] switchSound;
    public float switchVolume;

    public bool isCreator;
    public bool isShield;
    public bool isBlast;

    public void increaseDamage(int amount)
    {
        shootDamage += amount;
    }

    

}
