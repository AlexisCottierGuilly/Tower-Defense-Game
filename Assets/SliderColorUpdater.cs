using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderColorUpdater : MonoBehaviour
{
    public Slider slider;
    public Image fillImage;
    public Color minimumColor = Color.red;
    public Color maximumColor = Color.green;

    public void UpdateColor()
    {
        fillImage.color = Color.Lerp(minimumColor, maximumColor, slider.value);
    }
}
