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

    private Dictionary<MapSizeTypes, Vector2Int> mapSizes = new Dictionary<MapSizeTypes, Vector2Int>
    {
        { MapSizeTypes.Petit, new Vector2Int(24, 24) },
        { MapSizeTypes.Moyen, new Vector2Int(32, 32) },
        { MapSizeTypes.Grand, new Vector2Int(48, 48) }
    };
    private Dictionary<Vector2Int, MapSizeTypes> mapSizesReverse;

    public void Start()
    {
        mapSizesReverse = new Dictionary<Vector2Int, MapSizeTypes>();
        foreach (KeyValuePair<MapSizeTypes, Vector2Int> entry in mapSizes)
        {
            mapSizesReverse.Add(entry.Value, entry.Key);
        }
        
        /*switch (GameManager.instance.mapSize.x)
        {
            case 24:
                mapSize = MapSizeTypes.Petit;
                break;
            case 32:
                mapSize = MapSizeTypes.Moyen;
                break;
            case 48:
                mapSize = MapSizeTypes.Grand;
                break;
        }*/

        mapSize = MapSizeTypes.Moyen;

        Vector2Int size = new Vector2Int((int)GameManager.instance.mapSize.x, (int)GameManager.instance.mapSize.y);
        mapSize = mapSizesReverse[size];

        UpdateText();
    }
    
    public void OnClick()
    {
        /*Vector2Int size = new Vector2Int(32, 32);
        switch (mapSize)
        {
            case MapSizeTypes.Petit:
                mapSize = MapSizeTypes.Moyen;
                size = new Vector2Int(32, 32);
                break;
            case MapSizeTypes.Moyen:
                mapSize = MapSizeTypes.Grand;
                size = new Vector2Int(48, 48);
                break;
            case MapSizeTypes.Grand:
                mapSize = MapSizeTypes.Petit;
                size = new Vector2Int(24, 24);
                break;
        }*/

        // get the next map size and size (using the mapSizes dictionary)

        MapSizeTypes nextMapSize = mapSize;
        Vector2Int size = mapSizes[mapSize];
        foreach (KeyValuePair<MapSizeTypes, Vector2Int> entry in mapSizes)
        {
            if (entry.Key == mapSize)
            {
                int index = (int)mapSize;
                index = (index + 1) % mapSizes.Count;
                nextMapSize = (MapSizeTypes)index;
                size = mapSizes[nextMapSize];
                break;
            }
        }

        mapSize = nextMapSize;

        UpdateText();
        GameManager.instance.mapSize = size;

        Debug.Log("Map size : " + mapSize.ToString() + " " + size.ToString());

        /*switch (mapSize)
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
        }*/

    }

    void UpdateText()
    {
        text.text = mapSize.ToString();
    }
}
