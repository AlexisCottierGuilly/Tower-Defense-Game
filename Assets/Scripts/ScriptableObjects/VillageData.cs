using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VillageData", menuName = "Structures/VillageData")]
public class VillageData : StructureData
{
    public int maxHealth;
    public float hitRange;

    [Header("Main Village Only")]
    public bool towerOnTop = false;
}
