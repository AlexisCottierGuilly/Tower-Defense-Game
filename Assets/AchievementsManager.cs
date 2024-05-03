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
        GameManager.instance.UpdateAchievementProgress();
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
        // do not recalculate the progress of the achievements (use GameManager.instance.GetAchievementProgress(achievement) instead)

        GameObject newAchievement = Instantiate(achievementPlaceholder, contentParent.transform);

        newAchievement.SetActive(true);

        AchievementPreviewManager previewManager = newAchievement.GetComponent<AchievementPreviewManager>();
        previewManager.title.text = achievement.name;
        previewManager.description.text = achievement.description;

        AchievementProgress progress = GameManager.instance.GetAchievementProgress(achievement);

        previewManager.progressText.text = progress.currentProgress + " / " + progress.maxProgress;
        if (achievement.mesureUnit != "")
            previewManager.progressText.text += " " + achievement.mesureUnit;
        
        previewManager.progressSlider.value = (float)progress.currentProgress / progress.maxProgress;

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
