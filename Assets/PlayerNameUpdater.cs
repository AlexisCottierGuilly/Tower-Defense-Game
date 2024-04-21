using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerNameUpdater : MonoBehaviour
{
    public TextMeshProUGUI text;

    void Update()
    {
        text.text = GameManager.instance.player.name;
    }
}
