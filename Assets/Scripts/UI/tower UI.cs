using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


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
    private List<TowerType> unlockedTowers = new List<TowerType>();

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
        foreach (TowerPrefab prefab in GameManager.instance.towerPrefabs)
        {
            if (GameManager.instance.player.unlockedTowers.Contains(prefab.tower))
                AddButton(prefab);
        }
    }

    GameObject AddButton(TowerPrefab prefab)
    {
        GameObject go = Instantiate(towerButtonPlaceHolder, towerButtonPlaceHolder.transform);
        go.SetActive(true);
        TowerPlaceHolder towerPlaceHolder = go.GetComponent<TowerPlaceHolder>();
        towerPlaceHolder.icon.sprite = Sprite.Create(prefab.icon, new Rect(0, 0, prefab.icon.width, prefab.icon.height), new Vector2(0.5f, 0.5f));

        TowerData data = GameManager.instance.GetTowerPrefab(prefab.tower).GetComponent<TowerBehaviour>().data;

        towerPlaceHolder.text.GetComponent<TextMeshProUGUI>().text = data.cost.ToString();
        towerPlaceHolder.text.GetComponent<TowerCostUpdater>().data = data;

        towerPlaceHolder.title.GetComponent<TextMeshProUGUI>().text = data.name;

        towerPlaceHolder.button.onClick.AddListener(() => ChangeTower(prefab.tower.ToString()));
        go.transform.parent = buttonsParent.transform;

        towerPlaceHolder.keyText.text = $"{unlockedTowers.Count + 1}";

        unlockedTowers.Add(prefab.tower);

        return go;
    }
    
    public void ChangeTower(string newTower)
    {
        type = (TowerType)System.Enum.Parse(typeof(TowerType), newTower);
        GameObject prefab = GameManager.instance.GetTowerPrefab(type);
        if (type != null && prefab != null)
        {
            selectionManager.SetStructure(prefab);
        }
    }

    void HandleInputs()
    {
        int i = 0;
        foreach (TowerType t in unlockedTowers)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                ChangeTower(t.ToString());
            }

            i++;
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
