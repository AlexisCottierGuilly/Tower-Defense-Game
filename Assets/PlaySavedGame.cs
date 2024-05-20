using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PlaySavedGame : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public UI UIManager;

    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(PlayGame);
    }

    void PlayGame()
    {
        if (dropdown.options.Count == 0)
            return;

        string saveName = dropdown.options[dropdown.value].text;

        if (saveName != "")
        {
            GameManager.instance.gameName = saveName;
            GameManager.instance.loadSavedGame = true;
            UIManager.ChangeScene(GameState.Game.ToString());
        }
    }
}
