using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Decoration
{
    public GameObject prefab;
    public int count = 1;
    public float scale = 1f;
    public float offsetMultiplier = 0.5f;
}


public class DecorationGenerator : MonoBehaviour
{
    public GameGenerator gameGenerator;

    [Header("Decorations (count for 32x32)")]
    public List<Decoration> decorations = new List<Decoration>();

    [Header("Parents")]
    public GameObject decorationsParent;

    private Dictionary<GameObject, int> decorationCounts = new Dictionary<GameObject, int>();
    private float countMultiplier = 1;

    /*
    if (gameGenerator.randomWithSeed.Next(0, 10) == 0)
        SpawnDecoration(decorations[0].prefab, new_tile);
    */

    void Start()
    {
        countMultiplier = (GameManager.instance.mapSize.x * GameManager.instance.mapSize.y / (32f * 32f));
        Debug.Log($"Count multiplier: {countMultiplier}");
    }

    public void AddDecorations()
    {
        // loop in gamegenerator.tiles (list 2d) and skip the tiles with buildings
        // for each tile, check if it should spawn a decoration

        List<Vector2> villagePositions = new List<Vector2>();
        villagePositions.AddRange(gameGenerator.GetMainVillageTiles());
        villagePositions.AddRange(gameGenerator.GetVillageTiles());

        foreach (List<GameObject> row in gameGenerator.tiles)
        {
            foreach (GameObject tile in row)
            {
                if (villagePositions.Contains(tile.GetComponent<TileBehaviour>().position))
                    continue;

                if (tile.GetComponent<TileBehaviour>().type != TileType.Path && ShouldSpawnDecoration())
                {
                    Decoration decoration = GetWeightedDecoration();
                    if (decoration != null)
                    {
                        SpawnDecoration(decoration, tile);
                    }
                }
            }
        }

        string decorationCountString = "";
        // print the number of decorations spawned and the name of each decoration
        foreach (KeyValuePair<GameObject, int> decorationCount in decorationCounts)
        {
            decorationCountString += $"{decorationCount.Key.name}: {decorationCount.Value}\n";
        }

        Debug.Log(decorationCountString);
    }

    public bool ShouldSpawnDecoration()
    {
        // check the total number of decoration to spawn and the total number of tiles.
        // this ratio gives the chance of a decoration to spawn
        // return true if the random number is lower than the ratio

        int totalDecorationCount = 0;
        foreach (Decoration decoration in decorations)
        {
            totalDecorationCount += (int)((float)decoration.count * countMultiplier);
        }

        int totalTileCount = (int)(gameGenerator.terrainGenerator.size.x * gameGenerator.terrainGenerator.size.y);

        float ratio = (float)totalDecorationCount / (float)totalTileCount;

        return gameGenerator.randomWithSeed.NextDouble() < ratio;
    }
    
    public Decoration GetWeightedDecoration()
    {
        // use the weights from decorations (attribute count) to choose the right prefab
        // the count for each decoration should not be higher than the countMultiplier * count

        int totalWeight = 0;
        foreach (Decoration decoration in decorations)
        {
            totalWeight += decoration.count;
        }

        int random = gameGenerator.randomWithSeed.Next(0, totalWeight);

        int currentWeight = 0;
        foreach (Decoration decoration in decorations)
        {
            currentWeight += decoration.count;

            int alreadySpawned = 0;
            if (decorationCounts.ContainsKey(decoration.prefab))
                alreadySpawned = decorationCounts[decoration.prefab];

            if (random < currentWeight && alreadySpawned < countMultiplier * decoration.count)
            {
                if (!decorationCounts.ContainsKey(decoration.prefab))
                    decorationCounts[decoration.prefab] = 0;

                decorationCounts[decoration.prefab] += 1;
                
                return decoration;
            }
        }

        return null;
    }
    
    public void SpawnDecoration(Decoration decorationInfo, GameObject tile)
    {
        GameObject decoration = Instantiate(decorationInfo.prefab, tile.transform.position, Quaternion.identity);
        
        decoration.transform.eulerAngles = new Vector3(
            decoration.transform.eulerAngles.x,
            decoration.transform.eulerAngles.y,
            decoration.transform.eulerAngles.z
        );

        decoration.transform.localScale = new Vector3(
            decoration.transform.localScale.x * decorationInfo.scale,
            decoration.transform.localScale.y * decorationInfo.scale,
            decoration.transform.localScale.z * decorationInfo.scale
        );

        decoration.transform.position = new Vector3(
            decoration.transform.position.x,
            decoration.transform.position.y + decoration.transform.localScale.y * decorationInfo.offsetMultiplier,
            decoration.transform.position.z
        );

        decoration.transform.parent = decorationsParent.transform;

        gameGenerator.decorations.Add(decoration);
    }
}
