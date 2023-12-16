using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Monster
{
    Goblin,
    Troll
}

public class WaveManager : MonoBehaviour
{
    public GameGenerator gameGenerator;

    [Header("Settings")]
    public int wave = 0;
    public List<WaveData> waves = new List<WaveData>();
    public bool waveFinished = false;

    [HideInInspector] List<GameObject> monsters = new List<GameObject>();
    
    IEnumerator LoadNextRound()
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
        Debug.Log($"Spawned monster {type.ToString()}.");
    }
    
    void Start()
    {
        StartCoroutine(LoadNextRound());
    }

    void Update()
    {
        
    }
}
