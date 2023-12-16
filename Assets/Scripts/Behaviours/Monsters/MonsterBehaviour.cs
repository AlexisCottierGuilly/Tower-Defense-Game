using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterBehaviour : MonoBehaviour
{
    public MonsterData data;
    public int health;
    
    [Header("Space")]
    public NavMeshAgent agent;
    public Vector3 finalScale = new Vector3(1f, 1f, 1f);
    
    // Start is called before the first frame update
    void Start()
    {
        agent.speed = data.speed * 4f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
