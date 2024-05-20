using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Button))]
public class GameQuitManager : MonoBehaviour
{
    private void Awake()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(QuitGame);
    }

    private void OnApplicationQuit()
    {
        TryToSave();
    }

    void QuitGame()
    {
        TryToSave();
    }

    void TryToSave()
    {
        if (GameManager.instance.generator.waveManager.waveFinished)
            GameManager.instance.generator.savingManager.SaveGame();
    }
}
