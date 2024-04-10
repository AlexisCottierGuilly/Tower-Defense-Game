using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HealthTextUpdater : MonoBehaviour
{
    public TextMeshPro text;
    public VillageBehaviour village;
    public GameObject camera;

    void Update()
    {
        text.text = $"{village.health} / {village.data.maxHealth}";
        text.transform.LookAt(camera.transform);
        text.transform.eulerAngles += new Vector3(0,180,0);
        text.color = new Color(1f-village.health/village.data.maxHealth, village.health/village.data.maxHealth, 0f, 1f);
        //text.transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(text.transform.position, camera.transform.position, 10000f, 0f)+new Vector3(0,0,180));
       
    }
}
