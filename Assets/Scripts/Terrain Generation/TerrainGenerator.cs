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

    [Header("3D Models")]
    public GameObject tilePrefab;

    [Header("Parents")]
    public GameObject terrainParent;

    private List<List<GameObject>> tiles = new List<List<GameObject>>();

    
    // Start is called before the first frame update
    void Start()
    {
        if (seed == -1)
        {
            seed = Random.Range(0, 1000000);
        }
        GenerateMountains();
        Generate();
    }

    void Generate()
    {
        for (int x=0; x < size.x; x++)
        {
            tiles.Add(new List<GameObject>());

            for (int y=0; y < size.y; y++)
            {
                Vector3 position = new Vector3(
                    x * tileSize * 2,
                    Mathf.Round(GetTileHeight(new Vector2(x, y)) * 2f) / 2f * 2f,
                    y * tileSize * 2
                );

                GameObject new_tile = GameObject.Instantiate(tilePrefab, position, Quaternion.identity);
                new_tile.name = $"Tile ({x}, {y})";
                new_tile.transform.parent = terrainParent.transform;

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
    }

    void GenerateMountains()
    {
        mountains = new List<Vector2>();
        System.Random randomWithSeed = new System.Random((int)seed);
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
            distance = Mathf.Max(1f - distance, 0.5f) + distanceFromSide;
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
