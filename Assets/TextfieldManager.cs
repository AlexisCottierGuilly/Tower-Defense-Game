using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextfieldManager : MonoBehaviour
{
    public TMP_InputField inputField;

    public string CheckTextFormat(string text)
    {
        // the string should be all numbers and under 9 999 999
        string newText = "";
        foreach (char c in text)
        {
            if ("0123456789".Contains(c))
                newText += c;
        }

        if (newText == "")
            newText = "-1";
        else if (int.Parse(newText) > GameManager.instance.maxGameSeed)
            newText = GameManager.instance.maxGameSeed.ToString();
        
        return newText;
    }
    
    public void UpdateSeed()
    {
        string seed = CheckTextFormat(inputField.text);
        string newText = seed == "-1" ? "" : seed;
        inputField.text = newText;

        GameManager.instance.gameSeed = int.Parse(seed);
    }
}
