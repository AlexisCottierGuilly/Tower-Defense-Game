using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Menu,
    CreateGame,
    Game,
    Settings,
    Credits,
    PreviousScene
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
    public float towerSize = 1;
    public int gold = 100;
    
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
                Debug.Log("EventSystem deactivated");
            }
            
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
}
