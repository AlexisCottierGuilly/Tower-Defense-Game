using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SavedGamesDropdownManager : MonoBehaviour
{
    public TMP_Dropdown dropdown;

    private bool didLoadOptions = false;

    public void LoadOptions()
    {
        dropdown.ClearOptions();

        PlayerData playerData = GameManager.instance.player;
        List<GameSave> gameSaves = playerData.gameSaves;

        gameSaves.Sort((a, b) => a.lastOpenedTime.CompareTo(b.lastOpenedTime));

        List<string> optionNames = new List<string>();
        foreach (GameSave gameSave in gameSaves)
        {
            optionNames.Add(gameSave.saveName);
        }
        
        optionNames.Reverse();

        dropdown.AddOptions(optionNames);
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
