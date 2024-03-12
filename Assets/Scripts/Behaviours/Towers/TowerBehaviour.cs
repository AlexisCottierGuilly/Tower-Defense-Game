using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBehaviour : StructureBehaviour
{
    public TowerData data;
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
        GameObject projectile = Instantiate(data.projectile, projectileSpawnEmpty.transform);
        
        Vector3 force = Vector3.forward;
        force.y = 0.5f; // temporaire avant de trouver les bonnes valeurs selon les angles
        projectile.gameObject.GetComponent<Rigidbody>().AddForce(force * 1000f);
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
