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
    public Vector2 mapSize;
    public MapDifficultyTypes mapDifficulty = MapDifficultyTypes.Moyen;
    public GameState gameState = GameState.Menu;
    public GameState previousScene;
    public float UISize = 1;
    public float towerSize = 1;
    
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

    public void SwitchScene(GameState sceneName)
    {
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

        SceneManager.LoadScene(gameState.ToString());
    }
}
