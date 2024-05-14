using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public GameObject villageTowerPanelPlaceholder;
    public GameObject scrollParent;

    private List<Toggle> equippedToggles = new List<Toggle>();
    private Dictionary<TowerType, VillageTowerPanel> villageTowerPanels = new Dictionary<TowerType, VillageTowerPanel>();

    void Start()
    {
        LoadVillageTowers();
    }

    void LoadVillageTowers()
    {
        // find all the tower types the player has
        // for each tower type, load the village tower

        foreach (TowerType towerType in GameManager.instance.player.unlockedTowers)
        {
            LoadVillageTower(towerType);
        }
    }

    void LoadVillageTower(TowerType towerType)
    {
        GameObject villageTowerPanel = Instantiate(villageTowerPanelPlaceholder, scrollParent.transform);
        villageTowerPanel.SetActive(true);

        VillageTowerPanel vtp = villageTowerPanel.GetComponent<VillageTowerPanel>();
        TowerPrefab towerPrefab = GameManager.instance.TowerPrefabFromType(towerType);
        bool alreadyBought = GameManager.instance.player.villageTowers.Contains(towerType);

        vtp.towerType = towerType;
        vtp.title.text = towerPrefab.data.name.ToString();
        vtp.priceText.text = towerPrefab.data.shopCost.ToString();

        Texture2D tex = towerPrefab.icon;
        vtp.icon.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));

        vtp.equippedToggle.isOn = GameManager.instance.player.villageTower == towerType;

        if (alreadyBought)
        {
            vtp.buyBackground.color = Color.grey;
            vtp.buyButton.interactable = false;
        }
        else
        {
            vtp.equippedToggle.interactable = false;
        }

        equippedToggles.Add(vtp.equippedToggle);
        vtp.equippedToggle.onValueChanged.AddListener((bool value) => DidToggleEquipped(towerType));

        villageTowerPanels.Add(towerType, vtp);
    }

    void DidToggleEquipped(TowerType towerType)
    {
        Toggle currenToggle = villageTowerPanels[towerType].equippedToggle;

        if (!currenToggle.isOn)
        {
            GameManager.instance.player.villageTower = TowerType.None;
            return;
        }
        
        foreach (Toggle toggle in equippedToggles)
        {
            if (toggle.isOn && toggle != currenToggle)
            {
                toggle.isOn = false;
            }
        }

        GameManager.instance.player.villageTower = towerType;
    }
}
