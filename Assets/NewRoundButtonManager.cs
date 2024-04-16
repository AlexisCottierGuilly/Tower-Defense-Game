using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewRoundButtonManager : MonoBehaviour
{
    public WaveManager waveManager;

    public void Update()
    {
        if (waveManager.waveFinished)
        {
            gameObject.GetComponent<Animator>().SetBool("New Round", true);
        }
        else
        {
            gameObject.GetComponent<Animator>().SetBool("New Round", false);
        }

        if (waveManager.wave == waveManager.waves.Count)
        {
            gameObject.GetComponent<Button>().onClick.Invoke();
        }
    }
}
