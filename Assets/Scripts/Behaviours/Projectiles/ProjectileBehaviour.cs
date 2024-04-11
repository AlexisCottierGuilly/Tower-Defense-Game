using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    public ProjectileData data;
    [HideInInspector] public GameObject target;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < -10f)
        {
            Destroy(gameObject);
        }

        if (target != null && data.followTarget)
        {
            CorrectTrajectory();
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

            float radius = zone.GetComponent<ZoneBehaviour>().data.radius / 10f;

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

    public void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Tile"))
        {
            Die();
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Monster"))
        {
            target = null;
            Die();
        }
    }
}
