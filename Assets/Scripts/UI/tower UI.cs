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

public class TowerUI : MonoBehaviour
{
    public TowerType type = TowerType.None;
    public GameObject tower1;
    public GameObject tower2;
    public GameObject tower3;
    public GameObject selectionManagerObject;

    DetectSelection selectionManager;

    void Awake()
    {
        selectionManager = selectionManagerObject.GetComponent<DetectSelection>();
    }
    
    public void ChangeTower(string newTower)
    {
        type = (TowerType)System.Enum.Parse(typeof(TowerType), newTower);
        if (type != null)
        {
            if (newTower == "Normal")
            {
                selectionManager.SetStructure(tower1);
            }
            else if  (newTower == "Splash")
            {
                selectionManager.SetStructure(tower2);
            }
            else if  (newTower == "Ultimate")
            {
                selectionManager.SetStructure(tower3);
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            type = TowerType.None;
            selectionManager.UnsetStructure();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            type = TowerType.Normal;
            selectionManager.SetStructure(tower1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            type = TowerType.Splash;
            selectionManager.SetStructure(tower2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            type = TowerType.Ultimate;
            selectionManager.SetStructure(tower3);
        }
    }
}
