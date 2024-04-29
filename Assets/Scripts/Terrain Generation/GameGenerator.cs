using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

[System.Serializable]
public class MonsterPrefab
{
    public Monster monster;
    public MonsterData data;
}

[System.Serializable]
public class TowerPrefab
{
    public TowerType tower;
    public TowerData data;
    public Texture2D icon;
    public KeyCode key = KeyCode.Alpha1;
}


public class GameGenerator : MonoBehaviour
{
    [Header("Generators")]
    public TerrainGenerator terrainGenerator;
    public PathGenerator2 pathGenerator;
    public VillageGenerator villageGenerator;
    public DecorationGenerator decorationGenerator;

    [Header("Managers")]
    public WaveManager waveManager;
    public NotificationManager notificationManager;
    public ShootingManager shootingManager;
    
    [Header("Game Settings")]
    // public int gold = 0;
    public int maxHealth = 0;
    public int health = 0;
    public float gameTime = 0f;
    public bool paused = false;
    [Space]
    public float defaultSpeed = 1f;
    
    [Header("Generation Settings")]
    public long seed = -1;
    // good seed : 1821042

    [Header("Prefabs")]
    public List<MonsterPrefab> monsterPrefabs = new List<MonsterPrefab>();
    public List<TowerPrefab> towerPrefabs = new List<TowerPrefab>();

    [Header("Parents")]
    public GameObject towerParent;
    public GameObject projectileParent;

    [Header("Others")]
    public Camera mainCamera;
    public TextMeshProUGUI seedText;
    public float logicUpdateDelay = 1f;

    [HideInInspector] public List<List<GameObject>> tiles = new List<List<GameObject>>();
    [HideInInspector] public List<GameObject> towers = new List<GameObject>();
    [HideInInspector] public List<GameObject> decorations = new List<GameObject>();
    [HideInInspector] public System.Random randomWithSeed;
    
    [HideInInspector] public bool didModifyVillage = false;
    [HideInInspector] public bool didFinishLoading = false;
    
    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.generator = this;
        
        GameManager.instance.gold = GameManager.instance.initialGold;
        
        seed = GameManager.instance.gameSeed;
        if (seed == -1)
            seed = Random.Range(0, GameManager.instance.maxGameSeed);
        
        seedText.text = seed.ToString();
        
        terrainGenerator.size = GameManager.instance.mapSize;

        randomWithSeed = new System.Random((int)seed);
        terrainGenerator.Generate();
        villageGenerator.GenerateVillage();
        didModifyVillage = true;

        pathGenerator.Generate();
        decorationGenerator.AddDecorations();
        
        waveManager.InitializeSurfaces();
        notificationManager.ShowNotification("Bonne chance !");

        maxHealth = villageGenerator.GetRemainingLives();
        health = maxHealth;

        waveManager.gameFinished.AddListener(GameDidFinish);

        StartCoroutine(LogicUpdate());
        StartCoroutine(TowerUpdate());

        Physics.gravity = new Vector3(
            0f,
            -200f,
            0f
        );

        Time.timeScale = defaultSpeed;
    }

    public Rect GetBounds()
    {
        /* return new Rect(
            2 * terrainGenerator.tileSize * 2,
            2 * terrainGenerator.tileSize * 2,
            Mathf.Max(terrainGenerator.size.x - 3, 2) * terrainGenerator.tileSize * 2,
            Mathf.Max(terrainGenerator.size.y - 3, 2) * terrainGenerator.tileSize * 2
        ); */

        // take into account that it is a grid made out of hexagonal tiles, so the bounds are different

        float xTileSize = (5f / 3f) * terrainGenerator.tileSize;
        float yTileSize = Mathf.Sqrt(2) * terrainGenerator.tileSize;

        float minX = xTileSize / 4f;
        float minY = 0f;
        float maxX = (terrainGenerator.size.x + 0f) * xTileSize;
        float maxY = (terrainGenerator.size.y + 1f / 3f) * yTileSize;

        return new Rect(minX, minY, maxX, maxY);
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
        TowerBehaviour behaviour = tower.GetComponent<TowerBehaviour>();
        TowerData data = behaviour.data;
        
        if (CanPlace(position) && GameManager.instance.gold >= data.cost)
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
            behaviour.position = position;
            behaviour.projectileParent = projectileParent;
            towers.Add(tower);
            GameManager.instance.gold -= data.cost;

            return true;
        }

        return false;
    }

    public bool PlaceVillage(Vector2 position, GameObject village)
    {
        VillageBehaviour behaviour = village.GetComponent<VillageBehaviour>();
        VillageData data = village.GetComponent<VillageBehaviour>().data;

        if (data.cost != -1 && CanPlace(position) && GameManager.instance.gold >= data.cost)
        {
            villageGenerator.PlaceVillage(village, position);
            GameManager.instance.gold -= data.cost;
            didModifyVillage = true;

            return true;
        }

        return false;
    }

    public List<Vector2> GetMainVillageTiles()
    {
        List<Vector2> tiles = new List<Vector2>();

        if (villageGenerator.mainVillage == null)
            return tiles;
        
        Vector2 position = villageGenerator.mainVillage.GetComponent<VillageBehaviour>().position;
        /* for (int x=-1; x < 2; x++)
        {
            for (int y=-1; y < 2; y++)
                tiles.Add(position + new Vector2(x, y));
        } */
        tiles.Add(position);

        return tiles;
    }

    public List<Vector2> GetVillageTiles()
    {
        List<Vector2> tiles = new List<Vector2>();
        foreach (GameObject building in villageGenerator.villageBuildings)
        {
            Vector2 position = building.GetComponent<VillageBehaviour>().position;
            
            tiles.Add(position);
            //tiles.Add(position + new Vector2(-1f, 0f));
            //tiles.Add(position + new Vector2(0f, -1f));
            //tiles.Add(position + new Vector2(-1f, -1f));
        }
        return tiles;
    }

    public GameObject GetMonsterPrefab(Monster type)
    {
        foreach (MonsterPrefab monsterPrefab in monsterPrefabs)
        {
            if (monsterPrefab.monster == type)
                return monsterPrefab.data.prefab;
        }
        return null;
    }

    public GameObject GetTowerPrefab(TowerType type)
    {
        foreach (TowerPrefab towerPrefab in towerPrefabs)
        {
            if (towerPrefab.tower == type)
                return towerPrefab.data.prefab;
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

    IEnumerator LogicUpdate()
    {
        while (true)
        {
            if (!waveManager.waveFinished)
            {
                foreach (GameObject monster in waveManager.monsters)
                {
                    if (monster != null)
                    {
                        MonsterBehaviour behaviour = monster.GetComponent<MonsterBehaviour>();
                        bool modifyVillageObjective = didModifyVillage;
                        behaviour.UpdateObjective(modifyVillageObjective);
                    }
                }
            }

            didModifyVillage = false;

            // Debug.Log("Monster Logic updated");
            yield return new WaitForSeconds(logicUpdateDelay);
        }
    }

    public void CleanTowersList()
    {
        List<GameObject> newTowers = new List<GameObject>();
        foreach (GameObject tower in towers)
        {
            if (tower != null)
                newTowers.Add(tower);
        }

        towers = newTowers;
    }

    IEnumerator TowerUpdate()
    {   
        while (true)
        {
            if (!waveManager.waveFinished)
            {
                CleanTowersList();
                
                foreach (GameObject tower in towers)
                {
                    TowerBehaviour behaviour = tower.GetComponent<TowerBehaviour>();
                    behaviour.UpdateLogic();
                }
            }
            // Debug.Log("Tower Logic updated");
            yield return new WaitForSeconds(0.05f);
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        paused = true;
    }

    public void ResumeGame()
    {
        Time.timeScale = defaultSpeed;
        paused = false;
    }

    public void GameDidFinish()
    {
        int crystals = 0;
        foreach (GameObject building in villageGenerator.villageBuildings)
        {
            VillageBehaviour behaviour = building.GetComponent<VillageBehaviour>();
            crystals += 1;
        }

        crystals += GameManager.instance.GetDifficultyModifier().crystals;

        GameManager.instance.player.crystals += crystals;

        
    }

    void Update()
    {
        health = villageGenerator.GetRemainingLives();
        
        if (!paused)
        {
            gameTime += Time.deltaTime;
            if (health > 0)
                didFinishLoading = true;
        }
    }
}
