using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStatusManager : MonoBehaviour
{
    public GameObject victory;
    public GameObject defeat;
    public GameGenerator gameGenerator;
    public WaveManager waveManager;

    private bool registeredDefeat = false;

    void Start()
    {
        waveManager.gameFinished.AddListener(GameIsFinished);
    }
    
    void LateUpdate()
    {
        DifficultyModifier difficulty = GameManager.instance.GetDifficultyModifier();

        if ((gameGenerator.didFinishLoading &&
            (gameGenerator.health <= 0 ||
            gameGenerator.villageGenerator.mainVillage == null ||
            (gameGenerator.lostLives > 0 && !difficulty.canLoseLives))) ||
            gameGenerator.forceDefeat)
        {
            defeat.SetActive(true);
            gameGenerator.PauseGame();

            if (!registeredDefeat)
            {
                gameGenerator.savingManager.SaveGame();
                registeredDefeat = true;
            }
        }
        else if (waveManager.infiniteMode)
        {
            victory.SetActive(false);
            defeat.SetActive(false);
        }
    }

    void GameIsFinished()
    {
        Debug.Log("Game is finished");
        victory.SetActive(true);
        gameGenerator.PauseGame();
    }
}
