using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class audioManager : MonoBehaviour
{
    public static audioManager instance;

    [SerializeField] AudioSource audPlayer;
    [SerializeField] AudioClip audCopyObject;
    [SerializeField] AudioClip audCopyEnemy;
    [SerializeField] AudioClip audPasteObject;


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
    public void PlayCopyObjectSound()
    {
        audPlayer.PlayOneShot(audCopyObject);
    }
    public void PlayCopyEnemySound()
    {
        audPlayer.PlayOneShot(audCopyEnemy);
    }
    public void PlayPasteObjectSound()
    {
        audPlayer.PlayOneShot(audPasteObject);
    }
}
