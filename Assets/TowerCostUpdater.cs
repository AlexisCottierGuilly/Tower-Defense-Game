using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TowerCostUpdater : MonoBehaviour
{
    public TowerData data;
    public TextMeshProUGUI text;

    void Start()
    {
        text.text = $"{data.cost} or";
    }
}
