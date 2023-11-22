using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    [Header("Terrain Settings")]
    public Vector2 size = new Vector2(64, 64);
    public float scale = 1f;
    public long seed = -1;
    public float tileSize = 1f;
    [Space]
    public float hillHeight = 1f;
    public float hillWidth = 1f;
    public float hillSmoothness = 1f;
    public List<Vector2> mountains = new List<Vector2>();
    public List<List<Vector2>> paths = new List<List<Vector2>>();
    public float stepHeight = 1f;
    [Space]
    public bool addFog = true;

    [Header("3D Models")]
    public GameObject grassTilePrefab;
    public GameObject pathTilePrefab;
    public GameObject fogPrefab;
    [Space]
    public GameObject mainVillagePrefab;
    public List<GameObject> villagePrefabs = new List<GameObject>();
    public List<GameObject> towerPrefabs = new List<GameObject>();

    [Header("Parents")]
    public GameObject terrainParent;
    public GameObject villageParent;

    [Header("Others")]
    public Camera mainCamera;

    private List<List<GameObject>> tiles = new List<List<GameObject>>();
    [HideInInspector] public GameObject mainVillage;
    [HideInInspector] public List<GameObject> villageBuildings = new List<GameObject>();
    [HideInInspector] public List<GameObject> towers = new List<GameObject>();
    private System.Random randomWithSeed;

    
    // Start is called before the first frame update
    void Start()
    {
        if (seed == -1)
        {
            seed = Random.Range(0, 1000000);
        }
        randomWithSeed = new System.Random((int)seed);
        GenerateMountains();
        GenerateTerrain();

        // TODO: Generate paths
    }

    public Rect GetBounds()
    {
        return new Rect(
            0,
            0,
            Mathf.Max(size.x - 3, 0) * tileSize * 2,
            Mathf.Max(size.y - 3, 0) * tileSize * 2
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
        
        return canPlace;
    }

    public List<Vector2> GetMainVillageTiles()
    {
        List<Vector2> tiles = new List<Vector2>();
        Vector2 position = mainVillage.GetComponent<VillageBehaviour>().position;
        for (int x=-1; x < 2; x++)
        {
            for (int y=-1; y < 2; y++)
                tiles.Add(position + new Vector2(x, y));
        }
        Debug.Log(tiles.Count);
        return tiles;
    }

    public void GenerateTerrain()
    {
        for (int x=0; x < size.x; x++)
        {
            tiles.Add(new List<GameObject>());

            for (int y=0; y < size.y; y++)
            {
                Vector3 position = new Vector3(
                    x * tileSize * 2,
                    RoundTileHeight(GetTileHeight(new Vector2(x, y))) * 2f,
                    y * tileSize * 2
                );

                GameObject tilePrefab = grassTilePrefab;

                GameObject new_tile = GameObject.Instantiate(tilePrefab, position, Quaternion.identity);
                new_tile.name = $"Tile ({x}, {y})";
                TileBehaviour behaviour = new_tile.GetComponent<TileBehaviour>();
                behaviour.position = new Vector2(x, y);
                behaviour.type = TileType.Grass;
                new_tile.transform.parent = terrainParent.transform;

                if (addFog && (x == 0 || x == size.x - 1 || y == 0 || y == size.y - 1))
                {
                    GameObject fog = GameObject.Instantiate(fogPrefab, position, Quaternion.identity);
                    fog.name = $"Fog ({x}, {y})";
                    fog.transform.parent = terrainParent.transform;
                }

                float multiplier = 100f;
                new_tile.transform.localScale = new Vector3(
                    multiplier * tileSize,
                    multiplier * tileSize,
                    multiplier / 15f
                );

                new_tile.transform.Rotate(new Vector3(-90f, 0f, 0f));

                tiles[x].Add(new_tile);
            }
        }

        GenerateVillage();
        GeneratePaths();

        // now change the model of the tiles that are paths to pathTilePrefab
        for (int i=0; i < paths.Count; i++)
        {
            for (int j=0; j < paths[i].Count; j++)
            {
                Vector2 position = paths[i][j];
                GameObject tile = tiles[(int)position.x][(int)position.y];
                GameObject new_tile = GameObject.Instantiate(pathTilePrefab, tile.transform.position, Quaternion.identity);
                new_tile.name = tile.name;
                new_tile.transform.parent = tile.transform.parent;
                new_tile.transform.localScale = tile.transform.localScale;
                new_tile.transform.rotation = tile.transform.rotation;

                TileBehaviour behaviour = new_tile.GetComponent<TileBehaviour>();
                behaviour.position = position;
                behaviour.type = TileType.Path;

                DestroyImmediate(tile);
                tiles[(int)position.x][(int)position.y] = new_tile;
            }
        }
    }

    public void GenerateVillage()
    {
        /*
        loop in the 50% middle of the map
        find the highest point and place the main village there
        place 4 towers around the main village, randomly
        */

        int x = (int)(size.x / 3.5f);
        int y = (int)(size.y / 3.5f);

        int finalX = (int)size.x - x;
        int finalY = (int)size.y - y;

        float highestPoint = 0f;
        Vector2 highestPointPosition = new Vector2();
        GameObject highestPointTile = new GameObject();

        for (int i=x; i < finalX; i++)
        {
            for (int j=y; j < finalY; j++)
            {
                float height = GetTileHeight(new Vector2(i, j));
                if (height > highestPoint)
                {
                    highestPoint = height;
                    highestPointPosition = new Vector2(i, j);
                    highestPointTile = tiles[i][j];
                }
            }
        }

        FlattenAround(highestPointPosition);

        Vector3 position = highestPointTile.GetComponent<TileBehaviour>().placement.transform.position;
        mainVillage = GameObject.Instantiate(
            mainVillagePrefab,
            new Vector3(0f, 0f, 0f),
            Quaternion.identity
        );
        mainVillage.transform.position = new Vector3(
            position.x,
            position.y + mainVillage.transform.localScale.y / 2f,
            position.z
        );

        mainVillage.name = "Main Village";
        mainVillage.transform.parent = villageParent.transform;
        mainVillage.GetComponent<VillageBehaviour>().position = highestPointPosition;

        float cameraHeight = mainVillage.transform.position.y + mainVillage.GetComponent<MeshRenderer>().bounds.size.y + stepHeight * 2f;
        mainCamera.GetComponent<CameraManager>().SetHeight(cameraHeight);

        // place 4 towers around the main village, randomly
        float maxDistanceFromCenter = Mathf.Round(size.x / 4f);
        float minDistanceFromMainTower = Mathf.Round(size.x / 8f);
        Vector2 center = new Vector2(size.x / 2f, size.y / 2f);
        List<Vector2> villagePositions = new List<Vector2>();

        for (int i=0; i < 4; i++)
        {
            Vector2 randomPosition = new Vector2();
            while (villagePositions.Contains(randomPosition) || randomPosition == Vector2.zero
                || (randomPosition - highestPointPosition).magnitude < minDistanceFromMainTower)
            {
                randomPosition.x = randomWithSeed.Next(
                    (int)center.x - (int)maxDistanceFromCenter,
                    (int)center.x + (int)maxDistanceFromCenter
                );
                randomPosition.y = randomWithSeed.Next(
                    (int)center.y - (int)maxDistanceFromCenter,
                    (int)center.y + (int)maxDistanceFromCenter
                );
            }
            FlattenAround(randomPosition, 1, false);
            villagePositions.Add(randomPosition);

            GameObject villageStructurePrefab = villagePrefabs[randomWithSeed.Next(0, villagePrefabs.Count)];
            GameObject tile = tiles[(int)randomPosition.x][(int)randomPosition.y];
            position = tile.GetComponent<TileBehaviour>().placement.transform.position;
            GameObject villageStructure = GameObject.Instantiate(
                villageStructurePrefab,
                new Vector3(0f, 0f, 0f),
                Quaternion.identity);
            villageStructure.transform.position = new Vector3(
                position.x - tile.GetComponent<MeshRenderer>().bounds.size.x / 2f,
                position.y + villageStructure.transform.localScale.y / 2f,
                position.z - tile.GetComponent<MeshRenderer>().bounds.size.z / 2f
            );
            
            villageStructure.name = $"Village Structure {i}";
            villageStructure.transform.parent = villageParent.transform;
            villageStructure.GetComponent<VillageBehaviour>().position = randomPosition;
            villageBuildings.Add(villageStructure);
        }
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
                if (x < 0 || x >= size.x || y < 0 || y >= size.y)
                    continue;
                
                flattenedTiles.Add(tiles[x][y]);
                heights.Add(tiles[x][y].transform.position.y);
            }
        }

        tileHeight = System.Linq.Enumerable.Sum(heights) / heights.Count;
        tileHeight = RoundTileHeight(tileHeight);

        foreach (GameObject flattenedTile in flattenedTiles)
        {
            flattenedTile.transform.position = new Vector3(
                flattenedTile.transform.position.x,
                tileHeight,
                flattenedTile.transform.position.z
            );
        }
    }

    public void GenerateMountains()
    {
        mountains = new List<Vector2>();
        for (int side=0; side < 4; side++)
        {
            Vector2 mountainPosition = new Vector2();
            float row = randomWithSeed.Next(0, (int)size.x);
            if (side == 0f)
            {
                mountainPosition.y = 2f;
                mountainPosition.x = row;
            }
            else if (side == 1f)
            {
                mountainPosition.y = size.y - 3f;
                mountainPosition.x = row;
            }
            else if (side == 2f)
            {
                mountainPosition.x = 2f;
                mountainPosition.y = row;
            }
            else if (side == 3f)
            {
                mountainPosition.x = size.x - 3f;
                mountainPosition.y = row;
            }

            mountains.Add(mountainPosition);
        }
    }

    float RoundTileHeight(float height)
    {
        float multiplier = (1 / (stepHeight / 2f));
        return Mathf.Round(height * multiplier) / multiplier;
    }
    
    float GetTileHeight(Vector2 position)
    {
        float height1 = Mathf.PerlinNoise(
            (position.x + seed) / 20f / scale,
            (position.y + seed) / 20f / scale
        );

        float height2 = Mathf.PerlinNoise(
            (position.x + seed) / 7f / scale,
            (position.y + seed) / 7f / scale
        );

        float hills1 = Mathf.PerlinNoise(
            (position.x + seed) / 7f / scale,
            (position.y + seed) / 7f / scale
        );
        hills1 = Mathf.Max(hills1, 0f);

        float final_height = (height1 + height2) * 5f;
        final_height = (final_height + Mathf.Sqrt(0.2f * Mathf.Pow(hills1 + 1f, 4f)) * 1.35f) / 2f;

        return AddMountains(position, AddCentralHill(position, final_height));
    }

    float AddCentralHill(Vector2 position, float height)
    {
        // Distance between 0 and 1
        float distance = NormalizeDistance(CalculateDistanceFromCenter(position));
        distance = Mathf.Max(1f - distance, 0.5f);

        height *= 0.8f + (distance / 3f);

        float a = 0.45f * hillHeight / hillWidth;
        float a1 = 10f / hillSmoothness;
        float h = 7.5f / hillWidth - 1f;
        float k = 1.5f * a + 1;

        float new_height = height * (a * Mathf.Atan(a1 * distance - h) + k);
        return new_height;
    }

    float AddMountains(Vector2 position, float height)
    {
        List<float> proximities = new List<float>();
        foreach (Vector2 mountain in mountains)
        {
            float distance = NormalizeDistance(CalculateDistanceBetweenPoints(position, mountain));
            float distanceFromSide = Mathf.Max(0.075f - NormalizeDistance(CalculateDistanceFromSide(position)), 0f);
            distance = Mathf.Max(1f - distance, 0.5f) + distanceFromSide / 1.5f;
            proximities.Add(distance);
        }

        float meanDistance = System.Linq.Enumerable.Sum(proximities) / (float)proximities.Count;
        return height * Mathf.Pow((meanDistance + 0.5f), 6f);
    }

    float NormalizeDistance(float distance)
    {
        return distance / CalculateDistanceFromCenter(new Vector2(0, 0));
    }
    
    float CalculateDistanceBetweenPoints(Vector2 p1, Vector2 p2)
    {
        float distance = Mathf.Sqrt(Mathf.Pow(p1.x - p2.x, 2f) + Mathf.Pow(p1.y - p2.y, 2f));
        return distance;
    }
    
    float CalculateDistanceFromCenter(Vector2 position)
    {
        Vector2 center = new Vector2(Mathf.Round(size.x / 2f), Mathf.Round(size.y / 2f));
        float distance = CalculateDistanceBetweenPoints(center, position);

        return distance;
    }

    float CalculateDistanceFromSide(Vector2 position)
    {
        float distanceFromSideX = Mathf.Min(position.x, size.x - position.x - 1);
        float distanceFromSideY = Mathf.Min(position.y, size.y - position.y - 1);

        return Mathf.Min(distanceFromSideX, distanceFromSideY);
    }

    public void GeneratePathStarts()
    {
        /*
        The paths start on the sides of the map, at the lowest spots.
        Store the start paths in the paths list (first element is the start)
        */
        int numberOfPaths = 4;

        for (int i=0; i < numberOfPaths; i++)
        {
            // take the lowest spot on each side
            float lowestHeight = -1f;
            Vector2 lowestPosition = new Vector2(-1f, -1f);
            List<Vector2> allSidePositions = new List<Vector2>();
            
            if (i == 0)
            {
                // left side
                for (int j=0; j < size.y; j++)
                   allSidePositions.Add(new Vector2(0f, j));
            }
            else if (i == 1)
            {
                // top side
                for (int j=0; j < size.x; j++)
                    allSidePositions.Add(new Vector2(j, 0f));
            }
            else if (i == 2)
            {
                // right side
                for (int j=0; j < size.y; j++)
                    allSidePositions.Add(new Vector2(size.x - 1f, j));
            }
            else if (i == 3)
            {
                // bottom side
                for (int j=0; j < size.x; j++)
                   allSidePositions.Add(new Vector2(j, size.y - 1f));
            }

            foreach (Vector2 position in allSidePositions)
            {
                float height = tiles[(int)position.x][(int)position.y].transform.position.y;
                if (lowestHeight == -1f || height < lowestHeight)
                {
                    lowestHeight = height;
                    lowestPosition = position;
                }
            }

            // add the lowest spot to the paths list
            paths.Add(new List<Vector2>());
            paths[i].Add(lowestPosition);
        }
    }

    List<Vector2> GetVillageTiles()
    {
        List<Vector2> tiles = new List<Vector2>();
        foreach (GameObject building in villageBuildings)
        {
            Vector2 position = building.GetComponent<VillageBehaviour>().position;
            
            tiles.Add(position);
            tiles.Add(position + new Vector2(-1f, 0f));
            tiles.Add(position + new Vector2(0f, -1f));
            tiles.Add(position + new Vector2(-1f, -1f));
        }
        return tiles;
    }

    void GeneratePaths()
    {
        // the start is already in the paths list (first element is the start)
        // the end is the block where is the main tower and the paths should avoid getting under village buildings
        GeneratePathStarts();

        foreach (List<Vector2> path in paths)
        {
            Vector2 start = path[0];
            Vector2 end = mainVillage.GetComponent<VillageBehaviour>().position;

            Vector2 current = start;
            Vector2 previous = start;
            Vector2 next = new Vector2();
            List<Vector2> usedTiles = new List<Vector2>();
            List<Vector2> villages = GetVillageTiles();

            usedTiles.Add(start);
            float pathEfficiency = 0.75f; //0.5f;
            int count = 0;

            while (current != end)
            {
                next = GetNextPathTile(current, end, usedTiles, pathEfficiency, villages, path);
                if (next.x != -1 && next.y != -1)
                {
                    path.Add(next);
                    usedTiles.Add(next);
                    previous = current;
                    current = next;
                }
                else
                {
                    path.Remove(current);
                    current = previous;
                    if (path.Count > 1)
                        previous = path[path.Count - 2];
                    else
                        previous = start;
                }

                if (current == start)
                {
                    Debug.Log("No possible path");
                    break;
                }
                
                count++;
                if (count > 1000)
                {
                    Debug.Log("Path generation error");
                    break;
                }
            }

            if (current != start)
                path.Add(end);
        }
    }

    float AverageDistanceFromVillage(Vector2 position)
    {
        float distance = 0f;
        foreach (GameObject building in villageBuildings)
        {
            Vector2 buildingPosition = building.GetComponent<VillageBehaviour>().position;
            distance += Vector2.Distance(position, buildingPosition);
        }

        return distance / (float)(villageBuildings.Count);
    }
    
    float TileDirectionToVillage(Vector2 initialPosition, Vector2 projectedPosition)
    {
        /*
        Description:
            1 - Calculate the angle between the initial position and the projected position
            2 - Calculate the angle between the initial position and the village position
            3 - Calculate the difference between the two angles
        */
        
        Vector2 villagePosition = mainVillage.GetComponent<VillageBehaviour>().position;
        float initialToProjected = Mathf.Atan2(projectedPosition.y - initialPosition.y, projectedPosition.x - initialPosition.x) * Mathf.Rad2Deg;
        float initialToVillage = Mathf.Atan2(villagePosition.y - initialPosition.y, villagePosition.x - initialPosition.x) * Mathf.Rad2Deg;

        float angle = initialToVillage - initialToProjected;
        return angle;
    }
    
    Vector2 GetNextPathTile(Vector2 current, Vector2 end, List<Vector2> usedTiles,
        float efficiency, List<Vector2> villages, List<Vector2> path)
    {
        List<Vector2> tilesAround = GetTilesAround(current);
        Vector2 bestMatch = new Vector2(-1, -1);
        Vector2 bestMatch2 = new Vector2(-1, -1);
        Vector2 bestMatch3 = new Vector2(-1, -1);
        Vector2 bestMatch4 = new Vector2(-1, -1);

        float bestDistance = -1f;
        float bestStep = -1f;
        GameObject currentTile = tiles[(int)current.x][(int)current.y];

        foreach (Vector2 tile in tilesAround)
        {
            // float direction = TileDirectionToVillage(current, tile);
            // if the tile is beside a tile in the path (that is not current), skip it
            /*bool besidePath = false;
            foreach (Vector2 pathTile in path)
            {
                if (pathTile == current || pathTile == tile)
                    continue;
                
                int xDist = Mathf.Abs((int)tile.x - (int)pathTile.x);
                int yDist = Mathf.Abs((int)tile.y - (int)pathTile.y);
                if ((xDist == 1 && yDist == 0) || (xDist == 0 && yDist == 1))
                {
                    besidePath = true;
                    break;
                }
            }
            if (besidePath)
                continue;
            */
            
            GameObject otherTile = tiles[(int)tile.x][(int)tile.y];
            float heightDistance = Mathf.Abs(otherTile.transform.position.y - currentTile.transform.position.y);
            if (heightDistance > stepHeight * 2f || usedTiles.Contains(tile) ||
                villages.Contains(tile))
                continue;
            
            float distance = Vector2.Distance(tile, end); // / 1.25f + AverageDistanceFromVillage(tile);
            if (bestDistance == -1f || distance < bestDistance || (distance <= bestDistance &&
                heightDistance < bestStep && randomWithSeed.NextDouble() > 0.25f))
            {
                bestDistance = distance;
                bestStep = heightDistance;

                bestMatch4 = bestMatch3;
                bestMatch3 = bestMatch2;
                bestMatch2 = bestMatch;
                bestMatch = tile;
            }
        }

        // use the efficiency to choose the adequate tile
        // if the efficiency is 1, the best tile is chosen
        // the efficiency influences the choice of the tile (by random)

        float random = (float)randomWithSeed.NextDouble();
        // take into account that the tiles can be invalid
        Vector2 returnedTile = bestMatch;
        if (random > efficiency * 1.5f && bestMatch2.x != -1 && bestMatch2.y != -1)
        {
            returnedTile = bestMatch2;
            if (random > efficiency * 2.5f && bestMatch3.x != -1 && bestMatch3.y != -1)
            {
                returnedTile = bestMatch3;
                if (random > efficiency * 4f && bestMatch4.x != -1 && bestMatch4.y != -1)
                    returnedTile = bestMatch4;
            }
        }

        return returnedTile;
    }

    List<Vector2> GetTilesAround(Vector2 position)
    {
        List<Vector2> tilesAround = new List<Vector2>();
        // top, right, bottom, left
        Vector2[] directions = { new Vector2(0, 1), new Vector2(1, 0), new Vector2(0, -1), new Vector2(-1, 0) };

        foreach (Vector2 direction in directions)
        {
            Vector2 tile = position + direction;
            if (tile.x >= 0 && tile.x < size.x && tile.y >= 0 && tile.y < size.y)
                tilesAround.Add(tile);
        }

        return tilesAround;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
