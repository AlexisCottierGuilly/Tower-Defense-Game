using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MonsterHealthUpdater : MonoBehaviour
{
    public MonsterBehaviour monster;
    public GameObject camera;
    public TextMeshPro text;

    void Update()
    {
        text.text = $"{monster.health} / {monster.data.maxHealth}";
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

        float percentage = (float)monster.health / (float)monster.data.maxHealth;
        text.color = Color.Lerp(Color.red, Color.green, percentage);
    }
}
