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

    public bool resetTimeScale = true;

    void Start()
    {
        if (resetTimeScale)
            Time.timeScale = 1f;
    }
    
    public void ChangeScene(string sceneName)
    {
        string[] sceneNameSplit = sceneName.Split('/');
        sceneName = sceneNameSplit[0];
        bool additive = false;
        bool unload = false;
        if (sceneNameSplit.Length > 1 && sceneNameSplit[1] == "additive")
            additive = true;
        if (sceneNameSplit.Length > 2 && sceneNameSplit[2] == "unload")
            unload = true;
        
        GameState scene = (GameState)System.Enum.Parse(typeof(GameState), sceneName);
        GameManager.instance.SwitchScene(scene, additive, unload);
    }

    public void Leave()
    {
#if UNITY_EDITOR
        if (UnityEditor.EditorApplication.isPlaying == true)
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }
#else
        Application.Quit();
   
#endif
    }
}
