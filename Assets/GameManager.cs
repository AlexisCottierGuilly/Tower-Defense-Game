using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public enum GameState
{
    Menu,
    CreateGame,
    Game,
    Settings,
    Credits,
    PreviousScene,
    Tutoriel,
    Achievements,
    Shop,
    Trailer
}


[System.Serializable]
public class DifficultyModifier
{
    public MapDifficultyTypes difficulty;
    public int initialUsedPaths = 1;
    public int addPathReccurence = 2;
    public int crystals = 5;
    public int waves = 10;
    public bool canLoseLives = true;
}


[System.Serializable]
public class TowerPrefab
{
    public TowerType tower;
    public TowerData data;
    public Texture2D icon;
}


public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    
    [Header("General")]
    public int gameSeed;
    public int maxGameSeed = 9_999_999;
    public Vector2 mapSize;
    public MapDifficultyTypes mapDifficulty = MapDifficultyTypes.Moyen;
    public GameState gameState = GameState.Menu;
    public GameState previousScene;

    [Header("Settings")]
    public bool hideMouse = false;
    public int fov = 70;
    public float volume = 0.5f;
    public bool cinematicMode = false;
    public float defaultSpeed = 1f;
    public float fastSpeed = 3f;
    public string gameName = "Sans nom";
    public bool loadSavedGame = false;

    [Header("Difficulties")]
    public List<DifficultyModifier> difficultyModifiers = new List<DifficultyModifier>();

    [Header("Player Stats")]
    public int initialGold = 150;
    public int gold = 0;

    [Header("Data")]
    public SaveData save;
    public PlayerData player;
    public List<AchievementData> achievements;
    [HideInInspector] public List<AchievementProgress> achievementProgress;

    [Header("Audio")]
    public AudioClip clickSound;
    public AudioSource audioSource;

    [Header("Game Prefabs")]
    public List<TowerPrefab> towerPrefabs = new List<TowerPrefab>();

    [HideInInspector] public GameGenerator generator = null;

    /* 
    Best seeds
        Without Spikes :
        - 5564848

        With Spikes :
        - 9357669
        - 2944643

        Paths Testing :
        - 6564189
    */
    
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }

    void Start()
    {
        previousScene = gameState;
        player = save.players[0];

        UpdateGameName();
        LoadAchievementProgress();
    }

    public void SwitchScene(GameState sceneName, bool additive = false, bool unloadCurrent = false)
    {
        Cursor.visible = true;
        
        if (unloadCurrent)
        {
            SceneManager.UnloadSceneAsync(gameState.ToString());
            EnableEventSystem(previousScene.ToString());
        }
        
        if (sceneName != GameState.PreviousScene)
        {
            previousScene = gameState;
            gameState = sceneName;
        }
        else
        {
            GameState temp = gameState;
            gameState = previousScene;
            previousScene = temp;
        }

        if (!unloadCurrent)
        {
            if (additive)
            {
                GameObject.Find("EventSystem").SetActive(false);
            }
            
            if (sceneName == GameState.Credits)
                player.achievementStats.timesOpeningCredits += 1;

            SceneManager.LoadScene(gameState.ToString(), additive ? LoadSceneMode.Additive : LoadSceneMode.Single);
        }
    }

    void EnableEventSystem(string sceneName)
    {
        Scene s = SceneManager.GetSceneByName(sceneName);
        GameObject[] rootObjs = s.GetRootGameObjects();
        foreach (GameObject g in rootObjs)
        {
            if (g.name == "EventSystem")
            {
                g.SetActive(true);
                break;
            }
        }
    }

    public void UpdateGameName()
    {
        string currentName = gameName == "" ? "Sans nom" : gameName;
        string unmodifiedName = currentName;

        // detect a number at the end of the name (if present -> set the i to that number)

        int i = 0;
        string[] split = currentName.Split(' ');
        if (split.Length > 1)
        {
            string last = split[split.Length - 1];
            if (int.TryParse(last, out i))
            {
                unmodifiedName = currentName.Substring(0, currentName.Length - last.Length - 1);
            }
        }

        while (true)
        {
            bool found = false;

            foreach (GameSave save in player.gameSaves)
            {
                if (save.saveName == currentName)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                gameName = currentName;
                break;
            }

            i++;
            currentName = $"{unmodifiedName} " + i;
        }

        gameName = currentName;
    }

    public DifficultyModifier GetDifficultyModifier()
    {
        foreach (DifficultyModifier modifier in difficultyModifiers)
        {
            if (modifier.difficulty == mapDifficulty)
                return modifier;
        }

        throw new System.Exception("No difficulty modifier found for " + mapDifficulty.ToString());

        return null;
    }

    public void LoadAchievementProgress()
    {
        foreach (AchievementData achievement in achievements)
        {
            AchievementProgress progress = new AchievementProgress();
            progress.achievement = achievement;
            achievementProgress.Add(progress);
        }

        UpdateAchievementProgress();
    }

    public void UpdateAchievementProgress()
    {
        foreach (AchievementProgress aProgress in achievementProgress)
        {
            int progress = 0;
            int total = 0;

            foreach (var field in typeof(AchievementStats).GetFields())
            {
                int maximum = 0;
                int current = 0;

                if (field.FieldType == typeof(int))
                {
                    maximum = (int)field.GetValue(aProgress.achievement.requirements);
                    current = (int)field.GetValue(GameManager.instance.player.achievementStats);
                }
                else if (field.FieldType == typeof(float))
                {
                    maximum = Mathf.RoundToInt((float)field.GetValue(aProgress.achievement.requirements));
                    current = Mathf.RoundToInt((float)field.GetValue(GameManager.instance.player.achievementStats));
                }
                else if (field.FieldType == typeof(bool))
                {
                    maximum = (bool)field.GetValue(aProgress.achievement.requirements) ? 1 : 0;
                    current = (bool)field.GetValue(GameManager.instance.player.achievementStats) ? 1 : 0;
                }

                current = Mathf.Min(current, maximum);

                progress += current;
                total += maximum;
            }

            aProgress.currentProgress = progress;
            aProgress.maxProgress = total;
            aProgress.completed = aProgress.currentProgress >= aProgress.maxProgress;
        }
    }

    public AchievementProgress GetAchievementProgress(AchievementData achievement)
    {
        foreach (AchievementProgress progress in achievementProgress)
        {
            if (progress.achievement == achievement)
                return progress;
        }

        return null;
    }

    public GameObject GetTowerPrefab(TowerType type)
    {
        foreach (TowerPrefab towerPrefab in GameManager.instance.towerPrefabs)
        {
            if (towerPrefab.tower == type)
                return towerPrefab.data.prefab;
        }
        return null;
    }

    public TowerPrefab TowerPrefabFromType(TowerType type)
    {
        foreach (TowerPrefab prefab in towerPrefabs)
        {
            if (prefab.tower == type)
                return prefab;
        }

        return null;
    }
}
