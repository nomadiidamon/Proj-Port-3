using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class audioManager : MonoBehaviour
{
    public static audioManager instance;

    [SerializeField] AudioSource audPlayer;


    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        
    }

    public void PlayAud(AudioClip sound, float vol)
    {
        audPlayer.PlayOneShot(sound, vol);
    }

}
