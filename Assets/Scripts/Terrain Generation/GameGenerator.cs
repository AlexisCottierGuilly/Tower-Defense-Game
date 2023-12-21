using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class MonsterPrefab
{
    public Monster monster;
    public GameObject prefab;
}

[System.Serializable]
public class TowerPrefab
{
    public Tower monster;
    public GameObject prefab;
}


public class GameGenerator : MonoBehaviour
{
    [Header("Generators")]
    public TerrainGenerator terrainGenerator;
    public PathGenerator pathGenerator;
    public VillageGenerator villageGenerator;

    [Header("Managers")]
    public WaveManager waveManager;
    
    [Header("Game Settings")]
    public int gold = 0;
    
    [Header("Generation Settings")]
    public long seed = -1;

    [Header("Prefabs")]
    public List<MonsterPrefab> monsterPrefabs = new List<MonsterPrefab>();

    [Header("Parents")]
    public GameObject towerParent;

    [Header("Others")]
    public Camera mainCamera;

    [HideInInspector] public List<List<GameObject>> tiles = new List<List<GameObject>>();
    [HideInInspector] public List<GameObject> towers = new List<GameObject>();
    [HideInInspector] public System.Random randomWithSeed;
    
    // Start is called before the first frame update
    void Start()
    {
        if (seed == -1)
        {
            seed = Random.Range(0, 1000000);
        }
        seed = GameManager.instance.gameSeed;
        terrainGenerator.size = GameManager.instance.mapSize;

        randomWithSeed = new System.Random((int)seed);
        terrainGenerator.Generate();
        villageGenerator.GenerateVillage();
        pathGenerator.Generate();
        waveManager.InitializeSurfaces();
    }

    public Rect GetBounds()
    {
        return new Rect(
            2 * terrainGenerator.tileSize * 2,
            2 * terrainGenerator.tileSize * 2,
            Mathf.Max(terrainGenerator.size.x - 3, 2) * terrainGenerator.tileSize * 2,
            Mathf.Max(terrainGenerator.size.y - 3, 2) * terrainGenerator.tileSize * 2
        );
    }

    public bool CanPlace(Vector2 position)
    {
        bool canPlace = true;
        List<Vector2> villageTiles = GetVillageTiles();
        List<Vector2> mainVillageTiles = GetMainVillageTiles();
        if (villageTiles.Contains(position) || mainVillageTiles.Contains(position))
            canPlace = false;
        
        GameObject tile = tiles[(int)position.x][(int)position.y];
        if (tile.GetComponent<TileBehaviour>().type != TileType.Grass)
            canPlace = false;
        
        foreach (GameObject tower in towers)
        {
            if (tower.GetComponent<TowerBehaviour>().position == position)
                canPlace = false;
        }
        
        return canPlace;
    }

    public bool PlaceTower(Vector2 position, GameObject tower)
    {
        if (CanPlace(position))
        {
            GameObject tile = tiles[(int)position.x][(int)position.y];
            GameObject placement = tile.GetComponent<TileBehaviour>().placement;
            tile.GetComponent<TileBehaviour>().structure = tower;
            tower.transform.position = new Vector3(
                placement.transform.position.x,
                placement.transform.position.y + tower.transform.localScale.y / 2f,
                placement.transform.position.z
            );
            tower.transform.parent = towerParent.transform;
            tower.GetComponent<TowerBehaviour>().position = position;
            towers.Add(tower);
            return true;
        }
        return false;
    }

    public List<Vector2> GetMainVillageTiles()
    {
        List<Vector2> tiles = new List<Vector2>();
        Vector2 position = villageGenerator.mainVillage.GetComponent<VillageBehaviour>().position;
        for (int x=-1; x < 2; x++)
        {
            for (int y=-1; y < 2; y++)
                tiles.Add(position + new Vector2(x, y));
        }
        return tiles;
    }

    public GameObject GetMonsterPrefab(Monster type)
    {
        foreach (MonsterPrefab monsterPrefab in monsterPrefabs)
        {
            if (monsterPrefab.monster == type)
                return monsterPrefab.prefab;
        }
        return null;
    }

    public void FlattenAround(Vector2 position, int radius=1, bool centered=true)
    {
        GameObject tile = tiles[(int)position.x][(int)position.y];
        float tileHeight = tile.transform.position.y;
        int finish = centered ? 1 : 0;
        List<GameObject> flattenedTiles = new List<GameObject>();
        List<float> heights = new List<float>();

        flattenedTiles.Add(tile);
        heights.Add(tileHeight);
        for (int x=(int)position.x - radius; x < (int)position.x + radius + finish; x++)
        {
            for (int y=(int)position.y - radius; y < (int)position.y + radius + finish; y++)
            {
                if (x < 0 || x >= terrainGenerator.size.x || y < 0 || y >= terrainGenerator.size.y)
                    continue;
                
                flattenedTiles.Add(tiles[x][y]);
                heights.Add(tiles[x][y].transform.position.y);
            }
        }

        tileHeight = System.Linq.Enumerable.Sum(heights) / heights.Count;
        tileHeight = terrainGenerator.RoundTileHeight(tileHeight / 2f) * 2f;

        foreach (GameObject flattenedTile in flattenedTiles)
        {
            flattenedTile.transform.position = new Vector3(
                flattenedTile.transform.position.x,
                tileHeight,
                flattenedTile.transform.position.z
            );
        }
    }

    public float NormalizeDistance(float distance)
    {
        return distance / CalculateDistanceFromCenter(new Vector2(0, 0));
    }
    
    public float CalculateDistanceBetweenPoints(Vector2 p1, Vector2 p2)
    {
        float distance = Mathf.Sqrt(Mathf.Pow(p1.x - p2.x, 2f) + Mathf.Pow(p1.y - p2.y, 2f));
        return distance;
    }
    
    public float CalculateDistanceFromCenter(Vector2 position)
    {
        Vector2 center = new Vector2(Mathf.Round(terrainGenerator.size.x / 2f), Mathf.Round(terrainGenerator.size.y / 2f));
        float distance = CalculateDistanceBetweenPoints(center, position);

        return distance;
    }

    public float CalculateDistanceFromSide(Vector2 position)
    {
        float distanceFromSideX = Mathf.Min(position.x, terrainGenerator.size.x - position.x - 1);
        float distanceFromSideY = Mathf.Min(position.y, terrainGenerator.size.y - position.y - 1);

        return Mathf.Min(distanceFromSideX, distanceFromSideY);
    }

    public List<Vector2> GetVillageTiles()
    {
        List<Vector2> tiles = new List<Vector2>();
        foreach (GameObject building in villageGenerator.villageBuildings)
        {
            Vector2 position = building.GetComponent<VillageBehaviour>().position;
            
            tiles.Add(position);
            tiles.Add(position + new Vector2(-1f, 0f));
            tiles.Add(position + new Vector2(0f, -1f));
            tiles.Add(position + new Vector2(-1f, -1f));
        }
        return tiles;
    }
    
    public float TileDirectionToVillage(Vector2 initialPosition, Vector2 projectedPosition)
    {
        /*
        Description:
            1 - Calculate the angle between the initial position and the projected position
            2 - Calculate the angle between the initial position and the village position
            3 - Calculate the difference between the two angles
        */
        
        Vector2 villagePosition = villageGenerator.mainVillage.GetComponent<VillageBehaviour>().position;
        float initialToProjected = Mathf.Atan2(projectedPosition.y - initialPosition.y, projectedPosition.x - initialPosition.x) * Mathf.Rad2Deg;
        float initialToVillage = Mathf.Atan2(villagePosition.y - initialPosition.y, villagePosition.x - initialPosition.x) * Mathf.Rad2Deg;

        float angle = initialToVillage - initialToProjected;
        return angle;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
