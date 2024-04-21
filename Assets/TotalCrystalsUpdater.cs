using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TotalCrystalsUpdater : MonoBehaviour
{
    public TextMeshProUGUI text;

    void Update()
    {
        text.text = GameManager.instance.player.crystals.ToString();
    }
}
