using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TowerTargetHandler : MonoBehaviour
{
    public TargetType target = TargetType.Proche;
    public TextMeshProUGUI text;

    [HideInInspector] public TowerBehaviour tower;

    public void Start()
    {
        if (tower != null)
            target = tower.targetType;
        UpdateText();
    }

    public void OnClick()
    {
        switch (target)
        {
            case TargetType.Proche:
                target = TargetType.Loin;
                break;
            case TargetType.Loin:
                target = TargetType.Fort;
                break;
            case TargetType.Fort:
                target = TargetType.Faible;
                break;
            case TargetType.Faible:
                target = TargetType.Proche;
                break;
        }

        UpdateText();

        if (tower != null)
            tower.targetType = target;
    }

    public void UpdateText()
    {
        text.text = target.ToString();
    }
}
