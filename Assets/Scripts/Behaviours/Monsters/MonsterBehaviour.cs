using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterBehaviour : MonoBehaviour
{
    public MonsterData data;
    public int health;
    public MonsterHealthUpdater healthScript;
    public WaveManager waveManager;
    public GameGenerator gameGenerator = null;
    
    [Space]
    public NavMeshAgent agent;
    public Vector3 finalScale = new Vector3(1f, 1f, 1f);
    [Space]
    public float speed = 1f;
    public float climbingSpeedDivisor = 7f;

    private float timeFromPreviousAttack = 0f;
    private GameObject targetTower = null;

    [HideInInspector] public bool isClimbing;
    [HideInInspector] public Vector3 lastPosition;

    private bool didFirstUpdate = false;
    
    // Start is called before the first frame update
    void Start()
    {
        agent.speed = data.speed * 4f;
        health = data.maxHealth;

        UpdateObjective();
    }

    public void UpdateObjective()
    {
        Vector3 objective = Vector3.zero;
        GameObject objectiveObject = null;
        float minDistance = -1f;
        
        if (gameGenerator.villageGenerator.mainVillage != null)
        {
            objective = gameGenerator.villageGenerator.mainVillage.transform.position;
            objectiveObject = gameGenerator.villageGenerator.mainVillage;
            minDistance = Vector3.Distance(objective, transform.position) * 2f;
        }

        foreach (GameObject building in gameGenerator.villageGenerator.villageBuildings)
        {
            float dist = Vector3.Distance(building.transform.position, transform.position);
            if (minDistance == -1f || dist < minDistance)
            {
                minDistance = dist;
                objectiveObject = building;
                objective = building.transform.position;
            }
        }

        agent.destination = objective;
        targetTower = objectiveObject;
    }

    public void UpdateIsClimbing()
    {
        float xzDistance = Vector3.Distance(
            new Vector3(lastPosition.x, 0f, lastPosition.z),
            new Vector3(transform.position.x, 0f, transform.position.z)
        );
        float yDistance = transform.position.y - lastPosition.y;

        float angle = Mathf.Atan(yDistance / xzDistance) * Mathf.Rad2Deg;
        angle = Mathf.Abs(angle);

        isClimbing = angle > 60f;

        if (isClimbing)
            agent.speed = speed / climbingSpeedDivisor;
        else
            agent.speed = speed;
    }

    public void AttackStructure(GameObject structure)
    {
        float attackCoolDown = 1f / data.attackSpeed;
        
        if (timeFromPreviousAttack >= attackCoolDown) {
            VillageBehaviour villageBehaviour = structure.GetComponent<VillageBehaviour>();
            villageBehaviour.TakeDamage(data.damage);
            timeFromPreviousAttack = 0f;
        }
    }

    public void TakeDamage(GameObject projectile)
    {
        health -= projectile.GetComponent<ProjectileBehaviour>().data.impactDamage;
        CheckDeath();
    }

    public void TakeDamageFromZone(GameObject zone)
    {
        health -= zone.GetComponent<ZoneBehaviour>().data.damage;
        CheckDeath();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Projectile"))
        {
            TakeDamage(other.gameObject);
        }
    }

    void CheckDeath()
    {
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
        GameManager.instance.gold += data.gold;
        SpawnDeathMonsters();
    }

    void SpawnDeathMonsters()
    {
        foreach (MonsterCount monsterCount in data.spawnOnDeath)
        {
            for (int i = 0; i < monsterCount.count; i++)
            {
                waveManager.SpawnMonster(monsterCount.type, transform.position);
            }
        }
    }

    void Update()
    {
        timeFromPreviousAttack += Time.deltaTime;

        if (lastPosition != null)
            UpdateIsClimbing();

        lastPosition = transform.position;

        if (!didFirstUpdate)
        {
            didFirstUpdate = true;
            UpdateObjective();
        }

        /*if (agent.pathStatus == NavMeshPathStatus.PathComplete && targetTower != null)
        {
            // Debug.Log("Arrived to objective !", this.gameObject);
            AttackStructure(targetTower);
        }*/
    }
}
