using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsPauseGameManager : MonoBehaviour
{
    void Start()
    {
        if (GameManager.instance.generator != null)
        {
            GameManager.instance.generator.PauseGame();
        }
    }

    public void WillQuit()
    {
        if (GameManager.instance.generator != null)
        {
            GameManager.instance.generator.ResumeGame();
        }
    }
}
