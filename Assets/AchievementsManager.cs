using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementsManager : MonoBehaviour
{
    public GameObject achievementPlaceholder;
    public GameObject contentParent;

    private List<GameObject> achievements = new List<GameObject>();

    void Start()
    {
        LoadAchievements();
        ReorderAchievements();
    }

    void LoadAchievements()
    {
        foreach (AchievementData achievement in GameManager.instance.achievements)
        {
            LoadAchievement(achievement);
        }
    }

    void LoadAchievement(AchievementData achievement)
    {
        GameObject newAchievement = Instantiate(achievementPlaceholder, contentParent.transform);

        newAchievement.SetActive(true);

        AchievementPreviewManager previewManager = newAchievement.GetComponent<AchievementPreviewManager>();
        previewManager.title.text = achievement.name;
        previewManager.description.text = achievement.description;

        int progress = 0;
        int total = 0;

        foreach (var field in typeof(AchievementStats).GetFields())
        {
            int maximum = 0;
            int current = 0;

            if (field.FieldType == typeof(int))
            {
                maximum = (int)field.GetValue(achievement.requirements);
                current = (int)field.GetValue(GameManager.instance.player.achievementStats);
            }
            else if (field.FieldType == typeof(float))
            {
                maximum = Mathf.RoundToInt((float)field.GetValue(achievement.requirements));
                current = Mathf.RoundToInt((float)field.GetValue(GameManager.instance.player.achievementStats));
            }

            current = Mathf.Min(current, maximum);

            progress += current;
            total += maximum;
        }

        previewManager.progressText.text = progress + " / " + total;
        if (achievement.mesureUnit != "")
            previewManager.progressText.text += " " + achievement.mesureUnit;

        previewManager.progressSlider.value = (float)progress / total;

        achievements.Add(newAchievement);
    }

    void ReorderAchievements()
    {
        achievements.Sort((a, b) =>
        {
            AchievementPreviewManager aManager = a.GetComponent<AchievementPreviewManager>();
            AchievementPreviewManager bManager = b.GetComponent<AchievementPreviewManager>();

            float aProgress = aManager.progressSlider.value;
            float bProgress = bManager.progressSlider.value;

            return aProgress.CompareTo(bProgress);
        });

        achievements.Reverse();

        for (int i = 0; i < achievements.Count; i++)
        {
            achievements[i].transform.SetSiblingIndex(i);
        }
    }
}
