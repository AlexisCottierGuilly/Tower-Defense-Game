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

    void Shoot()
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
        float power = 350f * data.attackStrength;

        // TODO: change rotation (horizontal)

        float horizontalPower = power * Mathf.Cos(verticalShootAngle * Mathf.Deg2Rad);  // horizontal force
        force.y = power * Mathf.Sin(verticalShootAngle * Mathf.Deg2Rad);  // vertical force

        force.z = horizontalPower * Mathf.Cos(horizontalShootAngle * Mathf.Deg2Rad);  // forward force
        force.x = horizontalPower * Mathf.Sin(horizontalShootAngle * Mathf.Deg2Rad);  // side force

        projectile.gameObject.GetComponent<Rigidbody>().AddForce(force);
    }

    bool UpdateShootAngle()
    {
        // Returns true if there is an enemy
        return true;
    }

    public void UpdateLogic()
    {
        bool isEnemy = UpdateShootAngle();
        if (isEnemy && currentRechargingTime >= data.attackSpeed) {
            currentRechargingTime = 0f;

            Shoot();
        }
    }
}
