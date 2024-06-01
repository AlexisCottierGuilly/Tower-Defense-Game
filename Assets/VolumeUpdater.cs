using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class VolumeUpdater : MonoBehaviour
{
    public float volumeMultiplier = 1f;
    public bool isMusic = false;
    
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        audioSource.volume = GameManager.instance.volume * volumeMultiplier / 3f;
        
        if (isMusic)
            audioSource.volume *= GameManager.instance.musicVolume;
        else
            audioSource.volume *= GameManager.instance.effectsVolume;
    }
}
