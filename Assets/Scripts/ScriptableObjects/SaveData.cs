using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SaveData", menuName = "Save/SaveData")]
public class SaveData : ScriptableObject
{
    public List<PlayerData> players;
}

[System.Serializable]
public class PlayerData
{
    public string name = "Player";

    [Header("General")]
    public int crystals;

    [Header("Achievements")]
    public AchievementStats achievementStats;
}
