using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tower_selector : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown("1")) 
        {
            Debug.Log(1);
        } 
        if (Input.GetKeyDown("2")) 
        {
            Debug.Log(2);
        } 
        if (Input.GetKeyDown("3")) 
        {
            Debug.Log(3);
        } 
    }
}
