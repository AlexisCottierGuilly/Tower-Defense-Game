using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetHeightFromChildren : MonoBehaviour
{
    public float spacing = 0;
    public float additionnalHeight = 0;
    
    void Start()
    {
        SetHeight();
    }

    void Update()
    {
        SetHeight();
    }

    private void SetHeight()
    {
        // set sizeDelta of RectTransform to the total height of all children

        float height = additionnalHeight;
        int childrenCount = 0;

        foreach (Transform child in transform)
        {
            height += child.GetComponent<RectTransform>().rect.height;
            childrenCount++;
        }

        height += (childrenCount - 1) * spacing;

        GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x, height);
    }
}
