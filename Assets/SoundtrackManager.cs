using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class SoundtrackManager : MonoBehaviour
{
    public List<AudioClip> soundtracks;
    public float pauseBetweenTracks = 1.0f;

    private AudioSource audioSource;
    private int currentTrackIndex = 0;
    private List<AudioClip> playedTracks = new List<AudioClip>();

    private float currentElapsedTime = 0.0f;
    private float timeToNextTrack = 0.0f;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        currentTrackIndex = 0;

        playedTracks = new List<AudioClip>(soundtracks);
        Shuffle(playedTracks);
    }

    void Update()
    {
        currentElapsedTime += Time.unscaledDeltaTime;

        if (currentElapsedTime >= timeToNextTrack)
        {
            currentElapsedTime = 0.0f;

            audioSource.clip = playedTracks[currentTrackIndex];
            timeToNextTrack = audioSource.clip.length + pauseBetweenTracks;
            audioSource.Play();

            currentTrackIndex++;
            if (currentTrackIndex >= playedTracks.Count)
            {
                currentTrackIndex = 0;
                Shuffle(playedTracks);
            }
        }
    }

    private void Shuffle(List<AudioClip> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            AudioClip temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}
