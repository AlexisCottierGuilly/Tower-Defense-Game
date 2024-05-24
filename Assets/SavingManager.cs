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

        System.DateTime epochStart = new System.DateTime(2024, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
        gameSave.lastOpenedTime = (float)(System.DateTime.Now - epochStart).TotalSeconds;

        gameSave.didWinGame = gameGenerator.didWinGame;
        gameSave.infiniteMode = gameGenerator.waveManager.infiniteMode;
        
        DifficultyModifier difficulty = GameManager.instance.GetDifficultyModifier();
        gameSave.defeated = (gameGenerator.health <= 0 ||
            gameGenerator.villageGenerator.mainVillage == null ||
            (gameGenerator.lostLives > 0 && !difficulty.canLoseLives)) ||
                gameGenerator.forceDefeat;

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
            towerPlacement.stats = towerBehaviour.stats;

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

        GameObject mainVillage = gameGenerator.villageGenerator.mainVillage;

        if (mainVillage != null)
        {
            VillageBehaviour mainVillageBehaviour = gameGenerator.villageGenerator.mainVillage.GetComponent<VillageBehaviour>();
            mainVillagePlacement.position = mainVillageBehaviour.position;
            mainVillagePlacement.health = (int)mainVillageBehaviour.health;
            mainVillagePlacement.dead = false;
        }
        else
        {
            mainVillagePlacement.position = new Vector2(-1, -1);
            mainVillagePlacement.health = 0;
            mainVillagePlacement.dead = true;
        }

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
            gameGenerator.didWinGame = gameSave.didWinGame;
            gameGenerator.waveManager.infiniteMode = gameSave.infiniteMode;

            gameGenerator.forceDefeat = gameSave.defeated;

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
        List<GameObject> toRemove = new List<GameObject>();

        foreach (GameObject tower in gameGenerator.towers)
        {
            if (tower == null)
                continue;

            toRemove.Add(tower);
        }

        foreach (GameObject tower in toRemove)
        {
            gameGenerator.towers.Remove(tower);
            Destroy(tower);
        }
        
        GameObject mainVillage = gameGenerator.villageGenerator.mainVillage;
        VillageBehaviour mainVillageBehaviour = mainVillage.GetComponent<VillageBehaviour>();

        foreach (TowerPlacement towerPlacement in gameSave.towerPlacements)
        {
            GameObject prefab = GameManager.instance.GetTowerPrefab(towerPlacement.towerType);
            GameObject tower = Instantiate(prefab, gameGenerator.towerParent.transform);
            TowerBehaviour towerBehaviour = tower.GetComponent<TowerBehaviour>();

            towerBehaviour.position = towerPlacement.position;
            towerBehaviour.targetType = towerPlacement.targetType;
            towerBehaviour.stats = towerPlacement.stats;

            Vector3 positionOverride = new Vector3();

            if (towerPlacement.position == mainVillageBehaviour.position)
            {
                positionOverride = new Vector3(
                    mainVillageBehaviour.towerSpawn.transform.position.x,
                    mainVillageBehaviour.towerSpawn.transform.position.y + tower.transform.localScale.y / 2f,
                    mainVillageBehaviour.towerSpawn.transform.position.z
                );
            }

            gameGenerator.PlaceTower(towerBehaviour.position, tower, positionOverride: positionOverride, cost: false);
        }
    }

    void LoadVillages(GameSave gameSave)
    {
        // more complicated because village buildings could be destroyed and placed

        // first, delete all existing village buildings

        List<GameObject> toRemove = new List<GameObject>();

        foreach (GameObject village in gameGenerator.villageGenerator.villageBuildings)
        {
            if (village == null)
                continue;

            toRemove.Add(village);
        }

        foreach (GameObject village in toRemove)
        {
            gameGenerator.villageGenerator.maxHealth -= village.GetComponent<VillageBehaviour>().data.maxHealth;
            gameGenerator.villageGenerator.villageBuildings.Remove(village);
            Destroy(village);
        }

        // then, place the saved village buildings (using the gameGenerator.villageGenerator.PlaceVillage method)
        // to do so, find the prefab (stored in the villageGenerator) and the village positions, and set their health

        foreach (VillagePlacement villagePlacement in gameSave.villagePlacements)
        {
            GameObject prefab = gameGenerator.villageGenerator.villagePrefabs[0];
            Vector2 position = villagePlacement.position;

            gameGenerator.villageGenerator.PlaceVillage(prefab, position);

            GameObject lastVIllage = gameGenerator.villageGenerator.villageBuildings[gameGenerator.villageGenerator.villageBuildings.Count - 1];
            lastVIllage.GetComponent<VillageBehaviour>().health = villagePlacement.health;
        }
    }

    void LoadMainVillage(GameSave gameSave)
    {
        if (!gameSave.mainVillagePlacement.dead)
        {
            GameObject mainVillage = gameGenerator.villageGenerator.mainVillage;
            VillageBehaviour mainVillageBehaviour = mainVillage.GetComponent<VillageBehaviour>();

            mainVillageBehaviour.health = gameSave.mainVillagePlacement.health;
        }
        else
        {
            Destroy(gameGenerator.villageGenerator.mainVillage);
        }
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
