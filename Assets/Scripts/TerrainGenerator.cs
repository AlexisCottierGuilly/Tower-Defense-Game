using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    [Header("Terrain Settings")]
    public Vector2 size = new Vector2(64, 64);
    public int seed = -1;
    public float tileSize = 1f;

    [Header("3D Models")]
    public GameObject tilePrefab;

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
                    Mathf.Round(GetTileHeight(new Vector2(x, y)) * 2f) / 2f * tileSize * 2,
                    y * tileSize * 2
                );

                GameObject new_tile = GameObject.Instantiate(tilePrefab, position, Quaternion.identity);
                new_tile.transform.parent = gameObject.transform;

                new_tile.transform.localScale = new Vector3(
                    tileSize,
                    tileSize,
                    tileSize
                );

                tiles[x].Add(new_tile);
            }
        }
    }

    float GetTileHeight(Vector2 position)
    {
        float height1 = Mathf.PerlinNoise(
            (position.x + seed) / 30f,
            (position.y + seed) / 30f
        );

        float height2 = Mathf.PerlinNoise(
            (position.x + seed) / 10f,
            (position.y + seed) / 10f
        );

        float final_height = (height1 + height2) * 3f;

        return final_height;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
