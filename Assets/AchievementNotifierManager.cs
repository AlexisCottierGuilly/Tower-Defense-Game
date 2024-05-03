using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AchievementNotifierManager : MonoBehaviour
{
    public GameObject achievementNotification;
    public TextMeshProUGUI title;
    public TextMeshProUGUI description;
    public TextMeshProUGUI progressText;
    public float verificationInterval = 0.1f;

    private List<AchievementProgress> shownAchievements = new List<AchievementProgress>();
    private float timeFromLastVerification = 0f;

    public void NotifyAchievement(AchievementProgress achievement)
    {
        achievementNotification.GetComponent<Animator>().SetTrigger("Notify");

        title.text = achievement.achievement.name;
        description.text = achievement.achievement.description;

        progressText.text = achievement.currentProgress + " / " + achievement.maxProgress;
        if (achievement.achievement.mesureUnit != "")
            progressText.text += " " + achievement.achievement.mesureUnit;
    }

    void Start()
    {
        InitializeShownAchievements();
        VerifyNewNotifications();

        // testing
        // NotifyAchievement(GameManager.instance.achievementProgress[0]);
    }

    void Update()
    {
        timeFromLastVerification += Time.deltaTime;

        if (timeFromLastVerification >= verificationInterval)
        {
            timeFromLastVerification = 0f;
            VerifyNewNotifications();
        }
    }

    void VerifyNewNotifications()
    {
        GameManager.instance.UpdateAchievementProgress();
        
        foreach (AchievementProgress aProgress in GameManager.instance.achievementProgress)
        {
            if (aProgress.completed && !shownAchievements.Contains(aProgress))
            {
                NotifyAchievement(aProgress);
                shownAchievements.Add(aProgress);
            }
        }
    }

    void InitializeShownAchievements()
    {
        foreach (AchievementProgress aProgress in GameManager.instance.achievementProgress)
        {
            if (aProgress.completed)
                shownAchievements.Add(aProgress);
        }
    }
}
