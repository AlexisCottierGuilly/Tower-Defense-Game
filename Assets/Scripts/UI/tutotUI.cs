using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class tutotUI : MonoBehaviour
{
    public GameObject loreText;
    public GameObject info1;
    public GameObject info2;
    public GameObject info3;
    public GameObject info4;
    public GameObject info5;
    public GameObject info6;
    public GameObject VUI;
    public GameObject oneUI;
    public GameObject twoUI;
    public GameObject threeUI;
    public GameObject QUI;
    public GameObject ShiftUI;
    public GameObject SpaceUI;
    private bool V = false;
    private bool one = false;
    private bool two = false;
    private bool three = false;
    private bool Q = false;
    private bool shift = false;
    private bool space = false;
    private int state = 0;

    void Start()
    {
        info1.SetActive(false);
        info2.SetActive(false);
        info3.SetActive(false);
        info4.SetActive(false);
        info5.SetActive(false);
        info6.SetActive(false);
    }
    

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && state != 5  && state != 6 && state != 2 && state != 7)
        {
            state += 1;
            if (state == 1)
            {
                info1.SetActive(true);
                loreText.SetActive(false);
            }
            else if (state == 3)
            {
                info3.SetActive(true);
            }
            else if (state == 4)
            {
                info4.SetActive(true);
            }
        }
        else if (state == 2)
        {
            info2.SetActive(true);
            if (V == false)
                VUI.SetActive(true);
            
            if (V == true)
            {
                state += 1;
                info3.SetActive(true);
            }
            else if (Input.GetKeyDown(KeyCode.V))
            {
                V = true;
                VUI.GetComponent<Image>().color = Color.green;
            }
        }
        else if (state == 5)
        {
            info5.SetActive(true);
            if (one == false)
                oneUI.SetActive(true);
            if (two == false)
                twoUI.SetActive(true);
            if (three == false)
                threeUI.SetActive(true);
            if (Q == false)
                QUI.SetActive(true);
            
            if (Q == true && one == true && two == true && three == true)
            {
                state += 1;
                info6.SetActive(true);
            }
            else if (Input.GetKeyDown(KeyCode.Q))
            {
                Q = true;
                QUI.GetComponent<Image>().color = Color.green;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                one = true;
                oneUI.GetComponent<Image>().color = Color.green;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                two = true;
                twoUI.GetComponent<Image>().color = Color.green;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                three = true;
                threeUI.GetComponent<Image>().color = Color.green;
            }
        }
        else if (state == 6)
        {
            info6.SetActive(true);
            if (shift == false)
                ShiftUI.SetActive(true);
            if (space == false)
                SpaceUI.SetActive(true);
            if (shift == true && space == true)
            {
                state += 1;
            }
            else if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                shift = true;
                ShiftUI.GetComponent<Image>().color = Color.green;
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                space = true;
                SpaceUI.GetComponent<Image>().color = Color.green;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Return) && state == 7)
        {
            
            GameManager.instance.SwitchScene(GameState.Menu);
        }
    }
}
