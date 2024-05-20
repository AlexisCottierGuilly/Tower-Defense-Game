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
    public int rewardCount = 0;

    [Header("Achievements")]
    public AchievementStats achievementStats;
    
    [Header("Unlocked Towers")]
    public List<TowerType> unlockedTowers = new List<TowerType>() { TowerType.Normal };

    [Header("Other")]
    public TowerType villageTower = TowerType.None;
    public List<TowerType> villageTowers = new List<TowerType>();

    [Header("Game Saves")]
    public List<GameSave> gameSaves = new List<GameSave>();
}


[System.Serializable]
public class GameSave
{
    public string saveName;

    [Header("General")]
    public int seed;
    public int gold;
    public int wave;
    public float gameTime;
    public float lastOpenedTime;
    public MapDifficultyTypes mapDifficulty;
    public Vector2 mapSize;

    [Header("Randoms")]
    public System.Random randomWithSeed;
    public System.Random waveRandomWithSeed;

    [Header("Towers & Villages")]
    public List<TowerPlacement> towerPlacements = new List<TowerPlacement>();
    public List<VillagePlacement> villagePlacements = new List<VillagePlacement>();
    public VillagePlacement mainVillagePlacement;
}


[System.Serializable]
public class TowerPlacement
{
    public TowerType towerType;
    public Vector2 position;
    public TargetType targetType;
}


[System.Serializable]
public class VillagePlacement
{
    public Vector2 position;
    public int health;
}
