using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum SliderType
{
    UI,
    Tower
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
        if (sliderType is SliderType.UI)
            slider.value = GameManager.instance.UISize * 100f;
        else
            slider.value = GameManager.instance.TowerSize * 100f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            slider.value = Mathf.Round(slider.value / 10f) * 10f;
            Debug.Log(slider.value.ToString());
        }
        
        textMeshPro.text = $"{slider.value.ToString()}%";

        if (sliderType is SliderType.UI)
            GameManager.instance.UISize = slider.value / 100f;
        else
            GameManager.instance.TowerSize = slider.value / 100f;
    }
}
