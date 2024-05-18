using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicModeUpdater : MonoBehaviour
{
    public GameObject canvas;
    public KeyCode quitKey = KeyCode.Escape;
    public KeyCode startWaveKey = KeyCode.Return;
    public KeyCode toggleCinematicKey = KeyCode.P;
    public KeyCode hideMouseKey = KeyCode.M;

    [Space]
    public UI UIManager;
    public WaveManager waveManager;

    private bool hideMouseModifier = false;

    void Start()
    {
        if (GameManager.instance.cinematicMode)
        {
            canvas.SetActive(false);
        }

        hideMouseModifier = GameManager.instance.hideMouse;
    }

    void Update()
    {
        if (Input.GetKeyDown(quitKey))
        {
            UIManager.ChangeScene("Menu");
        }

        if (Input.GetKeyDown(startWaveKey))
        {
            waveManager.LoadWave();
        }

        if (Input.GetKeyDown(toggleCinematicKey))
        {
            GameManager.instance.cinematicMode = !GameManager.instance.cinematicMode;
            canvas.SetActive(!GameManager.instance.cinematicMode);
        }

        if (Input.GetKeyDown(hideMouseKey))
        {
            hideMouseModifier = !hideMouseModifier;
        }

        if (GameManager.instance.cinematicMode)
        {
            Cursor.visible = !hideMouseModifier;
        }
        else
        {
            Cursor.visible = true;
        }
    }
}
