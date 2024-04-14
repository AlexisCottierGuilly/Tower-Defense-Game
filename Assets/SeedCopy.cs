using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedCopy : MonoBehaviour
{
    public GameGenerator generator;
    
    public void CopySeed()
    {
        GUIUtility.systemCopyBuffer = generator.seed.ToString();
    }
}
