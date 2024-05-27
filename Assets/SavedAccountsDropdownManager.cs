using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SavedAccountsDropdownManager : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public SavedGamesDropdownManager savedGamesDropdownManager;
    private bool didLoadOptions = false;

    void Start()
    {
        dropdown.onValueChanged.AddListener(delegate { DidChangeOption(); });
    }

    void LoadOptions()
    {
        dropdown.ClearOptions();

        SaveData save = GameManager.instance.save;

        List<string> optionNames = new List<string>();
        string currentName = GameManager.instance.player.name;
        optionNames.Add(currentName);

        foreach (PlayerData playerData in save.players)
        {
            if (playerData.name == currentName)
                continue;
            optionNames.Add(playerData.name);
        }

        dropdown.AddOptions(optionNames);
    }

    void DidChangeOption()
    {
        string playerName = dropdown.options[dropdown.value].text;
        GameManager.instance.player = GameManager.instance.save.players.Find(x => x.name == playerName);
        GameManager.instance.save.lastPlayerName = playerName;

        /*savedGamesDropdownManager.LoadOptions();
        GameManager.instance.LoadAchievementProgress();
        GameManager.instance.save.lastPlayerName = playerName;*/

        GameManager.instance.Start();
        GameManager.instance.SwitchScene(GameState.Menu);
    }

    void Update()
    {
        if (!didLoadOptions)
        {
            LoadOptions();
            didLoadOptions = true;
        }
    }
}
