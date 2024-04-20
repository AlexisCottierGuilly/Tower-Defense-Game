using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using cakeslice;

public class TowerSelectionManager : MonoBehaviour
{
    public DetectSelection detectSelection;
    public Camera camera;
    public GameGenerator gameGenerator;

    [Header("Tower Stats")]
    public GameObject statsPanel;
    public TextMeshProUGUI statsTitle;
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI sellValueText;

    [Space]
    public float sellRate = 0.25f;

    private GameObject selection = null;

    void Start()
    {
        Unselect();
    }
    
    private void CheckSelection(Vector2 mouse)
    {
        // Find if the mouse is clicking on a tower
        GameObject tower = null;

        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(mouse);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject.GetComponent<TowerBehaviour>() != null)
                tower = hit.collider.gameObject;
        }

        if (tower != null)
        {
            Select(tower);
        }
        else
        {
            Unselect();
        }
    }

    void Update()
    {
        if (gameGenerator.paused)
        {
            Unselect();
            return;
        }
        
        if (detectSelection.objectType == ObjectType.None)
        {
            if (Input.GetMouseButtonUp(0))
            {
                Vector2 mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                CheckSelection(mousePosition);
            }
        }
        else
        {
            Unselect();
        }

        UpdateStats();
    }

    public void UpdateStats()
    {
        if (selection != null)
        {
            TowerBehaviour behaviour = selection.GetComponent<TowerBehaviour>();
            if (behaviour != null)
            {
                statsTitle.text = behaviour.data.name;
                damageText.text = behaviour.stats.damageDealt.ToString();
                sellValueText.text = Mathf.Round(behaviour.data.cost * sellRate).ToString();
            }
        }
    }

    public void Select(GameObject go)
    {
        Unselect();

        selection = go;
        ApplyOutline(go);

        statsPanel.SetActive(true);

        UpdateStats();
    }

    private void Unselect()
    {
        if (selection != null)
        {
            ApplyOutline(selection, true);
            selection = null;
        }

        statsPanel.SetActive(false);
    }

    public void Sell()
    {
        if (selection == null)
            return;
        
        TowerBehaviour behaviour = selection.GetComponent<TowerBehaviour>();
        
        if (behaviour != null)
        {
            int gainedGold = (int)(behaviour.data.cost * sellRate);
            GameManager.instance.gold += gainedGold;

            behaviour.Die();
        }

        Unselect();
    }

    private void ApplyOutline(GameObject go, bool remove=false)
    {
        // if not remove, add the Outline component to all children and go (only if they have a renderer on them (Mesh renderer, and all other types))
        // if remove, remove all their Outline components

        foreach (Transform child in go.transform)
        {
            ApplyOutline(child.gameObject, remove);
        }
        
        if (!remove)
        {
            if (go.GetComponent<Renderer>() != null)
            {
                Outline outlineGo = go.AddComponent<Outline>();
                outlineGo.color = 0;
            }
        }
        else
        {
            Outline outlineGo = go.GetComponent<Outline>();
            if (outlineGo != null)
                Destroy(outlineGo);
        }
    }
}
