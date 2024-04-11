using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tutotUI : MonoBehaviour
{
    public GameObject info1;
    public GameObject info2;
    public GameObject info3;
    public GameObject info4;
    public GameObject info5;
    public GameObject info6;
    private int state = 1;

    void Start()
    {
        info2.SetActive(false);
        info3.SetActive(false);
        info4.SetActive(false);
        info5.SetActive(false);
        info6.SetActive(false);
    }

    void Update()
    {
        if (state == 1 && Input.GetKeyDown(KeyCode.Return))
        {
            info2.SetActive(true);
            state += 1;
        }

    }
}
