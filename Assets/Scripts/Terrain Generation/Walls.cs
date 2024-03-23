using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Walls : MonoBehaviour
{
    void Start()
    {
        // print(GameManager.instance.mapSize.x);
        // print(GameManager.instance.mapSize.y);
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

    }
}
