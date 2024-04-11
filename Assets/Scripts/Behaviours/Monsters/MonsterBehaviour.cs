using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterBehaviour : MonoBehaviour
{
    public MonsterData data;
    public int health;
    public GameGenerator gameGenerator = null;
    
    [Header("Space")]
    public NavMeshAgent agent;
    public Vector3 finalScale = new Vector3(1f, 1f, 1f);

    private float timeFromPreviousAttack = 0f;
    private GameObject targetTower = null;
    
    // Start is called before the first frame update
    void Start()
    {
        agent.speed = data.speed * 4f;
        health = data.maxHealth;
    }

    public void UpdateObjective()
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

        agent.destination = objective;
        targetTower = objectiveObject;
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

    public void TakeDamage(GameObject projectile)
    {
        health -= projectile.GetComponent<ProjectileBehaviour>().data.impactDamage;
        CheckDeath();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Projectile"))
        {
            TakeDamage(other.gameObject);
        }
    }

    void CheckDeath()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
            GameManager.instance.gold += data.gold;
        }

    }

    void Update()
    {
        timeFromPreviousAttack += Time.deltaTime;

        /*if (agent.pathStatus == NavMeshPathStatus.PathComplete && targetTower != null)
        {
            // Debug.Log("Arrived to objective !", this.gameObject);
            AttackStructure(targetTower);
        }*/
    }
}
