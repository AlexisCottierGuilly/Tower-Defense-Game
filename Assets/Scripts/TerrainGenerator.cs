using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    [Header("Terrain Settings")]
    public Vector2 size = new Vector2(64, 64);
    public float tileSize = 1f;

    [Header("3D Models")]
    public GameObject tilePrefab;

    private List<List<GameObject>> tiles = new List<List<GameObject>>();

    
    // Start is called before the first frame update
    void Start()
    {
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
                    x * tileSize,
                    0,
                    y * tileSize
                );

                GameObject new_tile = Instantiate(tilePrefab, position, Quaternion.identity);
                new_tile.transform.parent = transform;

                new_tile.transform.localScale = new Vector3(
                    tileSize,
                    tileSize,
                    tileSize
                );

                tiles[x].Add(new_tile);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
