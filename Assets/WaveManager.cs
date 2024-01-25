using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;
using UnityEngine.AI;

public enum Monster
{
    Goblin,
    Troll
}

public class WaveManager : MonoBehaviour
{
    public GameGenerator gameGenerator;

    [Header("Prefabs")]
    public List<NavMeshSurface> surfaces = new List<NavMeshSurface>();

    [Header("Settings")]
    public int wave = 0;
    public List<WaveData> waves = new List<WaveData>();
    public bool waveFinished = true;
    
    [HideInInspector] int currentPathIndex = 0;
    [HideInInspector] public List<GameObject> monsters = new List<GameObject>();
    
    public void InitializeSurfaces()
    {
        foreach (NavMeshSurface surface in surfaces)
        {
            surface.BuildNavMesh();
        }
    }
    
    public IEnumerator LoadNextRound()
    {
        if (wave >= waves.Count)
            yield return new WaitForSeconds(0f);
        
        wave++;
        WaveData currentWaveData = waves[wave - 1];
        Debug.Log($"Loading round {wave}.");
        foreach (WavePart part in currentWaveData.waveParts)
        {
            StartCoroutine(SpawnMonsters(part.monster, part.amount, part.interval));
            yield return new WaitForSeconds((float)part.amount * part.interval);
        }
    }

    IEnumerator SpawnMonsters(Monster type, int repetition, float interval)
    {
        for (int i = 0; i < repetition; i++)
        {
            SpawnMonster(type);
            yield return new WaitForSeconds(interval);
        }
    }

    void SpawnMonster(Monster type)
    {
        GameObject monster = Instantiate(gameGenerator.GetMonsterPrefab(type));

        currentPathIndex++;
        if (currentPathIndex >= gameGenerator.pathGenerator.paths.Count)
            currentPathIndex = 0;

        Vector2 tilePosition = gameGenerator.pathGenerator.paths[currentPathIndex][0];
        GameObject placement = gameGenerator.tiles[(int)tilePosition.x][(int)tilePosition.y].GetComponent<TileBehaviour>().placement;
        MonsterBehaviour behaviour = monster.GetComponent<MonsterBehaviour>();
        
        behaviour.gameGenerator = gameGenerator;
        monster.transform.position = new Vector3(
            placement.transform.position.x,
            placement.transform.position.y + monster.transform.localScale.y / 2f,
            placement.transform.position.z
        );
        monster.transform.localScale = behaviour.finalScale;
        behaviour.agent.destination = gameGenerator.villageGenerator.mainVillage.transform.position;
        
        monsters.Add(monster);
    }

    public bool WaveIsFinished()
    {
        if (monsters.Count == 0)
            return true;
        return false;
    }

    void Update()
    {
        waveFinished = WaveIsFinished();
        
        if (waveFinished && Time.time > 5f)
        {
            StartCoroutine(LoadNextRound());
            waveFinished = false;
        }
    }
}
