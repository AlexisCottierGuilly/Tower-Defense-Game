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
    public float climbingSpeedDivisor = 5f;

    private float timeFromPreviousAttack = 0f;
    private GameObject targetTower = null;

    [HideInInspector] public bool isClimbing;
    [HideInInspector] public Vector3 lastPosition;

    private bool didFirstUpdate = false;
    private Dictionary<MonsterTimedSpawn, float> timesSinceSpawn = new Dictionary<MonsterTimedSpawn, float>();
    private Dictionary<MonsterTimedSpawn, float> spawnRateIntervals = new Dictionary<MonsterTimedSpawn, float>();

    /*
    Here is how to implement data.timedSpawns:
    -> The timedSpawn is a class defining the random rate at which a certain number of monsters will spawn.
    -> So wee need to compare the current time with the spawn rate interval, and choose when to spawn which timedSpawn (there can be multiple timedSpawns in the list).

    1 - Save a dictionary of the current time since the last spawn for each timedSpawn.
    2 - For each timeSpawn, define a new spawn rate interval (random between the min and max of the spawn rate interval).
        Save it in a dictionary.
    3 - In update. For each timedSpawn, check if the current time is greater than the spawn rate interval.
        If it is, spawn the monsters, reset the spawn rate interval and define a new random interval.
    */
    
    // Start is called before the first frame update
    void Start()
    {
        agent.speed = data.speed * 4f;
        health = data.maxHealth;

        InitializeTimedSpawns();
        UpdateObjective();
    }

    void InitializeTimedSpawns()
    {
        timesSinceSpawn = new Dictionary<MonsterTimedSpawn, float>();
        spawnRateIntervals = new Dictionary<MonsterTimedSpawn, float>();
        
        foreach (MonsterTimedSpawn timedSpawn in data.timedSpawns)
        {
            timesSinceSpawn[timedSpawn] = 0f;
            spawnRateIntervals[timedSpawn] = Random.Range(timedSpawn.spawnRateInterval.x, timedSpawn.spawnRateInterval.y);
        }
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
            agent.speed = data.speed * 4f / climbingSpeedDivisor;
        else
            agent.speed = data.speed * 4f;
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
        health -= projectile.GetComponent<ProjectileBehaviour>().GetDamage();
        CheckDeath();
    }

    public void TakeDamageFromZone(GameObject zone)
    {
        health -= zone.GetComponent<ZoneBehaviour>().GetDamage();
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
        SpawnMonsters(data.spawnOnDeath);
    }

    void SpawnMonsters(List<MonsterCount> monsters)
    {
        foreach (MonsterCount monsterCount in monsters)
        {
            int count = gameGenerator.randomWithSeed.Next((int)monsterCount.countInterval.x, (int)monsterCount.countInterval.y);
            
            for (int i = 0; i < count; i++)
            {
                waveManager.SpawnMonster(monsterCount.type, transform.position);
            }
        }
    }

    void UpdateTimedSpawns()
    {
        foreach (MonsterTimedSpawn timedSpawn in data.timedSpawns)
        {
            timesSinceSpawn[timedSpawn] += Time.deltaTime;

            if (timesSinceSpawn[timedSpawn] >= spawnRateIntervals[timedSpawn])
            {
                SpawnMonsters(timedSpawn.monsters);

                timesSinceSpawn[timedSpawn] = 0f;
                
                float min = timedSpawn.spawnRateInterval.x;
                float diff = timedSpawn.spawnRateInterval.y - min;
                spawnRateIntervals[timedSpawn] = (float)(gameGenerator.randomWithSeed.NextDouble() * diff + min);
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

        UpdateTimedSpawns();

        /*if (agent.pathStatus == NavMeshPathStatus.PathComplete && targetTower != null)
        {
            // Debug.Log("Arrived to objective !", this.gameObject);
            AttackStructure(targetTower);
        }*/
    }
}
