using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonNoiseMaker : MonoBehaviour
{
    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(PlaySound);
    }

    void PlaySound()
    {
        GameManager.instance.audioSource.clip = GameManager.instance.clickSound;
        GameManager.instance.audioSource.Play();
    }
}
