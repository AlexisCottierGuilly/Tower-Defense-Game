using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossHealthBar : MonoBehaviour
{
    public MonsterBehaviour boss;
    [Space]
    public Slider slider;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI healthText;
    public Image fillImage;
    [Space]
    public WaveManager waveManager;
    void Update()
    {
        if (boss == null)
        {
            waveManager.UnsetBossBar();
            return;
        }
        
        float currentHealth = (float)boss.health;
        float maxHealth = (float)boss.data.maxHealth;
        float percentage = currentHealth / maxHealth;
        
        fillImage.color = Color.Lerp(Color.red, Color.green, percentage);
        slider.value = percentage;

        healthText.text = $"{currentHealth} / {maxHealth}";
        nameText.text = boss.data.name;
    }
}
