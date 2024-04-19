using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum VillageType
{
    None,
    House,
    MainVillage
}

public class VillageUI : MonoBehaviour
{
    public VillageType villageType = VillageType.None;
    public GameObject house;
    public DetectSelection selectionManager;
    
    public void ChangeVillage(string newTower)
    {
        villageType = (VillageType)System.Enum.Parse(typeof(VillageType), newTower);
        if (villageType != null)
        {
            if (villageType == VillageType.House)
            {
                selectionManager.SetStructure(house, ObjectType.Village);
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            villageType = VillageType.None;
            selectionManager.UnsetStructure();
        }
        else if (Input.GetKeyDown(KeyCode.V))
        {
            villageType = VillageType.House;
            selectionManager.SetStructure(house, ObjectType.Village);
        }
    }
}
