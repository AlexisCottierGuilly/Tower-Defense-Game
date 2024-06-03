using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class ProjectileBehaviour : MonoBehaviour
{
    public ProjectileData data;

    [HideInInspector] public GameObject sender;
    [HideInInspector] public GameObject target;
    [HideInInspector] public bool targetEnemy = true;
    [HideInInspector] public bool targetVillage = true;

    private List<GameObject> decorationHits = new List<GameObject>();
    private Vector3 lastPosition;

    private float timeFromSpawn;
    private bool didRegisterLanding = false;
    
    // Start is called before the first frame update
    void Start()
    {
        if (data.sound != null)
        {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            VolumeUpdater volumeUpdater = gameObject.AddComponent<VolumeUpdater>();
            volumeUpdater.volumeMultiplier = data.volume;

            audioSource.spatialBlend = 0.75f;
            audioSource.rolloffMode = AudioRolloffMode.Linear;
            audioSource.maxDistance = 100f;
            audioSource.priority = 255;

            audioSource.clip = data.sound;
            audioSource.Play();
        }
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
        timeFromSpawn += Time.deltaTime;
        if (!didRegisterLanding && timeFromSpawn > 2.5f)
        {
            GameManager.instance.player.achievementStats.landedProjectiles += 1;
            didRegisterLanding = true;
        }
        
        if (transform.position.y < -10f)
        {
            Destroy(gameObject);
        }

        UpdateHits();

        if (target != null && data.followTarget)
        {
            CorrectTrajectory();
        }

        /*
        NOT WORKING
        if (data.angleFromGravity)
        {
            UpdateAngleFromGravity();
        } */

        lastPosition = gameObject.transform.position;
    }

    void UpdateHits()
    {
        Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, data.hitRadius);
        bool skipNext = false;

        foreach (Collider collider in colliders)
        {
            if (collider.gameObject == sender)
                continue;
            
            if (data.skipSameTypeAsSender)
            {
                MonsterBehaviour senderMonster = sender.GetComponent<MonsterBehaviour>();
                MonsterBehaviour monster = collider.gameObject.GetComponent<MonsterBehaviour>();
                if (senderMonster != null && monster != null && senderMonster.data == monster.data)
                {
                    continue;
                }
            }
            
            if (!skipNext && collider.gameObject.CompareTag("Monster") && targetEnemy)
            {
                MonsterBehaviour behaviour = collider.gameObject.GetComponent<MonsterBehaviour>();
                behaviour.ProjectileHit(gameObject);

                Die();
                skipNext = true;
            }
            else if (!skipNext && collider.gameObject.CompareTag("Village Building") && targetVillage)
            {
                VillageBehaviour village = collider.gameObject.GetComponent<VillageBehaviour>();
                village.ProjectileHit(gameObject);

                Die();
                skipNext = true;
            }
            else if (!skipNext && collider.gameObject.CompareTag("Tile"))
            {
                Die();
                skipNext = true;
            }
            
            else if (collider.gameObject.CompareTag("Decoration"))
            {
                if (!decorationHits.Contains(collider.gameObject))
                {
                    if (sender != null && sender.GetComponent<TowerBehaviour>() != null)
                    {
                        decorationHits.Add(collider.gameObject);
                        GameManager.instance.player.achievementStats.shotsOnDecorations += 1;
                    }
                }
            }
        }
    }

    void UpdateAngleFromGravity()
    {
        // use current and last position to calculate the angle that the projectile should be facing
        // calculate the x, y and z angles based on the difference between the current and last position
        
        float deltaX = gameObject.transform.position.x - lastPosition.x;
        float deltaY = gameObject.transform.position.y - lastPosition.y;
        float deltaZ = gameObject.transform.position.z - lastPosition.z;

        // only the y angle can stay the same.
        // the x and z angles need to be calculated based on the delta values
        // for the rotation to be correct, the x and z angle together should give the vertical wanted angle

        float distanceHorizontal = Mathf.Sqrt(deltaX * deltaX + deltaZ * deltaZ);
        float verticalAngle = Mathf.Atan2(deltaY, distanceHorizontal) * Mathf.Rad2Deg;

        // the projection of the x and z angles on the ground should be equal to the y angle. In addition, the x and z angles should make the vertical angle together

        float horizontalAngle = Mathf.Atan2(deltaZ, deltaX) * Mathf.Rad2Deg;

        // DOES NOT WORK :(
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
            behaviour.targetEnemy = targetEnemy;
            behaviour.targetVillage = targetVillage;

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
