using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillageGenerator : MonoBehaviour
{
    public GameGenerator gameGenerator;

    [Header("3D Models")]
    public GameObject mainVillagePrefab;
    public List<GameObject> villagePrefabs = new List<GameObject>();

    [Header("Parents")]
    public GameObject villageParent;

    [HideInInspector] public GameObject mainVillage;
    [HideInInspector] public List<GameObject> villageBuildings = new List<GameObject>();

    public void GenerateVillage()
    {
        /*
        loop in the 50% middle of the map
        find the highest point and place the main village there
        place 4 towers around the main village, randomly
        */

        int x = (int)(gameGenerator.terrainGenerator.size.x / 3.5f);
        int y = (int)(gameGenerator.terrainGenerator.size.y / 3.5f);

        int finalX = (int)gameGenerator.terrainGenerator.size.x - x;
        int finalY = (int)gameGenerator.terrainGenerator.size.y - y;

        float highestPoint = 0f;
        Vector2 highestPointPosition = new Vector2();
        GameObject highestPointTile = new GameObject();

        for (int i=x; i < finalX; i++)
        {
            for (int j=y; j < finalY; j++)
            {
                float height = gameGenerator.terrainGenerator.GetTileHeight(new Vector2(i, j));
                if (height > highestPoint)
                {
                    highestPoint = height;
                    highestPointPosition = new Vector2(i, j);
                    highestPointTile = gameGenerator.tiles[i][j];
                }
            }
        }

        gameGenerator.FlattenAround(highestPointPosition);

        Vector3 position = highestPointTile.GetComponent<TileBehaviour>().placement.transform.position;
        mainVillage = GameObject.Instantiate(
            mainVillagePrefab,
            new Vector3(0f, 0f, 0f),
            Quaternion.identity
        );
        mainVillage.transform.localScale *= GameManager.instance.towerSize;
        mainVillage.transform.position = new Vector3(
            position.x,
            position.y + mainVillage.transform.localScale.y / 2f,
            position.z
        );

        mainVillage.name = "Main Village";
        mainVillage.transform.parent = villageParent.transform;
        mainVillage.GetComponent<VillageBehaviour>().position = highestPointPosition;
        highestPointTile.GetComponent<TileBehaviour>().structure = mainVillage;

        float cameraHeight = (mainVillage.transform.position.y + mainVillage.GetComponent<MeshRenderer>().bounds.size.y
            / GameManager.instance.towerSize + gameGenerator.terrainGenerator.stepHeight * 10f); // *2f
        gameGenerator.mainCamera.GetComponent<CameraManager>().SetHeight(cameraHeight);

        // place 4 towers around the main village, randomly
        float maxDistanceFromCenter = Mathf.Round(gameGenerator.terrainGenerator.size.x / 4f);
        float minDistanceFromMainTower = Mathf.Round(gameGenerator.terrainGenerator.size.x / 8f);
        Vector2 center = new Vector2(gameGenerator.terrainGenerator.size.x / 2f, gameGenerator.terrainGenerator.size.y / 2f);
        List<Vector2> villagePositions = new List<Vector2>();

        for (int i=0; i < 4; i++)
        {
            Vector2 randomPosition = new Vector2();
            while (villagePositions.Contains(randomPosition) || randomPosition == Vector2.zero
                || (randomPosition - highestPointPosition).magnitude < minDistanceFromMainTower)
            {
                randomPosition.x = gameGenerator.randomWithSeed.Next(
                    (int)center.x - (int)maxDistanceFromCenter,
                    (int)center.x + (int)maxDistanceFromCenter
                );
                randomPosition.y = gameGenerator.randomWithSeed.Next(
                    (int)center.y - (int)maxDistanceFromCenter,
                    (int)center.y + (int)maxDistanceFromCenter
                );
            }
            // gameGenerator.FlattenAround(randomPosition, 1, false);
            villagePositions.Add(randomPosition);

            GameObject villageStructurePrefab = villagePrefabs[gameGenerator.randomWithSeed.Next(0, villagePrefabs.Count)];
            GameObject tile = gameGenerator.tiles[(int)randomPosition.x][(int)randomPosition.y];
            position = tile.GetComponent<TileBehaviour>().placement.transform.position;
            GameObject villageStructure = GameObject.Instantiate(
                villageStructurePrefab,
                new Vector3(0f, 0f, 0f),
                Quaternion.identity
            );
            
            villageStructure.transform.localScale *= GameManager.instance.towerSize;
            villageStructure.transform.position = new Vector3(
                position.x, // - tile.GetComponent<MeshRenderer>().bounds.size.x / 2f,
                position.y + villageStructure.transform.localScale.y / 2f,
                position.z // - tile.GetComponent<MeshRenderer>().bounds.size.z / 2f
            );

            villageStructure.name = $"Village Structure {i}";
            villageStructure.transform.parent = villageParent.transform;
            villageStructure.GetComponent<VillageBehaviour>().position = randomPosition;
            villageBuildings.Add(villageStructure);
        }
    }
}
