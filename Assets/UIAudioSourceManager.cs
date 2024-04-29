using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAudioSourceManager : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
