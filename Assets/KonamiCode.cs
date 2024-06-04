using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KonamiCode : MonoBehaviour
{
    private int position = 0;
    private bool done = false;

    void Start()
    {
        
    }

    void Update()
    {
        List<KeyCode> allUsedKeys = new List<KeyCode>() {KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.A, KeyCode.B};
        
        if (!done)
        {
            if (position == 10)
            {
                done = true;
                Debug.Log('W');
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (position == 0)
                {
                    position += 1;
                }
                else if (position == 1)
                {
                    position += 1;
                }
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (position == 2)
                {
                    position += 1;
                }
                else if (position == 3)
                {
                    position += 1;
                }
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (position == 4)
                {
                    position += 1;
                }
                else if (position == 6)
                {
                    position += 1;
                }
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (position == 5)
                {
                    position += 1;
                }
                else if (position == 7)
                {
                    position += 1;
                }
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                if (position == 9)
                {
                    position += 1;
                }
            }
            else if (Input.GetKeyDown(KeyCode.B))
            {
                if (position == 8)
                {
                    position += 1;
                }
            }
            else if (Input.anyKey)
            {
                bool isKey = false;

                foreach (KeyCode k in allUsedKeys)
                {
                    if (Input.GetKey(k))
                    {
                        isKey = true;
                        break;
                    }
                }

                if (!isKey)
                {
                    position = 0;
                }
            }
        }
    }
}
