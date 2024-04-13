using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealthBar : MonoBehaviour
{
    public VillageGenerator generator;
    [Space]
    public Slider slider;
    public TextMeshProUGUI text;
    public Image fillImage;
    
    void Update()
    {
        float currentHealth = (float)generator.GetRemainingLives();
        float maxHealth = (float)generator.maxHealth;
        float percentage = currentHealth / maxHealth;
        
        fillImage.color = Color.Lerp(Color.red, Color.green, percentage);
        slider.value = percentage;

        text.text = $"{currentHealth} / {maxHealth}";
    }
}
