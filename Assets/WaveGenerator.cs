using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class MonsterCost
{
    public Monster monster;
    [Header("Cost is Per Monster")]
    public int cost;
    public Vector2Int batchRange;
}


public class WaveGenerator : MonoBehaviour
{
    public List<MonsterCost> monsterCosts = new List<MonsterCost>();

    void Start()
    {

    }
    
    public WaveData GetRandomWave(int waveNumber)
    {
        waveNumber += 1000;
        
        System.Random rndSeed = GameManager.instance.generator.waveRandomWithSeed;

        int money = 75 + waveNumber * 15;

        WaveData wave = new WaveData();
        wave.waveParts = new List<WavePart>();
        wave.hideBossBar = true;

        /*
        The Plan
        1. Calculate the minimum cost of a batch of monsters
        While the min cost of batch is lower (or equal) than the money:
            a) Randomly select a monster
            b) Randomly select a batch size (with rndSeed)
            c) Remove the cost of the batch from the money
            d) Add the monster and batch size to the wave
            e) Add the wave to the list of waves
        
        return the waves
        */

        int minBatchCost = GetMinBatchCost();

        while (minBatchCost <= money)
        {
            MonsterCost monsterCost = GetNextMonsterCost(money, rndSeed);
            Monster monster = monsterCost.monster;
            
            int batch = rndSeed.Next(monsterCost.batchRange.x, monsterCost.batchRange.y + 1);
            money -= monsterCost.cost * batch;

            WavePart wavePart = new WavePart();
            wavePart.monster = monster;
            wavePart.amount = batch;
            wavePart.interval = (float)rndSeed.Next(1, 5) / 3f;

            wave.waveParts.Add(wavePart);
        }

        LogWave(wave);

        return wave;
    }

    private MonsterCost GetNextMonsterCost(int money, System.Random rnd)
    {
        // try to get a monster cost that is (at maximum) is less than the money
        // if not possible, take one that has a minimum less than the money
        // if not possible, take the one with the minimum cost

        MonsterCost nextMonsterCost = null;
        List<MonsterCost> shuffledMonsterCosts = ShuffleMonsterCosts(monsterCosts, rnd);

        foreach (MonsterCost monsterCost in shuffledMonsterCosts)
        {
            if (monsterCost.cost * monsterCost.batchRange.y <= money)
            {
                nextMonsterCost = monsterCost;

                break;
            }
        }

        if (nextMonsterCost == null)
        {
            foreach (MonsterCost monsterCost in shuffledMonsterCosts)
            {
                if (monsterCost.cost * monsterCost.batchRange.x <= money)
                {
                    nextMonsterCost = monsterCost;
                    break;
                }
            }

            if (nextMonsterCost == null)
            {
                int minCost = int.MaxValue;
                foreach (MonsterCost monsterCost in shuffledMonsterCosts)
                {
                    if (monsterCost.cost < minCost)
                    {
                        minCost = monsterCost.cost;
                        nextMonsterCost = monsterCost;
                    }
                }
            }
        }

        return nextMonsterCost;
    }

    private MonsterCost GetMonsterCost(Monster monster)
    {
        foreach (MonsterCost monsterCost in monsterCosts)
        {
            if (monsterCost.monster == monster)
            {
                return monsterCost;
            }
        }
        return null;
    }

    private int GetMinBatchCost()
    {
        int minCost = int.MaxValue;
        
        foreach (MonsterCost monsterCost in monsterCosts)
        {
            if (monsterCost.cost < minCost)
            {
                minCost = monsterCost.cost * (int)Mathf.Round((monsterCost.batchRange.x + monsterCost.batchRange.y) / 2f);
            }
        }

        return minCost;
    }

    private void LogWave(WaveData wave)
    {
        string s = "Wave: \n";

        foreach (WavePart part in wave.waveParts)
        {
            s += $"{part.monster} x{part.amount} (each {part.interval} s)" + "\n";
        }

        Debug.Log(s);
    }

    private List<MonsterCost> ShuffleMonsterCosts(List<MonsterCost> costs, System.Random rnd)
    {
        List<MonsterCost> shuffled = new List<MonsterCost>(costs);
        int n = shuffled.Count;
        while (n > 1)
        {
            n--;
            int k = rnd.Next(n + 1);
            MonsterCost value = shuffled[k];
            shuffled[k] = shuffled[n];
            shuffled[n] = value;
        }

        return shuffled;
    }
}
