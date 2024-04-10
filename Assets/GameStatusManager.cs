using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStatusManager : MonoBehaviour
{
    public GameObject victory;
    public GameObject defeat;
    public GameGenerator gameGenerator;
    public WaveManager waveManager;

    void Start()
    {
        waveManager.gameFinished.AddListener(GameIsFinished);
    }
    
    void LateUpdate()
    {
        if (gameGenerator.health <= 0 || gameGenerator.villageGenerator.mainVillage == null)
        {
            defeat.SetActive(true);
        }
        else
        {
            defeat.SetActive(false);
        }
    }

    void GameIsFinished()
    {
        victory.SetActive(true);
    }
}
