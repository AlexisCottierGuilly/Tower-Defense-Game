using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MonsterData", menuName = "Monsters/MonsterData")]
public class MonsterData : ScriptableObject
{
    public string name = "";
    public string spawnMessage = "";
    [Space]
    public bool isBoss = false;

    [Header("General")]
    public int maxHealth;
    public int damage;
    public float attackSpeed;
    public float speed;
    public int gold;
    public int crystals;

    [Header("Shooting")]
    public bool canShoot = false;
    public float range = 0f;
    public float shootSpeed = 0f;
    [Space]
    public bool targetEnemy = false;
    public bool targetVillage = true;
    public bool avoidSameType = false;
    [Space]
    public GameObject projectile;

    [Header("Spawning")]
    public List<MonsterCount> spawnOnDeath;
    public List<MonsterTimedSpawn> timedSpawns;
    [Space]
    public GameObject prefab;
}

[System.Serializable]
public class MonsterCount
{
    public Monster type;
    public Vector2 countInterval;
}

[System.Serializable]
public class MonsterTimedSpawn
{
    public List<MonsterCount> monsters;
    public Vector2 spawnRateInterval;
}
