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
    [Space]
    public GameObject projectileSpawnEmpty;
    public GameObject projectileParent;
    public float verticalShootAngle;
    [Space]
    public float permanentSpeedModifier = 1f;
    public float temporarySpeedModifier = 1f;

    private float timeFromPreviousAttack = 0f;
    private GameObject targetTower = null;

    [HideInInspector] public bool isClimbing;
    [HideInInspector] public Vector3 lastPosition;
    [HideInInspector] public float unmodifiedSpeed = 1f;

    private bool didFirstUpdate = false;
    private Dictionary<MonsterTimedSpawn, float> timesSinceSpawn = new Dictionary<MonsterTimedSpawn, float>();
    private Dictionary<MonsterTimedSpawn, float> spawnRateIntervals = new Dictionary<MonsterTimedSpawn, float>();
    private float currentRechargingTime = 0f;

    void Start()
    {
        SetSpeed(data.speed);

        agent.stoppingDistance = 4f;
        health = data.maxHealth;

        InitializeTimedSpawns();
        UpdateObjective();
    }

    public void SetSpeed(float speed)
    {
        unmodifiedSpeed = speed;
        float finalSpeed = unmodifiedSpeed;

        finalSpeed *= permanentSpeedModifier;
        finalSpeed *= temporarySpeedModifier;

        agent.speed = finalSpeed * 4f;
    }

    public void AddTemporaryModifier(float modifier)
    {
        temporarySpeedModifier *= modifier;
        SetSpeed(unmodifiedSpeed);
    }

    public void AddPermanentModifier(float modifier)
    {
        permanentSpeedModifier *= modifier;
        SetSpeed(unmodifiedSpeed);
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
            SetSpeed(data.speed / climbingSpeedDivisor);
        else
            SetSpeed(data.speed);
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

    public void ProjectileHit(GameObject projectile)
    {
        health -= projectile.GetComponent<ProjectileBehaviour>().GetDamage();
        CheckDeath();
    }

    public void ZoneHit(GameObject zone)
    {
        health -= zone.GetComponent<ZoneBehaviour>().GetDamage();
        CheckDeath();
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
        GameManager.instance.player.crystals += data.crystals;
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
        currentRechargingTime += Time.deltaTime;

        if (lastPosition != null)
            UpdateIsClimbing();

        lastPosition = transform.position;

        if (!didFirstUpdate)
        {
            didFirstUpdate = true;
            UpdateObjective();
        }

        if (data.canShoot)
        {
            UpdateStop();
            UpdateRotation();
            TryShoot();
        }

        UpdateTimedSpawns();
    }

    void LateUpdate()
    {
        temporarySpeedModifier = 1f;
    }

    private void UpdateStop()
    {
        if (targetTower != null)
        {
            float distance = Vector3.Distance(transform.position, targetTower.transform.position);
            if (distance <= data.range)
            {
                agent.isStopped = true;
            }
            else
            {
                agent.isStopped = false;
            }
        }
    }

    private void UpdateRotation()
    {
        if (targetTower != null)
        {
            Vector3 direction = targetTower.transform.position - transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            Vector3 rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * 5f).eulerAngles;
            transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);
        }
    }

    private void TryShoot()
    {
        if (data.canShoot && currentRechargingTime >= data.shootSpeed && targetTower != null && agent.isStopped)
        {
            float distance = Vector3.Distance(transform.position, targetTower.transform.position);
            if (distance <= data.range)
            {
                gameGenerator.shootingManager.Shoot(
                    this.gameObject,
                    targetTower,
                    data.projectile,
                    verticalShootAngle,
                    projectileParent,
                    projectileSpawnEmpty
                );
                currentRechargingTime = 0f;
            }
        }
    }
}
