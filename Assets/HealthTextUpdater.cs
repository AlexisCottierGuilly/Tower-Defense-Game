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

        float deltaX = gameObject.transform.position.x - camera.transform.position.x;
        float deltaZ = gameObject.transform.position.z - camera.transform.position.z;

        float deltaHorizontal = Mathf.Sqrt(
            Mathf.Pow(deltaX, 2f) +
            Mathf.Pow(deltaZ, 2f)
        );

        float deltaY = gameObject.transform.position.y - camera.transform.position.y;

        float angle = Mathf.Atan(Mathf.Abs(deltaY) / deltaHorizontal) * Mathf.Rad2Deg;

        text.transform.eulerAngles = new Vector3(
            angle,
            text.transform.eulerAngles.y + 180f,
            text.transform.eulerAngles.z
        );

        text.color = new Color(1f-village.health/village.data.maxHealth, village.health/village.data.maxHealth, 0f, 1f);
        //text.transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(text.transform.position, camera.transform.position, 10000f, 0f)+new Vector3(0,0,180));
       
    }
}
