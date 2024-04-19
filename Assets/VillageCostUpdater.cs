using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VillageCostUpdater : MonoBehaviour
{
    public VillageData data;
    public TextMeshProUGUI text;

    void Start()
    {
        text.text = $"{data.cost}";
    }
}
