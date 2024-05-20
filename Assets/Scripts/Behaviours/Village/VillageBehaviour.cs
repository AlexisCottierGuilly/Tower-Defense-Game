using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillageBehaviour : StructureBehaviour
{
    public VillageData data;
    public VillageGenerator generator;
    public GameObject towerSpawn;

    private bool didRemove = false;
    
    // Start is called before the first frame update
    void Awake()
    {
        health = data.maxHealth;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(gameObject.transform.position, data.hitRange);
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        UpdateMonsterHits();
    }

    public void TakeDamage(int damage)
    {
        if (damage > 0)
            GameManager.instance.generator.lostLives +=damage;
        
        health -= damage;

        if (health > data.maxHealth)
            health = data.maxHealth;

        if (health <= 0)
            RemoveFromGame();
    }

    public void ProjectileHit(GameObject projectile)
    {
        ProjectileBehaviour behaviour = projectile.GetComponent<ProjectileBehaviour>();
        if (behaviour != null)
        {
            TakeDamage(behaviour.GetDamage());
        }
    }

    public void ZoneHit(GameObject zone)
    {
        ZoneBehaviour behaviour = zone.GetComponent<ZoneBehaviour>();
        if (behaviour != null)
        {
            TakeDamage(behaviour.GetDamage());
        }
    }

    public void RemoveFromGame()
    {
        if (!didRemove)
        {
            didRemove = true;
            generator.RemoveVillageStructure(gameObject);
        }
    }

    void UpdateMonsterHits()
    {
        Collider[] hitColliders = Physics.OverlapSphere(gameObject.transform.position, data.hitRange);

        foreach (Collider hitCollider in hitColliders)
        {
            MonsterBehaviour monsterBehaviour = hitCollider.GetComponent<MonsterBehaviour>();
            if (monsterBehaviour != null)
            {
                monsterBehaviour.AttackStructure(gameObject);
            }
        }
    }
}
