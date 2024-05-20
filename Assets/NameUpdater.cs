using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameUpdater : MonoBehaviour
{
    void Start()
    {
        GameManager.instance.loadSavedGame = false;
    }
    
    public void UpdateNameFormat()
    {
        GameManager.instance.UpdateGameName();
    }
}
