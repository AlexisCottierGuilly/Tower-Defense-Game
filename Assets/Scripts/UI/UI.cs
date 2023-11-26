using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour
{
    /*
    Pour créer des fonts custom :
        1 - Télécharger un fichier .ttf
        2 - L'ajouter dans le dossier Assets\CustomFonts
        3 - Dans Unity, aller dans Window > TextMeshPro > Font Asset Creator
        4 - Glisser le fichier .ttf dans la case "Font Source"
        5 - Appuyer sur Create Font Atlas
        6 - Cliquer sur "Save" et enregistrer le fichier dans le dossier Assets\CustomFonts
    */
    
    public void ChangeScene(string sceneName)
    {
        Debug.Log("Changement de scène vers " + sceneName);
        GameManager.instance.SwitchScene(sceneName);
    }

    public void Leave()
    {
        Debug.Log("player left");
        UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }
}
