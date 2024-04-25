using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingManager : MonoBehaviour
{
    public GameGenerator gameGenerator;

    public bool Shoot(
        GameObject sender,
        GameObject target,
        bool targetEnemy,
        bool targetVillage,
        GameObject projectile,
        float verticalAngle,
        GameObject projectileParent,
        GameObject spawnEmpty
        )
    {
        float power = GetShootingForce(sender, target, verticalAngle);

        if (power != 0f)
        {
            GameObject projectileInstance = Instantiate(projectile, spawnEmpty.transform.position, Quaternion.identity);
            projectileInstance.transform.eulerAngles = new Vector3(
                90f - verticalAngle,
                sender.transform.eulerAngles.y,
                projectileInstance.transform.eulerAngles.z
            );

            ProjectileBehaviour behaviour = projectileInstance.GetComponent<ProjectileBehaviour>();

            projectileInstance.transform.parent = projectileParent.transform;
            behaviour.SetTarget(target);
            behaviour.sender = sender;
            behaviour.targetEnemy = targetEnemy;
            behaviour.targetVillage = targetVillage;

            Vector3 force = new Vector3();

            float horizontalPower = power * Mathf.Cos(verticalAngle * Mathf.Deg2Rad);
            force.y = power * Mathf.Sin(verticalAngle * Mathf.Deg2Rad);

            force.z = horizontalPower * Mathf.Cos(sender.transform.eulerAngles.y * Mathf.Deg2Rad);
            force.x = horizontalPower * Mathf.Sin(sender.transform.eulerAngles.y * Mathf.Deg2Rad);

            if (!float.IsNaN(force.x))
            {
                projectileInstance.gameObject.GetComponent<Rigidbody>().AddForce(force);
            }
            else
            {
                return false;
                Destroy(projectileInstance);
            }

            return true;
        }

        return false;
    }

    public float GetShootingForce(GameObject sender, GameObject target, float verticalAngle)
    {
        float acceleration = Physics.gravity.y;
        float angle = 25f;

        float deltaX = Mathf.Sqrt(
            Mathf.Pow(sender.transform.position.x - target.transform.position.x, 2f) +
            Mathf.Pow(sender.transform.position.z - target.transform.position.z, 2f)
        );
        float deltaY = sender.transform.position.y - target.transform.position.y;

        float angleDiffMultiplier;
        if (verticalAngle >= 25f)
            angleDiffMultiplier = Mathf.Pow(verticalAngle / angle, 1.35f);
        else
            angleDiffMultiplier = 1f * 1.025f;

        if (verticalAngle >= 25f)
            deltaY *= 17.5f / Mathf.Pow(angleDiffMultiplier, 1.25f);
        else
            deltaY *= 17.5f / (angleDiffMultiplier * 0.825f);

        float viX = deltaX / (Mathf.Sqrt((2 * (deltaX * Mathf.Tan(angle) * Mathf.Rad2Deg - deltaY)) / acceleration));
        float viY = viX * Mathf.Tan(angle) * Mathf.Rad2Deg;
        float vi = Mathf.Sqrt(Mathf.Pow(viX, 2f) + Mathf.Pow(viY, 2f));

        return vi * 9f * angleDiffMultiplier;
    }
}
