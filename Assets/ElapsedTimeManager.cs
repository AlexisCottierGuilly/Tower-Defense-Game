using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ElapsedTimeManager : MonoBehaviour
{
    public GameGenerator generator;
    public TextMeshProUGUI text;
    public string ending = " secondes";

    void Update()
    {
        text.text = generator.gameTime.ToString("F3").Replace(",", ".") + ending;
    }
}
