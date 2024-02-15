using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TowerType
{
    None,
    Normal,
    Splash,
    Ultimate
}
//public one
//public two
//public three

public class TowerUI : MonoBehaviour
{
    public TowerType type = TowerType.None;

    public void ChangeTower(string newTower)
    {
        type = (TowerType)System.Enum.Parse(typeof(TowerType), newTower);
        print(type);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            type = TowerType.None;
        }
    }
}
