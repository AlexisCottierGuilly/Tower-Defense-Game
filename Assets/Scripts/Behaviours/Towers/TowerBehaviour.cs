using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum TargetType
{
    Proche,
    Loin,
    Fort,
    Faible
}


public enum TowerType
{
    None,
    Normal,
    Explosive,
    Ultimate,
    Slow,
    Healing,
    Paralysing
}


[System.Serializable]
public class TowerStats
{
    public int damageDealt = 0;
}


public class TowerBehaviour : StructureBehaviour
{
    public TowerData data;
    [Space]
    public GameObject canon;
    public GameObject canonSupport;
    public GameObject projectileSpawnEmpty;
    public float verticalShootAngle = 0f;
    [Header("Stats")]
    public TowerStats stats = new TowerStats();

    // Shooting infos
    private float horizontalShootAngle = 0f;
    private float currentRechargingTime = 0f;

    [HideInInspector] public GameObject projectileParent;
    [HideInInspector] public TargetType targetType = TargetType.Proche;
    
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        if (!GameManager.instance.generator.waveManager.waveFinished)
            currentRechargingTime += Time.deltaTime;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(gameObject.transform.position, data.maxRange);
    }

    void TurnToward(GameObject go)
    {
        float yVar = go.transform.position.x - gameObject.transform.position.x;
        float xVar = go.transform.position.z - gameObject.transform.position.z;

        float angle = Mathf.Atan2(yVar, xVar) * Mathf.Rad2Deg;
        
        this.transform.eulerAngles = new Vector3(
            this.transform.eulerAngles.x,
            angle,
            this.transform.eulerAngles.z
        );
    }

    GameObject FindTargetAndAngle()
    {
        if (data.targetEnemy)
        {
            return FindEnemyAndAngle();
        }
        else
        {
            return FindVillageAndAngle();
        }
    }

    GameObject FindEnemyAndAngle()
    {
        // Returns true if there is an enemy
        Collider[] hitColliders = Physics.OverlapSphere(gameObject.transform.position, data.maxRange);
        float bestScore = -1f;
        GameObject bestObject = null;

        foreach (Collider other in hitColliders)
        {
            GameObject obj = other.gameObject;

            if (obj.CompareTag("Monster"))
            {
                float distance = Vector3.Distance(gameObject.transform.position, obj.transform.position);
                float health = (float)obj.GetComponent<MonsterBehaviour>().health;
                
                if (targetType == TargetType.Proche && (bestScore == -1f || distance < bestScore))
                {
                    bestScore = distance;
                    bestObject = obj;
                }
                else if (targetType == TargetType.Loin && (bestScore == -1f || distance > bestScore))
                {
                    bestScore = distance;
                    bestObject = obj;
                }
                else if (targetType == TargetType.Fort && (bestScore == -1f || health > bestScore))
                {
                    bestScore = health;
                    bestObject = obj;
                }
                else if (targetType == TargetType.Faible && (bestScore == -1f || health < bestScore))
                {
                    bestScore = health;
                    bestObject = obj;
                }
            }
        }

        if (bestObject != null)
        {
            TurnToward(bestObject);
        }
        
        return bestObject;
    }

    GameObject FindVillageAndAngle()
    {
        // Returns true if there is an enemy
        Collider[] hitColliders = Physics.OverlapSphere(gameObject.transform.position, data.maxRange);
        float bestScore = -1f;
        GameObject bestObject = null;

        foreach (Collider other in hitColliders)
        {
            GameObject obj = other.gameObject;

            if (obj.CompareTag("Village Building"))
            {
                float distance = Vector3.Distance(gameObject.transform.position, obj.transform.position);
                float health = (float)obj.GetComponent<VillageBehaviour>().health;

                if (targetType == TargetType.Proche && (bestScore == -1f || distance < bestScore))
                {
                    bestScore = distance;
                    bestObject = obj;
                }
                else if (targetType == TargetType.Loin && (bestScore == -1f || distance > bestScore))
                {
                    bestScore = distance;
                    bestObject = obj;
                }
                else if (targetType == TargetType.Fort && (bestScore == -1f || health > bestScore))
                {
                    bestScore = health;
                    bestObject = obj;
                }
                else if (targetType == TargetType.Faible && (bestScore == -1f || health < bestScore))
                {
                    bestScore = health;
                    bestObject = obj;
                }
            }
        }

        if (bestObject != null)
        {
            TurnToward(bestObject);
        }
        
        return bestObject;
    }

    public void UpdateLogic()
    {
        GameObject enemy = FindTargetAndAngle();
        if (enemy != null && currentRechargingTime >= data.attackSpeed) {
            currentRechargingTime = 0f;

            //Shoot(enemy);

            GameManager.instance.generator.shootingManager.Shoot(
                gameObject,
                enemy,
                data.targetEnemy,
                data.targetVillage,
                data.projectile,
                verticalShootAngle,
                projectileParent,
                projectileSpawnEmpty
            );
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
