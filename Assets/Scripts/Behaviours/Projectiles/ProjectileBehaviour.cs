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

    public void SetTarget(GameObject target)
    {
        this.target = target;
    }

    public void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Tile"))
        {
            Destroy(gameObject);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Monster"))
        {
            Destroy(gameObject);
            target = null;
        }
    }
}
