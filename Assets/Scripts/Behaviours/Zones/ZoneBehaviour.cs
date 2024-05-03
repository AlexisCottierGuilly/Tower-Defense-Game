using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class ZoneBehaviour : MonoBehaviour
{
    public ZoneData data;

    private float timeFromSpawn = 0f;
    private float timeFromLastDamage = int.MaxValue;

    [HideInInspector] public GameObject sender;
    [HideInInspector] public bool targetEnemy = true;
    [HideInInspector] public bool targetVillage = true;

    void Start()
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        VolumeUpdater volumeUpdater = gameObject.AddComponent<VolumeUpdater>();
        volumeUpdater.volumeMultiplier = 1f;

        audioSource.spatialBlend = 0.75f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.maxDistance = 100f;

        audioSource.clip = data.sound;
        audioSource.Play();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(gameObject.transform.position, data.radius);
    }

    void CheckForEnemies()
    {
        Collider[] hitColliders = Physics.OverlapSphere(gameObject.transform.position, data.radius);
        bool dealDamage = timeFromLastDamage >= data.interval;
        
        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.CompareTag("Monster") && targetEnemy)
            {
                MonsterBehaviour monster = hitCollider.gameObject.GetComponent<MonsterBehaviour>();
                if (dealDamage)
                    monster.ZoneHit(gameObject);

                monster.AddTemporaryModifier(data.speedModifier);
            }
            else if (hitCollider.gameObject.CompareTag("Village Building") && targetVillage)
            {
                VillageBehaviour village = hitCollider.gameObject.GetComponent<VillageBehaviour>();
                if (dealDamage)
                    village.ZoneHit(gameObject);
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
    
    void LateUpdate()
    {
        timeFromSpawn += Time.deltaTime;
        timeFromLastDamage += Time.deltaTime;

        if (timeFromSpawn >= data.duration)
            Destroy(gameObject);
        
        if (data.speedModifier != 1f || timeFromLastDamage >= data.interval)
        {
            CheckForEnemies();
            
            if (timeFromLastDamage >= data.interval)
                timeFromLastDamage = 0f;
        }
    }
}
