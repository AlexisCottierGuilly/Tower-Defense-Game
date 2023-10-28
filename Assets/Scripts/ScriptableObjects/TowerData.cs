using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TowerData", menuName = "Structures/TowerData")]
public class TowerData : StructureData
{
    public GameObject projectilePrefab; // dans le futur, créer une classe pour les projectiles
    public float maxRange;
    public float attackSpeed;
    public float cost;
}
