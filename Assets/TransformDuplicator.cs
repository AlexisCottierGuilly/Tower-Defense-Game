using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class TransformDuplicator : MonoBehaviour
{
    public RectTransform target;
    public Vector2 positionOffset;

    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        rectTransform.anchoredPosition = target.anchoredPosition + positionOffset;
        rectTransform.rotation = target.rotation;
        rectTransform.localScale = target.localScale;
    }
}
