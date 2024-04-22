using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum TowerType
{
    None,
    Normal,
    Splash,
    Ultimate
}


public class TowerUI : MonoBehaviour
{
    public GameGenerator gameGenerator;
    [Space]
    public TowerType type = TowerType.None;
    public List<TowerButton> towerButtons = new List<TowerButton>();
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
        foreach (TowerButton button in towerButtons)
        {
            GameObject go = AddButton(button);

        }
    }

    GameObject AddButton(TowerButton button)
    {
        GameObject go = Instantiate(towerButtonPlaceHolder, transform);
        go.SetActive(true);
        TowerPlaceHolder towerPlaceHolder = go.GetComponent<TowerPlaceHolder>();
        towerPlaceHolder.icon.sprite = Sprite.Create(button.icon, new Rect(0, 0, button.icon.width, button.icon.height), new Vector2(0.5f, 0.5f));
        
        TowerData data = gameGenerator.GetTowerPrefab(button.type).GetComponent<TowerBehaviour>().data;
        
        towerPlaceHolder.text.GetComponent<TextMeshProUGUI>().text = data.cost.ToString();
        towerPlaceHolder.text.GetComponent<TowerCostUpdater>().data = data;
        
        towerPlaceHolder.button.onClick.AddListener(() => ChangeTower(button.type.ToString()));
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
        foreach (TowerButton button in towerButtons)
        {
            if (Input.GetKeyDown(button.key))
            {
                ChangeTower(button.type.ToString());
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


[System.Serializable]
public class TowerButton
{
    public TowerType type;
    public Texture2D icon;
    public KeyCode key = KeyCode.Alpha1;
}
