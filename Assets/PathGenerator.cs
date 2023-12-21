using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathGenerator : MonoBehaviour
{
    public GameGenerator gameGenerator;
    
    [Header("Settings")]
    public List<List<Vector2>> paths = new List<List<Vector2>>();
    public float stepHeight = 2f;

    [Header("3D Models")]
    public GameObject pathTilePrefab;

    public void Generate()
    {
        GeneratePaths();

        // now change the model of the tiles that are paths to pathTilePrefab
        for (int i=0; i < paths.Count; i++)
        {
            for (int j=0; j < paths[i].Count; j++)
            {
                Vector2 position = paths[i][j];
                GameObject tile = gameGenerator.tiles[(int)position.x][(int)position.y];
                GameObject new_tile = GameObject.Instantiate(pathTilePrefab, tile.transform.position, Quaternion.identity);
                new_tile.name = tile.name;
                new_tile.transform.parent = tile.transform.parent;
                new_tile.transform.localScale = tile.transform.localScale;
                new_tile.transform.rotation = tile.transform.rotation;

                TileBehaviour behaviour = new_tile.GetComponent<TileBehaviour>();
                behaviour.position = position;
                behaviour.type = TileType.Path;

                DestroyImmediate(tile);
                gameGenerator.tiles[(int)position.x][(int)position.y] = new_tile;
            }
        }
    }

    public void GeneratePathStarts()
    {
        /*
        The paths start on the sides of the map, at the lowest spots.
        Store the start paths in the paths list (first element is the start)
        */
        paths = new List<List<Vector2>>();
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
                for (int j=0; j < gameGenerator.terrainGenerator.size.y; j++)
                   allSidePositions.Add(new Vector2(0f, j));
            }
            else if (i == 1)
            {
                // top side
                for (int j=0; j < gameGenerator.terrainGenerator.size.x; j++)
                    allSidePositions.Add(new Vector2(j, 0f));
            }
            else if (i == 2)
            {
                // right side
                for (int j=0; j < gameGenerator.terrainGenerator.size.y; j++)
                    allSidePositions.Add(new Vector2(gameGenerator.terrainGenerator.size.x - 1f, j));
            }
            else if (i == 3)
            {
                // bottom side
                for (int j=0; j < gameGenerator.terrainGenerator.size.x; j++)
                   allSidePositions.Add(new Vector2(j, gameGenerator.terrainGenerator.size.y - 1f));
            }

            foreach (Vector2 position in allSidePositions)
            {
                float height = gameGenerator.tiles[(int)position.x][(int)position.y].transform.position.y;
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

    public void GeneratePaths()
    {
        // the start is already in the paths list (first element is the start)
        // the end is the block where is the main tower and the paths should avoid getting under village buildings
        GeneratePathStarts();

        foreach (List<Vector2> path in paths)
        {
            Vector2 start = path[0];
            Vector2 end = gameGenerator.villageGenerator.mainVillage.GetComponent<VillageBehaviour>().position;

            Vector2 current = start;
            Vector2 previous = start;
            Vector2 next = new Vector2();
            List<Vector2> usedTiles = new List<Vector2>();
            List<Vector2> villages = gameGenerator.GetVillageTiles();

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

    public float AverageDistanceFromVillage(Vector2 position)
    {
        float distance = 0f;
        foreach (GameObject building in gameGenerator.villageGenerator.villageBuildings)
        {
            Vector2 buildingPosition = building.GetComponent<VillageBehaviour>().position;
            distance += Vector2.Distance(position, buildingPosition);
        }

        return distance / (float)(gameGenerator.villageGenerator.villageBuildings.Count);
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
        GameObject currentTile = gameGenerator.tiles[(int)current.x][(int)current.y];
        float stepHeightValue = gameGenerator.terrainGenerator.stepHeight * stepHeight;

        foreach (Vector2 tile in tilesAround)
        {   
            GameObject otherTile = gameGenerator.tiles[(int)tile.x][(int)tile.y];
            float heightDistance = Mathf.Abs(otherTile.transform.position.y - currentTile.transform.position.y);
            if (heightDistance > stepHeightValue && heightDistance - stepHeightValue
                <= gameGenerator.terrainGenerator.stepHeight)
            {
                float diff = gameGenerator.terrainGenerator.stepHeight;
                //if (heightDistance - stepHeightValue > gameGenerator.terrainGenerator.stepHeight)
                //    diff *= 2f;
                otherTile.transform.position = new Vector3(
                    otherTile.transform.position.x,
                    otherTile.transform.position.y - diff,
                    otherTile.transform.position.z
                );
            }
            heightDistance = Mathf.Abs(otherTile.transform.position.y - currentTile.transform.position.y);

            if (heightDistance > stepHeightValue || usedTiles.Contains(tile) ||
                villages.Contains(tile))
                continue;
            
            float distance = Vector2.Distance(tile, end); /// 1.25f + AverageDistanceFromVillage(tile);
            if (bestDistance == -1f || distance < bestDistance || (distance <= bestDistance &&
                heightDistance < bestStep && gameGenerator.randomWithSeed.NextDouble() > 0.25f))
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

        float random = (float)gameGenerator.randomWithSeed.NextDouble();
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
            if (tile.x >= 0 && tile.x < gameGenerator.terrainGenerator.size.x && tile.y >= 0 && tile.y < gameGenerator.terrainGenerator.size.y)
                tilesAround.Add(tile);
        }

        return tilesAround;
    }
}
