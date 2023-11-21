using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public Button button;
    /*
    Pour créer des fonts custom :
        1 - Télécharger un fichier .ttf
        2 - L'ajouter dans le dossier Assets\CustomFonts
        3 - Dans Unity, aller dans Window > TextMeshPro > Font Asset Creator
        4 - Glisser le fichier .ttf dans la case "Font Source"
        5 - Appuyer sur Create Font Atlas
        6 - Cliquer sur "Save" et enregistrer le fichier dans le dossier Assets\CustomFonts
    */
    
    void Awake()
    {
        button.onClick.AddListener(OnClick);
    }
    
    public void OnClick()
    {
        print("Start");
    }
}