using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class safe : MonoBehaviour
{
    void Start()
    {
        if (0 >= Time.time)
        {
            SceneManager.LoadScene("Menu");
        }
    }
}
