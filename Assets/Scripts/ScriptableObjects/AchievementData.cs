using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "AchievementData", menuName = "Save/AchievementData")]
public class AchievementData : ScriptableObject
{
    public string name;
    public string description;
    public string mesureUnit;
    public AchievementStats requirements;
}


[System.Serializable]
public class AchievementStats
{
    public int shotsOnDecorations;
    public int timesOpeningCredits;
    public float timeWaitedInTutorial;
}


[System.Serializable]
public class AchievementProgress
{
    public AchievementData achievement;
    public int currentProgress;
    public int maxProgress;
    public bool completed;
}
