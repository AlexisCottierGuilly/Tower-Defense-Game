using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MonsterData", menuName = "Monsters/MonsterData")]
public class MonsterData : ScriptableObject
{
    public int maxHealth;
    public int damage;
    public float attackSpeed;
    public float speed;
    public int gold;
    public List<MonsterCount> spawnOnDeath;
    [Space]
    public GameObject prefab;
}

[System.Serializable]
public class MonsterCount
{
    public Monster type;
    public int count;
}

