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
    public TowerTargetHandler towerTargetHandler;

    [Header("Decoration Stats")]
    public GameObject decorationStatsPanel;
    public TextMeshProUGUI decorationSellValueText;

    [Space]
    public float sellRate = 0.25f;

    private GameObject selection = null;

    void Start()
    {
        Unselect();
    }
    
    private void CheckSelection(Vector2 mouse)
    {
        if (RectTransformUtility.RectangleContainsScreenPoint(statsPanel.GetComponent<RectTransform>(), mouse))
            return;
        
        // Find if the mouse is clicking on a tower
        GameObject tower = null;

        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(mouse);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject.GetComponent<TowerBehaviour>() != null)
            {
                SelectTower(hit.collider.gameObject);
            }
            else if (hit.collider.gameObject.GetComponent<DecorationBehaviour>() != null)
            {
                SelectDecoration(hit.collider.gameObject);
            }
            else
                Unselect();
        }
        else
            Unselect();
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

        UpdateTowerStats();
    }

    public void UpdateTowerStats()
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

    public void UpdateDecorationStats()
    {
        if (selection != null)
        {
            DecorationBehaviour behaviour = selection.GetComponent<DecorationBehaviour>();
            if (behaviour != null)
            {
                decorationSellValueText.text = behaviour.sellValue.ToString();
            }
        }
    }

    public void SelectTower(GameObject go)
    {
        Unselect();

        selection = go;
        ApplyOutline(go);
        
        towerTargetHandler.tower = go.GetComponent<TowerBehaviour>();
        towerTargetHandler.Start();

        statsPanel.SetActive(true);

        UpdateTowerStats();
    }

    public void SelectDecoration(GameObject go)
    {
        Unselect();

        selection = go;
        ApplyOutline(go);
        
        DecorationBehaviour behaviour = go.GetComponent<DecorationBehaviour>();
        decorationStatsPanel.SetActive(true);

        UpdateDecorationStats();
    }

    private void Unselect()
    {
        if (selection != null)
        {
            ApplyOutline(selection, true);
            selection = null;
        }

        towerTargetHandler.tower = null;

        statsPanel.SetActive(false);
        decorationStatsPanel.SetActive(false);
    }

    public void SellTower()
    {
        if (selection == null)
            return;
        
        TowerBehaviour behaviour = selection.GetComponent<TowerBehaviour>();
        
        if (behaviour != null)
        {
            int gainedGold = (int)(behaviour.data.cost * sellRate);
            GameManager.instance.gold += gainedGold;

            behaviour.Die();
            gameGenerator.notificationManager.ShowNotification("Bro tu viens de te faire scam.");
        }

        Unselect();
    }

    public void SellDecoration()
    {
        if (selection == null)
            return;
        
        DecorationBehaviour behaviour = selection.GetComponent<DecorationBehaviour>();
        
        if (behaviour != null)
        {
            if (GameManager.instance.gold >= behaviour.sellValue)
            {
                GameManager.instance.gold -= behaviour.sellValue;

                Destroy(selection);
                gameGenerator.notificationManager.ShowNotification("Pauvre panorama magnifique :(");
                Unselect();
            }
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
