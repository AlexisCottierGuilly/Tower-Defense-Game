using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopEasterEggsManager : MonoBehaviour
{
    public GameObject gemsReward;
    public string writtenLetters = "";

    void Update()
    {
        // add letters to the string
        List<KeyCode> keyCodes = new List<KeyCode> { KeyCode.N, KeyCode.O, KeyCode.I, KeyCode.S, KeyCode.E };
        foreach (KeyCode keyCode in keyCodes)
        {
            if (Input.GetKeyDown(keyCode))
            {
                writtenLetters += keyCode.ToString().ToLower();
            }
        }
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (writtenLetters.Length > 0)
            {
                writtenLetters = writtenLetters.Substring(0, writtenLetters.Length - 1);
            }
        }

        if (!GameManager.instance.player.gemsInShop)
        {
            if (writtenLetters.Contains("noise"))
            {
                gemsReward.GetComponent<Animator>().SetTrigger("Show");
                GameManager.instance.player.gemsInShop = true;
                GameManager.instance.player.crystals += 10;
            }
        }
    }
}
