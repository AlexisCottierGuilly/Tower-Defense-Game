using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class VillageTowerPanel : MonoBehaviour
{
    public TowerType towerType;
    public TextMeshProUGUI title;
    public TextMeshProUGUI priceText;
    public Toggle equippedToggle;
    public Image icon;
    public Image buyBackground;
    public Button buyButton;

    public void TryToBuy()
    {
        // if the player has enough crystals, buy the tower

        // the type is stored in the towerType
        
        // if the player can buy, set interactable to false for the buy button and set interactable for the equipped toggle to true

        TowerPrefab towerPrefab = GameManager.instance.TowerPrefabFromType(towerType);
        int cost = towerPrefab.data.shopCost;

        if (GameManager.instance.player.crystals >= cost)
        {
            GameManager.instance.player.crystals -= cost;
            GameManager.instance.player.villageTowers.Add(towerType);

            buyBackground.color = Color.grey;
            buyButton.interactable = false;
            equippedToggle.interactable = true;

            equippedToggle.isOn = true;
        }
    }
}
