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

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
            RemoveFromGame();
    }

    public void RemoveFromGame()
    {
        generator.RemoveVillageStructure(gameObject);
    }
}
