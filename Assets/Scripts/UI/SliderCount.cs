using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum SliderType
{
    FOV,
    Volume
}

public class SliderCount : MonoBehaviour
{
    public Slider slider;
    public SliderType sliderType;
    private TextMeshProUGUI textMeshPro;
    
    // Start is called before the first frame update
    void Start()
    {
        textMeshPro = gameObject.GetComponent<TextMeshProUGUI>();
        if (sliderType is SliderType.FOV)
            slider.value = GameManager.instance.fov;
        else
            slider.value = GameManager.instance.volume * 100f;
    }

    // Update is called once per frame
    public void UpdateValue()
    {
        if (Input.GetKey(KeyCode.LeftShift))
            slider.value = Mathf.Round(slider.value / 10f) * 10f;
        
        textMeshPro.text = $"{slider.value.ToString()}%";

        if (sliderType is SliderType.FOV)
            GameManager.instance.fov = (int)slider.value;
        else
            GameManager.instance.volume = slider.value / 100f;
    }
}
