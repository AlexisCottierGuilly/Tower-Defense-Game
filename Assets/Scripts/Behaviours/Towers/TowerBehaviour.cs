using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBehaviour : StructureBehaviour
{
    public TowerData data;
    public GameObject projectileParent;
    [Space]
    public GameObject canon;
    public GameObject projectileSpawnEmpty;
    public float verticalShootAngle = 0f;

    // Shooting infos
    private float horizontalShootAngle = 0f;
    private float currentRechargingTime = 0f;
    
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

    float GetShootingForce(GameObject monster)
    {
        /*
        Vi = ?
        float angle = verticalShootAngle
        float distY = gameObject.transform.position.y - monster.transform.position.y
        float portee = Mathf.Sqrt(
            Mathf.Pow(gameObject.transform.position.x - monster.transform.position.x, 2f),
            Mathf.Pow(gameObject.transform.position.z - monster.transform.position.z, 2f),
        );
        float acceleration = Physics.gravity.y;

        -> Système d'équations
        Viy = Vix(tan(angle))
        Vix = portee / deltaT
        distY = Viy(deltaT) + 1/2(acceleration)(deltaT ** 2)

        Trouver Vi...
        
        return Vi !!!
        */
        
        float portee = Mathf.Sqrt(
            Mathf.Pow(gameObject.transform.position.x - monster.transform.position.x, 2f) + 
            Mathf.Pow(gameObject.transform.position.z - monster.transform.position.z, 2f)
        );

        float acceleration = Physics.gravity.y;

        return 350f * data.attackStrength;
    }

    void Shoot(GameObject monster)
    {
        horizontalShootAngle = transform.eulerAngles.y;

        GameObject projectile = Instantiate(data.projectile, projectileSpawnEmpty.transform.position, Quaternion.identity);
        projectile.transform.eulerAngles = new Vector3(
            canon.transform.eulerAngles.x,
            projectile.transform.eulerAngles.y,
            projectile.transform.eulerAngles.z
        );
        projectile.transform.parent = projectileParent.transform;
        
        Vector3 force = new Vector3();
        float power = GetShootingForce(monster);  // 350f * data.attackStrength;

        // TODO: change rotation (horizontal)

        float horizontalPower = power * Mathf.Cos(verticalShootAngle * Mathf.Deg2Rad);  // horizontal force
        force.y = power * Mathf.Sin(verticalShootAngle * Mathf.Deg2Rad);  // vertical force

        force.z = horizontalPower * Mathf.Cos(horizontalShootAngle * Mathf.Deg2Rad);  // forward force
        force.x = horizontalPower * Mathf.Sin(horizontalShootAngle * Mathf.Deg2Rad);  // side force

        projectile.gameObject.GetComponent<Rigidbody>().AddForce(force);
    }

    GameObject UpdateShootAngle()
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
            float yVar = closestObject.transform.position.x - gameObject.transform.position.x;
            float xVar = closestObject.transform.position.z - gameObject.transform.position.z;

            float angle = Mathf.Atan2(yVar, xVar) * Mathf.Rad2Deg;
            
            gameObject.transform.eulerAngles = new Vector3(
                gameObject.transform.eulerAngles.x,
                angle,
                gameObject.transform.eulerAngles.z
            );
        }
        
        return closestObject;
    }

    public void UpdateLogic()
    {
        GameObject enemy = UpdateShootAngle();
        if (enemy != null && currentRechargingTime >= data.attackSpeed) {
            currentRechargingTime = 0f;
            
            Shoot(enemy);
        }
    }
}
