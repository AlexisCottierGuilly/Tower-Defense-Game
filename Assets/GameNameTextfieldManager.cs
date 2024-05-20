using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_InputField))]
public class GameNameTextfieldManager : MonoBehaviour
{
    public TextMeshProUGUI placeholderText;
    
    private TMP_InputField inputField;

    private void Awake()
    {
        inputField = GetComponent<TMP_InputField>();
    }

    void Start()
    {
        inputField.onValueChanged.AddListener(UpdateGameName);
        placeholderText.text = GameManager.instance.gameName;
    }

    public void UpdateGameName(string text)
    {
        GameManager.instance.gameName = text;
    }
}
