using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedButtonManager : MonoBehaviour
{
    public Button button;
    public Image normalImage;
    public Image fastImage;
    public float speedMultiplier = 1.5f;
    [Space]
    public GameGenerator generator;

    void Start()
    {
        UpdateGraphics();
    }

    public void DidClick()
    {
        if (Time.timeScale == 1f)
        {
            Time.timeScale = speedMultiplier;
            generator.defaultSpeed = speedMultiplier;
        }
        else if (Time.timeScale != 0f)
        {
            Time.timeScale = 1f;
            generator.defaultSpeed = 1f;
        }

        UpdateGraphics();
    }
    
    void UpdateGraphics()
    {
        if (Time.timeScale == 1f)
        {
            normalImage.color = new Color(normalImage.color.r, normalImage.color.g, normalImage.color.b, 200);
            fastImage.color = new Color(fastImage.color.r, fastImage.color.g, fastImage.color.b, 0);
        }
        else
        {
            normalImage.color = new Color(normalImage.color.r, normalImage.color.g, normalImage.color.b, 0);
            fastImage.color = new Color(fastImage.color.r, fastImage.color.g, fastImage.color.b, 200);
        }
    }
}
