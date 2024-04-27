using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class VolumeUpdater : MonoBehaviour
{
    public float volumeMultiplier = 1f;
    
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        audioSource.volume = GameManager.instance.volume * volumeMultiplier;
    }
}
