using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class gold : MonoBehaviour
{
    private int last = GameManager.instance.gold;
    public TMP_Text Text;

    void Update()
    {
        if (GameManager.instance.gold != last)
        {
            //Text.TMP_Text = "Gold: " + GameManager.instance.gold.ToString();
            last = GameManager.instance.gold;
        }
    }
}
