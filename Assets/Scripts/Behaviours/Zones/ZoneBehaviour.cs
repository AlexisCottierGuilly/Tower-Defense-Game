using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneBehaviour : MonoBehaviour
{
    public ZoneData data;
    public GameObject sender;
    private float timeFromSpawn = 0f;
    private float timeFromLastDamage = int.MaxValue;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(gameObject.transform.position, data.radius);
    }

    void CheckForEnemies()
    {
        Collider[] hitColliders = Physics.OverlapSphere(gameObject.transform.position, data.radius);

        foreach (Collider hitCollider in hitColliders)
        {
            MonsterBehaviour monsterBehaviour = hitCollider.GetComponent<MonsterBehaviour>();
            if (monsterBehaviour != null)
            {
                monsterBehaviour.TakeDamageFromZone(gameObject);
            }
        }
    }

    public int GetDamage()
    {
        int damage = data.damage;

        if (sender != null)
        {
            TowerBehaviour behaviour = sender.GetComponent<TowerBehaviour>();
            if (behaviour != null)
            {
                behaviour.stats.damageDealt += damage;
            }
        }

        return damage;
    }
    
    void Update()
    {
        timeFromSpawn += Time.deltaTime;
        timeFromLastDamage += Time.deltaTime;

        if (timeFromLastDamage >= data.interval)
        {
            CheckForEnemies();
            timeFromLastDamage = 0f;
        }

        if (timeFromSpawn >= data.duration)
            Destroy(gameObject);
    }
}
