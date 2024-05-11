using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathGenerator2 : MonoBehaviour
{
    public GameGenerator gameGenerator;
    
    [Header("Settings")]
    public List<List<Vector2>> paths = new List<List<Vector2>>();
    public float stepHeight = 2f;
    public int maxAcceptedStepHeight = 2;

    [Header("3D Models")]
    public GameObject pathTilePrefab;

    public void Generate()
    {
        GeneratePaths();

        return; // IT WILL NOT REPLACE THE TILES WITH PATH TILES

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

                DestroyImmediate(tile);  // ADD THIS LINE LATER
                gameGenerator.tiles[(int)position.x][(int)position.y] = new_tile;
            }
        }
    }

    public List<Vector2> CleanPath(List<Vector2> path)
    {
        /*
        The paths can be really messy
        They can travel using tiles that are connected and take long roads to get to the center
        This function will clean the path by removing the tiles that are not necessary,
        taking the most direct path to the end from the center
        */

        if (path.Count == 0)
            return path;
        
        List<Vector2> cleanedPath = new List<Vector2>();
        cleanedPath.Add(path[0]);

        int c = 0;
        for (int i=1; i < path.Count; i++)
        {
            // check the neighbours of the current and
            // see if one of its neighbours is not
            // the last or the next tile in the path

            // if so, remove all the tiles in between the current and the neighbour

            Vector2 current = path[i];
            Vector2 last = cleanedPath[cleanedPath.Count - 1];

            Vector2 next = new Vector2(-1, -1);

            if (i < path.Count - 1)
                next = path[i + 1];

            cleanedPath.Add(current);
            Debug.Log($"Checking tile {current}.", gameGenerator.tiles[(int)current.x][(int)current.y]);

            List<Vector2> neighbours = GetNeighbours(current);
            Vector2 neighbour = new Vector2(-1, -1);

            foreach (Vector2 n in neighbours)
            {
                if (n == last || n == next)
                    continue;

                if (cleanedPath.Contains(n))
                {
                    neighbour = n;
                    Debug.Log($"Found neighbour {n}.", gameGenerator.tiles[(int)n.x][(int)n.y]);
                    break;
                }
            }

            if (neighbour.x != -1 && neighbour.y != -1)
            {
                int m = cleanedPath.Count - 1;
                
                int index = cleanedPath.IndexOf(neighbour);
                Debug.Log($"Removing tiles from {index} to {m}.");
                for (int j=0; j <= (m - index); j++)
                {
                    cleanedPath.RemoveAt(cleanedPath.Count - 2);
                }
            }

            Debug.Log($"Cleaned path length : {cleanedPath.Count}.");

            c++;

            if (c > 5000)
            {
                Debug.LogError("Infinite loop in CleanPath.");
                break;
            }
        }

        Debug.Log($"Final cleaned path length : {cleanedPath.Count}.");
        return cleanedPath;
    }
    
    public void GeneratePaths()
    {
        /*
        The algorithm is simple:

        1. Start at the main village tile (defined from
            gameGenerator.villagGenerator.mainVillage.GetComponent<VillageBehaviour>().position)
        
        2. Generate the ends of the paths (one end for each side - 4 sides = 4 ends)

        3. Using a non-recursive depth-first search, generate the path from the main village to the end of the path
            3.1. There is a maximum step height that the path can have. The path can carve and extrude the terrain (but
                not too much). The step height is defined by the stepHeight variable and the stepHeightModifier variable
                defines the maximum carving/extruding height.
            3.2. If the path is blocked at one point, and it can't carve/extrude enough, it will go back to the last
                tile it went. If it goes back to the main village, it will increment the stepHeightModifier and try again.
        
        4. Repeat the process for each end of the path
        */

        // Setup the initial variables

        List<Vector2> ends = GeneratePathEnds();
        paths = new List<List<Vector2>>();

        // TEMPORARY: NO PATHS GENERATED
        foreach (Vector2 end in ends)
        {
            List<Vector2> path = new List<Vector2>();
            path.Add(end);
            paths.Add(path);
        }

        return;

        foreach (Vector2 end in ends)
        {
            Vector2 start = gameGenerator.villageGenerator.mainVillage.GetComponent<VillageBehaviour>().position;
            Vector2 current = start;

            List<Vector2> visited = new List<Vector2>();
            List<Vector2> path = new List<Vector2>();

            float targetHeight = gameGenerator.tiles[(int)end.x][(int)end.y].transform.position.y;

            int acceptedStepHeight = 1;

            /*
            Used custom functions useful for the search
            1. GetNeighbours(Vector2 position) - returns a list of the neighbours of the position
            2. GetNextNeighbour(Vector2 current, Vector2 end, List<Vector2> visited, float pathStepModifier)
                - returns the next neighbour to go to (null if not possible)
            */

            int i = 0;
            bool hasBeenBlocked = false;
            while (current != end)
            {
                if (!hasBeenBlocked)
                {
                    path.Add(current);
                    visited.Add(current);
                }
                hasBeenBlocked = false;

                Vector2 next = GetNextNeighbour(current, end, visited, acceptedStepHeight, targetHeight);
                if (next.x == -1 && next.y == -1)
                {
                    path.RemoveAt(path.Count - 1);

                    if (path.Count == 0)
                    {
                        acceptedStepHeight++;
                        current = start;
                        visited = new List<Vector2>();

                        if (acceptedStepHeight > maxAcceptedStepHeight)
                        {
                            break;
                        }
                    }
                    else
                    {
                        current = path[path.Count - 1];
                        hasBeenBlocked = true;
                    }
                }
                else
                {
                    current = next;
                }

                i++;
                if (i > 5000)
                {
                    break;
                }
            }
            
            if (current == end)
                path.Add(current);
            else
                Debug.LogError("Path not found.");

            // path = CleanPath(path);
            paths.Add(path);
        }
    }

    public List<Vector2> GeneratePathEnds()
    {
        /*
        The ends of the paths are the lowest points at the extremities of the terrain.

        The terrain is a square, so ends are in :
        1. the top row
        2. the right column
        3. the bottom row
        4. the left column

        The ends can't be the same for two paths, so we need to 
        get the lowest points, excluding the corners.
        */

        List<Vector2> ends = new List<Vector2>();

        List<List<Vector2>> endsList = new List<List<Vector2>>();

        int cornerCut = 5;

        // add top row without corners
        List<Vector2> topRow = new List<Vector2>();
        for (int x = cornerCut; x < gameGenerator.terrainGenerator.size.x - cornerCut; x++)
            topRow.Add(new Vector2(x, gameGenerator.terrainGenerator.size.y - 1));
        endsList.Add(topRow);

        // add right column without corners
        List<Vector2> rightColumn = new List<Vector2>();
        for (int y = cornerCut; y < gameGenerator.terrainGenerator.size.y - cornerCut; y++)
            rightColumn.Add(new Vector2(gameGenerator.terrainGenerator.size.x - 1, y));
        endsList.Add(rightColumn);

        // add bottom row without corners
        List<Vector2> bottomRow = new List<Vector2>();
        for (int x = cornerCut; x < gameGenerator.terrainGenerator.size.x - cornerCut; x++)
            bottomRow.Add(new Vector2(x, 0));
        endsList.Add(bottomRow);

        // add left column without corners
        List<Vector2> leftColumn = new List<Vector2>();
        for (int y = cornerCut; y < gameGenerator.terrainGenerator.size.y - cornerCut; y++)
            leftColumn.Add(new Vector2(0, y));
        endsList.Add(leftColumn);

        // get the lowest points
        foreach (List<Vector2> endList in endsList)
        {
            Vector2 lowest = new Vector2(0, 0);
            float lowestHeight = float.MaxValue;

            foreach (Vector2 end in endList)
            {
                float height = gameGenerator.tiles[(int)end.x][(int)end.y].transform.position.y;
                if (height < lowestHeight)
                {
                    lowestHeight = height;
                    lowest = end;
                }
            }

            ends.Add(lowest);
        }

        return ends;
    }

    Vector2 GetNextNeighbour(Vector2 current, Vector2 end, List<Vector2> visited, int acceptedStepHeight, float targetHeight)
    {
        float realStepHeight = acceptedStepHeight * gameGenerator.terrainGenerator.stepHeight;
        
        List<Vector2> neighbours = GetNeighbours(current);
        List<Vector2> possibleNeighbours = new List<Vector2>();
        float lastTileHeight = gameGenerator.tiles[(int)current.x][(int)current.y].transform.position.y;

        foreach (Vector2 neighbour in neighbours)
        {
            if (!visited.Contains(neighbour))
                possibleNeighbours.Add(neighbour);
        }

        if (possibleNeighbours.Count == 0)
            return new Vector2(-1, -1);

        /*
        Now we need to get the neighbour that is (importance in parentheses) :
        1. closest to the end (75% importance)
        2. closest to targetHeigh (25% importance)
        3. not too high or too low (stepHeight)
            3.1. the height of each tile can be modified by +- pathStepModifier
            (least important, but if there is no other option, it will be used)
        */

        List<Vector2> neightboursByScore = new List<Vector2>();
        List<float> scores = new List<float>();
        List<float> heights = new List<float>();

        foreach (Vector2 neighbour in possibleNeighbours)
        {
            float height = gameGenerator.tiles[(int)neighbour.x][(int)neighbour.y].transform.position.y;
            float distanceToEnd = Vector2.Distance(neighbour, end);
            float distanceToTarget = height - lastTileHeight;

            float heightDifference = distanceToTarget;
            distanceToTarget = Mathf.Abs(distanceToTarget);
            
            // distanceToEnd and distanceToTarget have to be normalized (0-1)
            distanceToEnd /= Vector2.Distance(Vector2.zero, new Vector2(gameGenerator.terrainGenerator.size.x / 2f, gameGenerator.terrainGenerator.size.y / 2f));
            distanceToTarget /= gameGenerator.villageGenerator.mainVillage.transform.position.y;
            
            float score = distanceToEnd * 1f + distanceToTarget * 0f;

            neightboursByScore.Add(neighbour);
            scores.Add(score);
            heights.Add(heightDifference);
        }

        // order the neighbours by score
        for (int i = 0; i < scores.Count; i++)
        {
            for (int j = i + 1; j < scores.Count; j++)
            {
                if (scores[j] < scores[i])
                {
                    float tempScore = scores[i];
                    scores[i] = scores[j];
                    scores[j] = tempScore;

                    Vector2 tempNeighbour = neightboursByScore[i];
                    neightboursByScore[i] = neightboursByScore[j];
                    neightboursByScore[j] = tempNeighbour;

                    float tempHeight = heights[i];
                    heights[i] = heights[j];
                    heights[j] = tempHeight;
                }
            }
        }

        // check the heights to ignore the neighbours that are too high or too low (stepHeight)
        // if possible, modify the tile' gameobject height using pathStepModifier

        Vector2 next = new Vector2(-1, -1);
        for (int i = 0; i < neightboursByScore.Count; i++)
        {
            if (Mathf.Abs(heights[i]) <= realStepHeight)
            {
                next = neightboursByScore[i];
                break;
            }
            /*
            else
            {
                // MODIFYING THE HEIGHT IS NOT WORKING PROPERLY

                // mandatory : modify the height so it's abs is <= stepHeight
                // try to get the height of the tile closest to the targetHeight, by adding or subtracting pathStepModifier

                // the height difference if heights[i] (positive or negative)

                GameObject tile = gameGenerator.tiles[(int)neightboursByScore[i].x][(int)neightboursByScore[i].y];
                
                int augmentation = 1;
                float heightDifference = heights[i];

                if (heightDifference < 0)
                    augmentation = -1;

                
                // the height is too low
                // try to add a fraction of pathStepModifier (but an int) that makes the tile's height closer to targetHeight

                float currentHeight = tile.transform.position.y;
                // for all fractions (int) of pathStepModifier, test the distance to targetHeight (if it is in stepHeight bounds)
                float bestHeight = currentHeight;
                float bestDistance = Mathf.Abs(currentHeight - targetHeight);

                float bestNewHeight = currentHeight;
                float bestNewDifference = heightDifference;

                int pathStepModifier = 1;

                for (int j = 1; j <= pathStepModifier; j++)
                {
                    float newHeight = currentHeight + j * (-augmentation) * gameGenerator.terrainGenerator.stepHeight;
                    float newDistance = Mathf.Abs(newHeight - targetHeight);
                    float newHeightDifference = newHeight - lastTileHeight;

                    if (newDistance < bestDistance && Mathf.Abs(newHeightDifference) <= realStepHeight)
                    {
                        bestHeight = newHeight;
                        bestDistance = newDistance;
                        bestNewHeight = newHeight;
                        bestNewDifference = newHeightDifference;
                    }
                }

                heights[i] = bestNewDifference;

                //if (bestNewHeight != currentHeight)
                //    Debug.Log($"Modifying height of tile {tile.name} from {currentHeight} to {bestNewHeight}.", tile);

                tile.transform.position = new Vector3(
                    tile.transform.position.x,
                    bestNewHeight,
                    tile.transform.position.z
                );

                if (Mathf.Abs(heights[i]) <= realStepHeight)
                {
                    next = neightboursByScore[i];
                    break;
                }
            }*/
        }

        return next;
    }

    List<Vector2> GetNeighbours(Vector2 position)
    {
        List<Vector2> tilesAround = new List<Vector2>();
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
