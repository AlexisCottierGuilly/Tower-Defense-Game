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

        return AddCentralHill(position, final_height);
    }

    float AddCentralHill(Vector2 position, float height)
    {
        // Distance between 0 and 1
        float distance = NormalizeDistance(CalculateDistanceFromCenter(position));
        distance = Mathf.Max(1f - distance, 0.5f);
        //float width = Mathf.Min(
        //    Mathf.Max(hillWidth * 0.75f, 0f),
        //    1f
        //);
        //distance = ((1f / hillWidth - distance) / 2f) + width;
        //distance = Mathf.Max(distance, 1f);

        height *= 0.8f + (distance / 3f);

        //float new_height = height * Mathf.Pow(distance, 3f * hillHeight / hillWidth);
        float a = 0.45f * hillHeight / hillWidth;
        float a1 = 10f / hillSmoothness;
        float h = 7.5f / hillWidth - 1f;
        float k = 1.5f * a + 1;

        float new_height = height * (a * Mathf.Atan(a1 * distance - h) + k);
        return new_height;
    }

    float NormalizeDistance(float distance)
    {
        return distance / CalculateDistanceFromCenter(new Vector2(0, 0));
    }
    
    float CalculateDistanceFromCenter(Vector2 position)
    {
        Vector2 center = new Vector2(Mathf.Round(size.x / 2f), Mathf.Round(size.y / 2f));
        float distance = Mathf.Sqrt(Mathf.Pow(center.x - position.x, 2f) + Mathf.Pow(center.y - position.y, 2f));

        return distance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
