using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoSkipRoundUpdater : MonoBehaviour
{
    public GameGenerator gameGenerator;
    public Toggle toggle;

    void Start()
    {
        DidChangeValue();
    }

    public void DidChangeValue()
    {
        gameGenerator.waveManager.autoStart = toggle.isOn;
    }
}
