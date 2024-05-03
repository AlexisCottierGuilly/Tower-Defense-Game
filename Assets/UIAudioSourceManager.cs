using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAudioSourceManager : MonoBehaviour
{
    public static UIAudioSourceManager instance = null;
    
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }
}
