using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tutotUI : MonoBehaviour
{
    public GameObject loreText;
    public GameObject info1;
    public GameObject info2;
    public GameObject info3;
    public GameObject info4;
    public GameObject info5;
    public GameObject info6;
    public GameObject oneUI;
    public GameObject twoUI;
    public GameObject threeUI;
    public GameObject QUI;
    private bool one = false;
    private bool two = false;
    private bool three = false;
    private bool Q = false;
    private int state = 0;

    void Start()
    {
        info1.SetActive(false);
        info2.SetActive(false);
        info3.SetActive(false);
        info4.SetActive(false);
        info5.SetActive(false);
        info6.SetActive(false);
        oneUI.SetActive(false);
        twoUI.SetActive(false);
        threeUI.SetActive(false);
        QUI.SetActive(false);
    }
    

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && state != 5)
        {
            state += 1;
            if (state == 1)
            {
                info1.SetActive(true);
                loreText.SetActive(false);
            }
            else if (state == 2)
            {
                info2.SetActive(true);
            }
            else if (state == 3)
            {
                info3.SetActive(true);
            }
            else if (state == 4)
            {
                info4.SetActive(true);
            }
            else if (state == 6)
            {
                info6.SetActive(true);
            }
        }
        else if (state == 5)
        {
            info5.SetActive(true);
            oneUI.SetActive(true);
            twoUI.SetActive(true);
            threeUI.SetActive(true);
            QUI.SetActive(true);
            if (Q == true && one == true && two == true && three == true)
            {
                state += 1;
                info6.SetActive(true);
            }
            else if (Input.GetKeyDown(KeyCode.Q))
            {
                Q = true;
                QUI.SetActive(false);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                one = true;
                oneUI.SetActive(false);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                two = true;
                twoUI.SetActive(false);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                three = true;
                threeUI.SetActive(false);
            }
        }
    }
}
