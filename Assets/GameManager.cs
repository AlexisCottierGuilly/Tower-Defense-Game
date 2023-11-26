using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public int gameSeed;
    public Vector2 mapSize;
    public GameState gameState = GameState.Menu;
    
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
       gameSeed = 0;
       mapSize = new Vector2(32, 32);
    }

    public void SwitchScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        switch (sceneName)
        {
            case "Menu":
                gameState = GameState.Menu;
                break;
            case "CreateGame":
                gameState = GameState.CreateGame;
                break;
            case "Game":
                gameState = GameState.Game;
                break;
            case "Settings":
                gameState = GameState.Settings;
                break;
            case "Credits":
                gameState = GameState.Credits;
                break;
        }
    }
}


public enum GameState
{
    Menu,
    CreateGame,
    Game,
    Settings,
    Credits
}
