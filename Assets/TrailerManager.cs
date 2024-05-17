using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.Events;


public enum TrailerType
{
    Logo,
    Title,
    Video
}


public class TrailerManager : MonoBehaviour
{
    public List<TrailerPart> trailerParts = new List<TrailerPart>();

    [Header("Logo")]
    public GameObject logoScreen;

    [Header("Title")]
    public GameObject titleScreen;
    public TextMeshProUGUI titleText;

    [Header("Video")]
    public GameObject videoScreen;
    public VideoPlayer videoPlayer;

    [HideInInspector] public UnityEvent OnPartLoaded;

    private int currentPartIndex = -1;
    private TrailerPart currentPart;
    private float timeFromPartStart = 0f;

    private void Start()
    {
        logoScreen.SetActive(false);
        titleScreen.SetActive(false);
        videoScreen.SetActive(false);
        
        if (trailerParts.Count > 0)
        {
            LoadNextPart();
        }
    }

    private void LoadNextPart()
    {
        timeFromPartStart = 0f;
        
        if (currentPart != null)
        {
            switch (currentPart.type)
            {
                case TrailerType.Logo:
                    logoScreen.SetActive(false);
                    break;
                case TrailerType.Title:
                    titleScreen.SetActive(false);
                    break;
                case TrailerType.Video:
                    videoScreen.SetActive(false);
                    break;
            }
        }

        currentPartIndex++;

        if (currentPartIndex >= trailerParts.Count)
            return;

        currentPart = trailerParts[currentPartIndex];

        LoadPart(currentPart);
    }

    private void LoadPart(TrailerPart part)
    {
        switch (part.type)
        {
            case TrailerType.Logo:
                logoScreen.SetActive(true);
                break;
            case TrailerType.Title:
                titleScreen.SetActive(true);
                titleText.text = part.title;
                break;
            case TrailerType.Video:
                videoScreen.SetActive(true);
                videoPlayer.clip = part.video;
                videoPlayer.Play();
                break;
        }

        OnPartLoaded.Invoke();
    }

    void Update()
    {
        if (currentPart == null)
            return;

        timeFromPartStart += Time.deltaTime;
        if (timeFromPartStart >= currentPart.duration)
        {
            LoadNextPart();
        }
    
    }
}


[System.Serializable]
public class TrailerPart
{
    public TrailerType type;
    public float duration;

    [Header("Title")]
    public string title;

    [Header("Video")]
    public VideoClip video;
}
