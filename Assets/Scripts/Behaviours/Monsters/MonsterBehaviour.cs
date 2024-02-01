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
    
    // Start is called before the first frame update
    void Start()
    {
        agent.speed = data.speed * 4f;
    }

    public void UpdateObjective()
    {
        Vector3 objective = gameGenerator.villageGenerator.mainVillage.transform.position;
        float minDistance = Vector3.Distance(objective, transform.position) * 2f;

        foreach (GameObject building in gameGenerator.villageGenerator.villageBuildings)
        {
            float dist = Vector3.Distance(building.transform.position, transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                objective = building.transform.position;
            }
        }

        agent.destination = objective;
    }

    public void OnTriggerStay(Collider other)
    {
        float attackCoolDown = 1f / data.attackSpeed;
        
        if (timeFromPreviousAttack >= attackCoolDown && other.gameObject.CompareTag("Village Building")) {
            VillageBehaviour villageBehaviour = other.gameObject.GetComponent<VillageBehaviour>();
            villageBehaviour.TakeDamage(data.damage);
            Debug.Log($"Tower took {data.damage} damage.", other.gameObject);
            timeFromPreviousAttack = 0f;
        }
    }

    void Update()
    {
        timeFromPreviousAttack += Time.deltaTime;
    }
}
