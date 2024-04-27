using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum TowerType
{
    None,
    Normal,
    Explosive,
    Ultimate,
    Slow
}


public class TowerUI : MonoBehaviour
{
    public GameGenerator gameGenerator;
    [Space]
    public TowerType type = TowerType.None;
    [Space]
    public GameObject towerButtonPlaceHolder;
    public GameObject buttonsParent;
    [Space]
    public GameObject selectionManagerObject;

    DetectSelection selectionManager;

    void Start()
    {
        InitializeButtons();
    }
    
    void Awake()
    {
        selectionManager = selectionManagerObject.GetComponent<DetectSelection>();
    }

    void InitializeButtons()
    {
        foreach (TowerPrefab prefab in gameGenerator.towerPrefabs)
        {
            GameObject go = AddButton(prefab);

        }
    }

    GameObject AddButton(TowerPrefab prefab)
    {
        GameObject go = Instantiate(towerButtonPlaceHolder, transform);
        go.SetActive(true);
        TowerPlaceHolder towerPlaceHolder = go.GetComponent<TowerPlaceHolder>();
        towerPlaceHolder.icon.sprite = Sprite.Create(prefab.icon, new Rect(0, 0, prefab.icon.width, prefab.icon.height), new Vector2(0.5f, 0.5f));

        TowerData data = gameGenerator.GetTowerPrefab(prefab.tower).GetComponent<TowerBehaviour>().data;

        towerPlaceHolder.text.GetComponent<TextMeshProUGUI>().text = data.cost.ToString();
        towerPlaceHolder.text.GetComponent<TowerCostUpdater>().data = data;

        towerPlaceHolder.title.GetComponent<TextMeshProUGUI>().text = data.name;

        towerPlaceHolder.button.onClick.AddListener(() => ChangeTower(prefab.tower.ToString()));
        go.transform.parent = buttonsParent.transform;

        return go;
    }
    
    public void ChangeTower(string newTower)
    {
        type = (TowerType)System.Enum.Parse(typeof(TowerType), newTower);
        GameObject prefab = gameGenerator.GetTowerPrefab(type);
        if (type != null && prefab != null)
        {
            selectionManager.SetStructure(prefab);
        }
    }

    void HandleInputs()
    {
        foreach (TowerPrefab prefab in gameGenerator.towerPrefabs)
        {
            if (Input.GetKeyDown(prefab.key))
            {
                ChangeTower(prefab.tower.ToString());
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            type = TowerType.None;
        }

        HandleInputs();
    }
}
