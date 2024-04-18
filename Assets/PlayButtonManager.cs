using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayButtonManager : MonoBehaviour
{
    public Button button;
    public Image pauseImage;
    public Image playImage;
    [Space]
    public GameGenerator generator;
    private bool isPaused = false;

    void Start()
    {
        isPaused = generator.paused;
        UpdateGraphics();
    }
    
    public void DidClick()
    {
        if (isPaused)
        {
            generator.ResumeGame();
            isPaused = false;
        }
        else
        {
            generator.PauseGame();
            isPaused = true;
        }

        UpdateGraphics();
    }

    void UpdateGraphics()
    {
        if (!isPaused)
        {
            pauseImage.color = new Color(pauseImage.color.r, pauseImage.color.g, pauseImage.color.b, 200);
            playImage.color = new Color(playImage.color.r, playImage.color.g, playImage.color.b, 0);
        }
        else
        {
            pauseImage.color = new Color(pauseImage.color.r, pauseImage.color.g, pauseImage.color.b, 0);
            playImage.color = new Color(playImage.color.r, playImage.color.g, playImage.color.b, 200);
        }
    }
}
