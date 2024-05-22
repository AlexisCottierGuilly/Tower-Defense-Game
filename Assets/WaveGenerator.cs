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
    public float batchAugmentationPercentage = 1f;

    [HideInInspector] public Vector2Int modifiedBatchRange;
}


public class WaveGenerator : MonoBehaviour
{
    public List<MonsterCost> monsterCosts = new List<MonsterCost>();
    public float batchSizeMultiplier = 1f;

    void Start()
    {
        UpdateMonsterCostBatchRange();
    }
    
    public WaveData GetRandomWave(int waveNumber, bool log=true)
    {
        System.Random rndSeed = GameManager.instance.generator.waveRandomWithSeed;

        int money = 100 + (int)Mathf.Round(Mathf.Pow(waveNumber, 1.25f) * 7.5f);
        batchSizeMultiplier = 1 + (waveNumber / 15f);

        UpdateMonsterCostBatchRange();

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

            int batch = rndSeed.Next(monsterCost.modifiedBatchRange.x, monsterCost.modifiedBatchRange.y + 1);
            money -= monsterCost.cost * batch;

            WavePart wavePart = new WavePart();
            wavePart.monster = monster;
            wavePart.amount = batch;
            wavePart.interval = (float)rndSeed.Next(1, 5) / 3f;

            wave.waveParts.Add(wavePart);
        }

        if (log)
            LogWave(wave, waveNumber: waveNumber);

        return wave;
    }

    private void UpdateMonsterCostBatchRange()
    {
        foreach (MonsterCost monsterCost in monsterCosts)
        {
            monsterCost.modifiedBatchRange = new Vector2Int(
                (int)Mathf.Round(monsterCost.batchRange.x * batchSizeMultiplier * monsterCost.batchAugmentationPercentage),
                (int)Mathf.Round(monsterCost.batchRange.y * batchSizeMultiplier * monsterCost.batchAugmentationPercentage)
            );

            monsterCost.modifiedBatchRange = new Vector2Int(
                Mathf.Max(1, monsterCost.modifiedBatchRange.x),
                Mathf.Max(1, monsterCost.modifiedBatchRange.y)
            );
        }
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
            if (monsterCost.cost * monsterCost.modifiedBatchRange.y <= money)
            {
                nextMonsterCost = monsterCost;

                break;
            }
        }

        if (nextMonsterCost == null)
        {
            foreach (MonsterCost monsterCost in shuffledMonsterCosts)
            {
                if (monsterCost.cost * monsterCost.modifiedBatchRange.x <= money)
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
                minCost = monsterCost.cost * (int)Mathf.Round((monsterCost.modifiedBatchRange.x + monsterCost.modifiedBatchRange.y) / 2f);
            }
        }

        return minCost;
    }

    private void LogWave(WaveData wave, int waveNumber = -1)
    {
        string waveNumberString = waveNumber == -1 ? "" : " " + waveNumber.ToString();
        
        string s = $"Wave{waveNumberString}: \n";

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
