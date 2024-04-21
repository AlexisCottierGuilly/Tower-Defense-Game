using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileData", menuName = "Projectiles/ProjectileData")]
public class ProjectileData : ScriptableObject
{
    public float hitRadius = 0.5f;
    public int impactDamage;
    public bool followTarget;
    [Space]
    public GameObject impactZone;
    [Space]
    public bool isEnemy = false;
}
