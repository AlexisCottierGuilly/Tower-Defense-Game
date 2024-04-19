using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillageBehaviour : StructureBehaviour
{
    public VillageData data;
    public VillageGenerator generator;
    
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

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
        health -= damage;

        if (health <= 0)
            RemoveFromGame();
    }

    public void RemoveFromGame()
    {
        if (gameObject != null)
        {
            // Oui... le gameObject a déjà été détruit une fois.
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
