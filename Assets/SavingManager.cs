using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavingManager : MonoBehaviour
{
    public GameGenerator gameGenerator;

    public void SaveGame()
    {
        // in the PlayerData class, there is a list of GameSave objects
        // each GameSave object has a seed, a wave, a randomwithseed, a waverandomwithseed,
        // a list of towerplacements, a list of villageplacements, and a mainvillageplacement

        // the towerplacement class has a tower type, a position, and a target type
        // the villageplacement class has a position, and a health

        // first, remove all saves that has the same name
        GameManager.instance.player.gameSaves.RemoveAll(x => x.saveName == GameManager.instance.gameName);

        // create a new instance of the GameSave class, and fill it with the necessary data
        GameSave gameSave = new GameSave();
        gameSave.saveName = GameManager.instance.gameName;
        gameSave.seed = (int)gameGenerator.seed;
        gameSave.wave = gameGenerator.waveManager.wave;
        gameSave.randomWithSeed = gameGenerator.randomWithSeed;

        // add the tower placements to the game save
        foreach (GameObject tower in gameGenerator.towers)
        {
            TowerPlacement towerPlacement = new TowerPlacement();
            TowerBehaviour towerBehaviour = tower.GetComponent<TowerBehaviour>();

            towerPlacement.towerType = GetTowerType(towerBehaviour);
            towerPlacement.position = towerBehaviour.position;
            towerPlacement.targetType = towerBehaviour.targetType;

            gameSave.towerPlacements.Add(towerPlacement);
        }

        // add the village placements to the game save
        foreach (GameObject village in gameGenerator.villageGenerator.villageBuildings)
        {
            VillagePlacement villagePlacement = new VillagePlacement();
            VillageBehaviour villageBehaviour = village.GetComponent<VillageBehaviour>();

            villagePlacement.position = villageBehaviour.position;
            villagePlacement.health = (int)villageBehaviour.health;

            gameSave.villagePlacements.Add(villagePlacement);
        }

        // add the main village placement to the game save
        VillagePlacement mainVillagePlacement = new VillagePlacement();
        VillageBehaviour mainVillageBehaviour = gameGenerator.villageGenerator.mainVillage.GetComponent<VillageBehaviour>();

        mainVillagePlacement.position = mainVillageBehaviour.position;
        mainVillagePlacement.health = (int)mainVillageBehaviour.health;

        gameSave.mainVillagePlacement = mainVillagePlacement;

        // add the game save to the player data
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

    public void LoadGame()
    {
        
    }
}
