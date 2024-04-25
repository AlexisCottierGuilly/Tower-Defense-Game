using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
    
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        currentRechargingTime += Time.deltaTime;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(gameObject.transform.position, data.maxRange);
    }

    /*float GetShootingForce(GameObject monster)
    {
        // Vi = ?
        // float angle = verticalShootAngle
        // [deltaY] float distY = gameObject.transform.position.y - monster.transform.position.y
        // [deltaX] float portee = Mathf.Sqrt(
        //     Mathf.Pow(gameObject.transform.position.x - monster.transform.position.x, 2f),
        //     Mathf.Pow(gameObject.transform.position.z - monster.transform.position.z, 2f),
        // );
        // float acceleration = Physics.gravity.y;

        // Vix = deltaX / (sqrt((2(deltaX * tan(angle) - deltaY) / acceleration)))

        //                     OU
        
        //                  deltaX
        // Vix = -_-_-_-_-_-_-_-_-_-_-_-_-_-_-_
        //       | deltaX * tan(angle) - deltaY
        //       | ----------------------------
        //      V        acceleration
        
        // Viy = Vix * tan(angle)
        // Vi = Mathf.Atan2(Viy, Vix) * Mathf.Rad2Deg

        // return Vi !!!
        
        float acceleration = Physics.gravity.y;  // acceleration negative
        float angle = 25f; // BOGUE //verticalShootAngle;
        
        float deltaX = Mathf.Sqrt(
            Mathf.Pow(gameObject.transform.position.x - monster.transform.position.x, 2f) + 
            Mathf.Pow(gameObject.transform.position.z - monster.transform.position.z, 2f)
        );
        float deltaY = gameObject.transform.position.y - monster.transform.position.y;
        
        float angleDiffMultiplier;
        if (verticalShootAngle >= 25f)
            angleDiffMultiplier = Mathf.Pow(verticalShootAngle / angle, 1.35f);
        else
            angleDiffMultiplier = 1f * 1.025f;
        
        if (verticalShootAngle >= 25f)
            deltaY *= 17.5f / Mathf.Pow(angleDiffMultiplier, 1.25f);
        else
            deltaY *= 17.5f / (angleDiffMultiplier * 0.825f);  // Mathf.Pow(angleDiffMultiplier, 1.15f) * 1.2f;
        
        // if (Mathf.Atan2(deltaY, deltaX) * Mathf.Rad2Deg >= angle) {
        //     return 0f;  // Not possible to shoot on the monster
        // }

        float viX = deltaX / (Mathf.Sqrt((2 * (deltaX * Mathf.Tan(angle) * Mathf.Rad2Deg - deltaY)) / acceleration));
        float viY = viX * Mathf.Tan(angle) * Mathf.Rad2Deg;
        float vi = Mathf.Sqrt(Mathf.Pow(viX, 2f) + Mathf.Pow(viY, 2f));

        return vi * 9f * angleDiffMultiplier; //7.5f;  // 10f;

        //return 350f * data.attackStrength;
    }

    void Shoot(GameObject monster)
    {
        horizontalShootAngle = transform.eulerAngles.y;

        float power = GetShootingForce(monster);

        if (power != 0f)
        {
            GameObject projectile = Instantiate(data.projectile, projectileSpawnEmpty.transform.position, Quaternion.identity);
            projectile.transform.eulerAngles = new Vector3(  // ROTATION MOCHE
                90f - verticalShootAngle, //canonSupport.transform.eulerAngles.x,
                this.transform.eulerAngles.y,
                projectile.transform.eulerAngles.z
            );
            projectile.transform.parent = projectileParent.transform;
            projectile.GetComponent<ProjectileBehaviour>().SetTarget(monster);
            projectile.GetComponent<ProjectileBehaviour>().sender = this.gameObject;
            
            Vector3 force = new Vector3();

            float horizontalPower = power * Mathf.Cos(verticalShootAngle * Mathf.Deg2Rad);  // horizontal force
            force.y = power * Mathf.Sin(verticalShootAngle * Mathf.Deg2Rad);  // vertical force

            force.z = horizontalPower * Mathf.Cos(horizontalShootAngle * Mathf.Deg2Rad);  // forward force
            force.x = horizontalPower * Mathf.Sin(horizontalShootAngle * Mathf.Deg2Rad);  // side force

            if (!float.IsNaN(force.x))
            {
                projectile.gameObject.GetComponent<Rigidbody>().AddForce(force);
            }
            else
            {
                Destroy(projectile);
            }
        }
    }*/

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
        float closestDistance = -1f;
        GameObject closestObject = null;

        foreach (Collider other in hitColliders)
        {
            GameObject obj = other.gameObject;

            if (obj.CompareTag("Monster"))
            {
                float distance = Vector3.Distance(gameObject.transform.position, obj.transform.position);
                
                if (closestDistance == -1f || distance < closestDistance)
                {
                    closestDistance = distance;
                    closestObject = obj;
                }
            }
        }

        if (closestObject != null)
        {
            TurnToward(closestObject);
        }
        
        return closestObject;
    }

    GameObject FindVillageAndAngle()
    {
        // Returns true if there is an enemy
        Collider[] hitColliders = Physics.OverlapSphere(gameObject.transform.position, data.maxRange);
        float closestDistance = -1f;
        GameObject closestObject = null;

        foreach (Collider other in hitColliders)
        {
            GameObject obj = other.gameObject;

            if (obj.CompareTag("Village Building"))
            {
                float distance = Vector3.Distance(gameObject.transform.position, obj.transform.position);
                
                if (closestDistance == -1f || distance < closestDistance)
                {
                    closestDistance = distance;
                    closestObject = obj;
                }
            }
        }

        if (closestObject != null)
        {
            TurnToward(closestObject);
        }
        
        return closestObject;
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
