using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum MapSizeTypes
{
    Petit,
    Moyen,
    Grand
}

public class MapSizeHandler : MonoBehaviour
{
    public MapSizeTypes mapSize = MapSizeTypes.Moyen;
    public TextMeshProUGUI text;

    public void Start()
    {
        switch (GameManager.instance.mapSize.x)
        {
            case 16:
                mapSize = MapSizeTypes.Petit;
                break;
            case 24:
                mapSize = MapSizeTypes.Moyen;
                break;
            case 32:
                mapSize = MapSizeTypes.Grand;
                break;
        }

        UpdateText();
    }
    
    public void OnClick()
    {
        switch (mapSize)
        {
            case MapSizeTypes.Petit:
                mapSize = MapSizeTypes.Moyen;
                break;
            case MapSizeTypes.Moyen:
                mapSize = MapSizeTypes.Grand;
                break;
            case MapSizeTypes.Grand:
                mapSize = MapSizeTypes.Petit;
                break;
        }

        UpdateText();

        Vector2Int size = new Vector2Int(32, 32);
        switch (mapSize)
        {
            case MapSizeTypes.Petit:
                size = new Vector2Int(24, 24);
                break;
            case MapSizeTypes.Moyen:
                size = new Vector2Int(32, 32);
                break;
            case MapSizeTypes.Grand:
                size = new Vector2Int(48, 48);
                break;
        }

        GameManager.instance.mapSize = size;
    }

    void UpdateText()
    {
        text.text = mapSize.ToString();
    }
}
