using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeForMenu : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.instance.SwitchScene(GameState.Menu);
        }
    }
}
