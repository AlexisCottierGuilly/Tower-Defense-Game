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
    Achievements
}


[System.Serializable]
public class DifficultyModifier
{
    public MapDifficultyTypes difficulty;
    public int initialUsedPaths = 1;
    public int addPathReccurence = 2;
    public int crystals = 5;
}


public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public int gameSeed;
    public int maxGameSeed = 9_999_999;
    public Vector2 mapSize;
    public MapDifficultyTypes mapDifficulty = MapDifficultyTypes.Moyen;
    public GameState gameState = GameState.Menu;
    public GameState previousScene;
    public float UISize = 1;
    public float volume = 0.5f;
    [Space]
    public List<DifficultyModifier> difficultyModifiers = new List<DifficultyModifier>();
    [Space]
    public int initialGold = 150;
    public int gold = 0;
    [Space]
    public SaveData save;
    public PlayerData player;
    public List<AchievementData> achievements;
    [Space]
    public GameGenerator generator = null;
    [Space]
    public AudioClip clickSound;
    public AudioSource audioSource;

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
    }

    public void SwitchScene(GameState sceneName, bool additive = false, bool unloadCurrent = false)
    {
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
}
