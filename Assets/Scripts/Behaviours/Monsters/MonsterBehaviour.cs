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
    public float verticalShootAngle;
    [Space]
    public float permanentSpeedModifier = 1f;
    public float temporarySpeedModifier = 1f;

    private float timeFromPreviousAttack = 0f;
    private GameObject targetTower = null;

    [HideInInspector] public bool isClimbing;
    [HideInInspector] public Vector3 lastPosition;
    [HideInInspector] public float unmodifiedSpeed = 1f;
    [HideInInspector] public GameObject projectileParent;

    private bool didFirstUpdate = false;
    private Dictionary<MonsterTimedSpawn, float> timesSinceSpawn = new Dictionary<MonsterTimedSpawn, float>();
    private Dictionary<MonsterTimedSpawn, float> spawnRateIntervals = new Dictionary<MonsterTimedSpawn, float>();
    private float currentRechargingTime = 0f;
    private bool shootOverride = true;
    private bool didRegisterDeath = false;

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

        agent.speed = finalSpeed * 3.5f;
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

    public void UpdateObjective(bool canSearchVillages = true)
    {
        bool searchVillages = true;
        if (data.targetEnemy)
        {
            searchVillages = !UpdateEnemyObjective();
        }

        if (searchVillages && (canSearchVillages || (data.targetEnemy && shootOverride && !GameManager.instance.generator.waveManager.isSpawningMonsters)))
        {
            if (!data.targetEnemy || !GameManager.instance.generator.waveManager.isSpawningMonsters)
            {
                UpdateVillageObjective();

                if (data.targetEnemy)
                {
                    shootOverride = false;
                    agent.isStopped = false;
                }
            }
        }
    }

    private void UpdateVillageObjective()
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

        agent.SetDestination(objective);
        targetTower = objectiveObject;
    }

    private bool UpdateEnemyObjective()
    {
        GameObject closestEnemy = null;
        float minDistance = -1f;

        foreach (GameObject enemy in gameGenerator.waveManager.monsters)
        {
            if (enemy == null || enemy == gameObject)
                continue;
            
            MonsterBehaviour monster = enemy.GetComponent<MonsterBehaviour>();
            if (monster != null && monster.data == data)
                continue;

            float dist = Vector3.Distance(enemy.transform.position, transform.position);
            if (minDistance == -1f || dist < minDistance)
            {
                minDistance = dist;
                closestEnemy = enemy;
            }
        }

        if (closestEnemy != null)
        {
            agent.SetDestination(closestEnemy.transform.position);
            targetTower = closestEnemy;
            return true;
        }

        return false;
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

        if (health > data.maxHealth)
            health = data.maxHealth;
    }

    public void ZoneHit(GameObject zone)
    {
        int damage = zone.GetComponent<ZoneBehaviour>().GetDamage();

        health -= damage;
        CheckDeath();

        if (health > data.maxHealth)
            health = data.maxHealth;
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
        if (!didRegisterDeath)
        {
            GameManager.instance.gold += data.gold;
            GameManager.instance.player.crystals += data.crystals;
            SpawnMonsters(data.spawnOnDeath);

            didRegisterDeath = true;
        }
    }

    void SpawnMonsters(List<MonsterCount> monsters)
    {
        foreach (MonsterCount monsterCount in monsters)
        {
            int count = gameGenerator.randomWithSeed.Next((int)monsterCount.countInterval.x, (int)monsterCount.countInterval.y);
            
            for (int i = 0; i < count; i++)
            {
                // a little randomised position with the gameGenerator.randomWithSeed (+- 0.2f tiles)

                float randomRandius = 2f;

                Vector3 position = new Vector3(
                    transform.position.x + (float)(gameGenerator.randomWithSeed.NextDouble() * randomRandius * 2f - randomRandius),
                    transform.position.y,
                    transform.position.z + (float)(gameGenerator.randomWithSeed.NextDouble() * randomRandius * 2f - randomRandius)
                );
                
                waveManager.SpawnMonster(monsterCount.type, position);
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

        if (data.canShoot && shootOverride)
        {
            UpdateStop();
            UpdateRotation();
            TryShoot();
        }

        UpdateTimedSpawns();

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

            if (lookRotation != Quaternion.identity)
            {
                Vector3 rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * 5f).eulerAngles;
                transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);
            }
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
                    data.targetEnemy,
                    data.targetVillage,
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
