using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public GameGenerator gameGenerator;
    
    [Header("Settings")]
    public Vector2 size = new Vector2(32, 32);
    public float scale = 1f;
    public float tileSize = 1f;
    [Space]
    public float hillHeight = 1f;
    public float hillWidth = 1f;
    public float hillSmoothness = 1f;
    public List<Vector2> mountains = new List<Vector2>();
    public float stepHeight = 1f;
    [Space]
    public bool addFog = true;

    [Header("3D Models")]
    public GameObject grassTilePrefab;
    public GameObject fogPrefab;
    [Space]
    public GameObject terrainParent;
    
    public void Generate()
    {
        GenerateMountains();
        
        float x_pos = 0;
        float y_pos = 0;
        
        for (int x=0; x < size.x; x++)
        {
            gameGenerator.tiles.Add(new List<GameObject>());

            for (int y=0; y < size.y; y++)
            {
                float offset = (x % 2 == 0 ? Mathf.Sqrt(0.75f) : 0f);

                Vector3 position = new Vector3(
                    (x_pos + offset) * tileSize,
                    RoundTileHeight(GetTileHeight(new Vector2(x, y))) * 2f,
                    y_pos * tileSize
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
                    multiplier * 2f /// 15f
                );

                new_tile.transform.Rotate(new Vector3(-90f, 0f, 0f));

                gameGenerator.tiles[x].Add(new_tile);
                x_pos += Mathf.Sqrt(3f);
            }
            y_pos += 0.75f * 2f;
            x_pos = 0;
        }
    }
    
    public void GenerateMountains()
    {
        mountains = new List<Vector2>();
        for (int side=0; side < 4; side++)
        {
            Vector2 mountainPosition = new Vector2();
            float row = gameGenerator.randomWithSeed.Next(0, (int)size.x);
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

    public float RoundTileHeight(float height)
    {
        float multiplier = (1 / (stepHeight / 2f));
        return Mathf.Round(height * multiplier) / multiplier;
    }
    
    public float GetTileHeight(Vector2 position)
    {
        float height1 = Mathf.PerlinNoise(
            (position.x + gameGenerator.seed) / 20f / scale,
            (position.y + gameGenerator.seed) / 20f / scale
        );
        float height2 = Mathf.PerlinNoise(
            (position.x + gameGenerator.seed) / 10f / scale,
            (position.y + gameGenerator.seed) / 10f / scale
        );
        float final_height = height1 + height2;

        float hills1 = Mathf.PerlinNoise(
            (position.x + gameGenerator.seed) / 5f / scale,
            (position.y + gameGenerator.seed) / 5f / scale
        );
        hills1 = Mathf.Max(hills1, 0f);

        float cliffValue = Mathf.PerlinNoise(
            (position.x + gameGenerator.seed + 1024f) / 12f / scale,
            (position.y + gameGenerator.seed + 1024f) / 12f / scale
        );

        if (cliffValue > 0.25f)
        {
            float cliffModifier = Mathf.PerlinNoise(
                (position.x + gameGenerator.seed + 512f) / 8f / scale,
                (position.y + gameGenerator.seed + 512f) / 8f / scale
            );
            cliffModifier /= 0.75f;
            
            final_height += cliffModifier;
        }

        final_height *= 5f;
        final_height = (final_height + Mathf.Sqrt(0.2f * Mathf.Pow(hills1 + 1f, 4f)) * 1.35f) / 2f;

        return AddMountains(position, AddCentralHill(position, final_height));
    }

    float AddCentralHill(Vector2 position, float height)
    {
        // Distance between 0 and 1
        float distance = gameGenerator.NormalizeDistance(gameGenerator.CalculateDistanceFromCenter(position));
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
            float distance =gameGenerator. NormalizeDistance(gameGenerator.CalculateDistanceBetweenPoints(position, mountain));
            float distanceFromSide = Mathf.Max(0.075f - gameGenerator.NormalizeDistance(gameGenerator.CalculateDistanceFromSide(position)), 0f);
            distance = Mathf.Max(1f - distance, 0.5f) + distanceFromSide / 1.5f;
            proximities.Add(distance);
        }

        float meanDistance = System.Linq.Enumerable.Sum(proximities) / (float)proximities.Count;
        return height * Mathf.Pow((meanDistance + 0.5f), 6f);
    }
}
