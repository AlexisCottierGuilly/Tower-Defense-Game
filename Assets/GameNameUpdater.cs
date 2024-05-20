using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameNameUpdater : MonoBehaviour
{
    private TextMeshProUGUI text;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        text.text = GameManager.instance.gameName;
    }
}
