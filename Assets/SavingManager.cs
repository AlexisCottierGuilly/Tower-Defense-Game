using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavingManager : MonoBehaviour
{
    public GameGenerator gameGenerator;

    private GameSave gameSave;

    public void SaveGame()
    {
        GameManager.instance.player.gameSaves.RemoveAll(x => x.saveName == GameManager.instance.gameName);

        GameSave gameSave = new GameSave();

        gameSave.saveName = GameManager.instance.gameName;
        gameSave.seed = (int)gameGenerator.seed;
        gameSave.gold = GameManager.instance.gold;
        gameSave.wave = gameGenerator.waveManager.wave;

        gameSave.mapDifficulty = GameManager.instance.mapDifficulty;
        gameSave.mapSize = GameManager.instance.mapSize;

        gameSave.gameTime = gameGenerator.gameTime;
        gameSave.lastOpenedTime = (float)(System.DateTime.Now - System.DateTime.MinValue).TotalSeconds;

        gameSave.randomWithSeed = CreateCopy(gameGenerator.randomWithSeed);
        gameSave.waveRandomWithSeed = CreateCopy(gameGenerator.waveRandomWithSeed);

        foreach (GameObject tower in gameGenerator.towers)
        {
            if (tower == null)
                continue;

            TowerPlacement towerPlacement = new TowerPlacement();
            TowerBehaviour towerBehaviour = tower.GetComponent<TowerBehaviour>();

            towerPlacement.towerType = GetTowerType(towerBehaviour);
            towerPlacement.position = towerBehaviour.position;
            towerPlacement.targetType = towerBehaviour.targetType;

            gameSave.towerPlacements.Add(towerPlacement);
        }

        foreach (GameObject village in gameGenerator.villageGenerator.villageBuildings)
        {
            if (village == null)
                continue;
            
            VillagePlacement villagePlacement = new VillagePlacement();
            VillageBehaviour villageBehaviour = village.GetComponent<VillageBehaviour>();

            villagePlacement.position = villageBehaviour.position;
            villagePlacement.health = (int)villageBehaviour.health;

            gameSave.villagePlacements.Add(villagePlacement);
        }

        VillagePlacement mainVillagePlacement = new VillagePlacement();
        VillageBehaviour mainVillageBehaviour = gameGenerator.villageGenerator.mainVillage.GetComponent<VillageBehaviour>();

        mainVillagePlacement.position = mainVillageBehaviour.position;
        mainVillagePlacement.health = (int)mainVillageBehaviour.health;

        gameSave.mainVillagePlacement = mainVillagePlacement;

        // save the decorations
        foreach (GameObject decoration in gameGenerator.decorations)
        {
            if (decoration == null)
                continue;

            DecorationBehaviour decorationBehaviour = decoration.GetComponent<DecorationBehaviour>();
            gameSave.acceptedDecorations.Add(decorationBehaviour.position);
        }

        GameManager.instance.player.gameSaves.Add(gameSave);
    }

    TowerType GetTowerType(TowerBehaviour towerBehaviour)
    {
        foreach (TowerPrefab towerPrefab in GameManager.instance.towerPrefabs)
        {
            if (towerPrefab.data == towerBehaviour.data)
                return towerPrefab.tower;
        }

        return TowerType.None;
    }

    System.Random CreateCopy(System.Random rng)
    {
        System.Random copy = new System.Random();
        System.Reflection.FieldInfo[] fields = typeof(System.Random).GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        foreach (System.Reflection.FieldInfo field in fields)
        {
            if (field.Name == "SeedArray")
            {
                int[] seedArray = (int[])field.GetValue(rng);
                int[] seedArrayCopy = new int[seedArray.Length];
                seedArray.CopyTo(seedArrayCopy, 0);
                field.SetValue(copy, seedArrayCopy);
            }
            else
            {
                field.SetValue(copy, field.GetValue(rng));
            }
        }

        return copy;
    }

    public void LoadGameSettings()
    {
        gameSave = null;
        foreach (GameSave save in GameManager.instance.player.gameSaves)
        {
            if (save.saveName == GameManager.instance.gameName)
            {
                gameSave = save;
                break;
            }
        }

        if (gameSave != null)
        {
            GameManager.instance.gameSeed = gameSave.seed;
            gameGenerator.seed = gameSave.seed;

            GameManager.instance.gold = gameSave.gold;

            gameGenerator.waveManager.wave = gameSave.wave;
            gameGenerator.waveManager.waveText.text = $"Vague {gameSave.wave}/{gameGenerator.waveManager.waves.Count}";
            gameGenerator.waveManager.didCallWaveFinished = true;

            gameGenerator.gameTime = gameSave.gameTime;

            // Error : the randoms are not being saved correctly (they are null when loaded)
            //gameGenerator.randomWithSeed = gameSave.randomWithSeed;
            //gameGenerator.waveRandomWithSeed = gameSave.waveRandomWithSeed;

            // temporary
            gameGenerator.randomWithSeed = new System.Random((int)gameSave.seed);
            gameGenerator.waveRandomWithSeed = new System.Random((int)gameSave.seed);

            GameManager.instance.mapSize = gameSave.mapSize;
            GameManager.instance.mapDifficulty = gameSave.mapDifficulty;
        }
    }

    public void LoadGameBuildings()
    {
        LoadTowers(gameSave);
        LoadVillages(gameSave);
        LoadMainVillage(gameSave);
        LoadDeletedDecorations(gameSave);
    }

    void LoadTowers(GameSave gameSave)
    {
        // create all the towers and remove the one on the main village if needed
    }

    void LoadVillages(GameSave gameSave)
    {
        // more complicated because village buildings could be destroyed and placed
    }

    void LoadMainVillage(GameSave gameSave)
    {
        GameObject mainVillage = gameGenerator.villageGenerator.mainVillage;
        VillageBehaviour mainVillageBehaviour = mainVillage.GetComponent<VillageBehaviour>();

        mainVillageBehaviour.health = gameSave.mainVillagePlacement.health;
    }

    void LoadDeletedDecorations(GameSave gameSave)
    {
        List<GameObject> toRemove = new List<GameObject>();

        foreach (GameObject decoration in gameGenerator.decorations)
        {
            if (decoration == null)
                continue;

            DecorationBehaviour decorationBehaviour = decoration.GetComponent<DecorationBehaviour>();

            if (!gameSave.acceptedDecorations.Contains(decorationBehaviour.position))
            {
                toRemove.Add(decoration);
            }
        }

        foreach (GameObject decoration in toRemove)
        {
            gameGenerator.decorations.Remove(decoration);
            Destroy(decoration);
        }
    }
}
