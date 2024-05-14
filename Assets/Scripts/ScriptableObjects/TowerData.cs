using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TowerData", menuName = "Structures/TowerData")]
public class TowerData : StructureData
{
    public int shopCost;
    [Space]
    public float maxRange;
    public float attackSpeed;
    public GameObject projectile;
    public bool targetEnemy = true;
    public bool targetVillage = false;
    [Space]
    public GameObject prefab;
}
