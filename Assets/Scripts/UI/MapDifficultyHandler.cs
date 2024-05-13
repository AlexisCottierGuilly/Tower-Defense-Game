using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum MapDifficultyTypes
{
    Facile,
    Moyen,
    Difficile,
    Hardcore
}

public class MapDifficultyHandler : MonoBehaviour
{
    public MapDifficultyTypes mapDifficulty = MapDifficultyTypes.Moyen;
    public TextMeshProUGUI text;

    public void Start()
    {
        mapDifficulty = GameManager.instance.mapDifficulty;
        UpdateText();
    }
    
    public void OnClick()
    {
        switch (mapDifficulty)
        {
            case MapDifficultyTypes.Facile:
                mapDifficulty = MapDifficultyTypes.Moyen;
                break;
            case MapDifficultyTypes.Moyen:
                mapDifficulty = MapDifficultyTypes.Difficile;
                break;
            case MapDifficultyTypes.Difficile:
                mapDifficulty = MapDifficultyTypes.Hardcore;
                break;
            case MapDifficultyTypes.Hardcore:
                mapDifficulty = MapDifficultyTypes.Facile;
                break;
        }

        UpdateText();
        GameManager.instance.mapDifficulty = mapDifficulty;
    }

    void UpdateText()
    {
        text.text = mapDifficulty.ToString();
    }
}
