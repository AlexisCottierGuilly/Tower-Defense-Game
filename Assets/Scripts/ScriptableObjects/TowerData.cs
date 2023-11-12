using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TowerData", menuName = "Structures/TowerData")]
public class TowerData : StructureData
{
    public float maxRange;
    public float attackSpeed;
    public GameObject projectile;
}
