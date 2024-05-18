using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedButtonManager : MonoBehaviour
{
    public Button button;
    public Image normalImage;
    public Image fastImage;
    [Space]
    public GameGenerator generator;

    void Start()
    {
        UpdateGraphics();
    }

    public void DidClick()
    {
        if (Time.timeScale == GameManager.instance.defaultSpeed)
        {
            Time.timeScale = GameManager.instance.fastSpeed;
        }
        else if (Time.timeScale != 0f)
        {
            Time.timeScale = GameManager.instance.defaultSpeed;
        }

        UpdateGraphics();
    }
    
    void UpdateGraphics()
    {
        if (Time.timeScale == GameManager.instance.defaultSpeed)
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
