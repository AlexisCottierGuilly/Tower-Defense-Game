using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowRangeManager : MonoBehaviour
{
    public GameObject rangeSpherePrefab;
    public Material valid;
    public Material invalid;

    private GameObject rangeSphere;

    private void Start()
    {
        rangeSphere = Instantiate(rangeSpherePrefab, Vector3.zero, Quaternion.identity);
        rangeSphere.SetActive(false);
    }

    public void SetRange(GameObject tower, bool valid = true)
    {
        TowerBehaviour towerBehaviour = tower.GetComponent<TowerBehaviour>();
        if (towerBehaviour == null)
            return;

        float range = towerBehaviour.data.maxRange;

        rangeSphere.transform.localScale = new Vector3(range * 2f, 0.1f, range * 2f);
        rangeSphere.transform.position = tower.transform.position;

        if (valid)
            rangeSphere.GetComponent<MeshRenderer>().material = this.valid;
        else
            rangeSphere.GetComponent<MeshRenderer>().material = invalid;

        rangeSphere.SetActive(true);
    }

    public void UnsetRange()
    {
        rangeSphere.SetActive(false);
    }
}
