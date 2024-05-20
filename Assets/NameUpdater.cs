using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameUpdater : MonoBehaviour
{
    public void UpdateNameFormat()
    {
        GameManager.instance.UpdateGameName();
    }
}
