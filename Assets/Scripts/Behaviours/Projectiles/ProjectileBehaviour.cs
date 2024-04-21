using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    public ProjectileData data;
    [HideInInspector] public GameObject sender;
    [HideInInspector] public GameObject target;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public int GetDamage()
    {
        int damage = data.impactDamage;

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

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 1f, 1f, 0.5f);
        Gizmos.DrawWireSphere(gameObject.transform.position, data.hitRadius);
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < -10f)
        {
            Destroy(gameObject);
        }

        UpdateHits();

        if (target != null && data.followTarget)
        {
            CorrectTrajectory();
        }
    }

    void UpdateHits()
    {
        Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, data.hitRadius);

        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.CompareTag("Monster") && !data.isEnemy)
            {
                MonsterBehaviour monster = collider.gameObject.GetComponent<MonsterBehaviour>();
                monster.ProjectileHit(gameObject);

                Die();
            }
            else if (collider.gameObject.CompareTag("Tile"))
            {
                Die();
            }
            else if (collider.gameObject.CompareTag("Village Building") && data.isEnemy)
            {
                VillageBehaviour village = collider.gameObject.GetComponent<VillageBehaviour>();
                village.ProjectileHit(gameObject);

                Die();
            }
        }
    }

    void CorrectTrajectory()
    {
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        
        float deltaX = target.transform.position.x - gameObject.transform.position.x;
        float deltaY = target.transform.position.y - gameObject.transform.position.y;
        float deltaZ = target.transform.position.z - gameObject.transform.position.z;

        float currentSpeedX = rb.velocity.x;
        float currentSpeedY = rb.velocity.y;
        float currentSpeedZ = rb.velocity.z;

        float correctionX = deltaX - currentSpeedX;
        float correctionY = deltaY - currentSpeedY;
        float correctionZ = deltaZ - currentSpeedZ;

        float divisor = 5f;

        Vector3 correctionForce = new Vector3(
            correctionX / divisor,
            correctionY / divisor,
            correctionZ / divisor
        );

        if (correctionForce.x * currentSpeedX < 0f)  // if the forces are not the same sign (+-)
            correctionForce.x = 0f;
        //if (correctionForce.y * currentSpeedY < 0f)
        //    correctionForce.y = 0f;
        if (correctionForce.z * currentSpeedZ < 0f)
            correctionForce.z = 0f;

        rb.AddForce(correctionForce);
    }

    public void SpawnZone()
    {
        if (data.impactZone != null)
        {
            GameObject zone = Instantiate(data.impactZone, gameObject.transform.position, Quaternion.identity);
            zone.transform.eulerAngles = new Vector3(
                zone.transform.eulerAngles.x - 90f,
                zone.transform.eulerAngles.y,
                zone.transform.eulerAngles.z
            );

            ZoneBehaviour behaviour = zone.GetComponent<ZoneBehaviour>();

            behaviour.sender = sender;
            float radius = behaviour.data.radius / 5f;

            zone.transform.localScale = new Vector3(
                radius,
                radius,

                radius // height
            );

            // var zoneShape = zone.GetComponent<ParticleSystem>().shape;
            // zoneShape.radius = radius;
        }
    }
    
    public void SetTarget(GameObject target)
    {
        this.target = target;
    }

    public void Die()
    {
        SpawnZone();
        Destroy(gameObject);
    }
}
