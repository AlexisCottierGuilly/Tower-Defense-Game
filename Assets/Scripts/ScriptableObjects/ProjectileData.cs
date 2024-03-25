using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileData", menuName = "Projectiles/ProjectileData")]
public class ProjectileData : ScriptableObject
{
    public float speed;
    public float radius;
    public int fireDuration;
    public int fireDamage;
    public int impactDamage;
    public float shootingAngle;
    public bool followTarget;
}
