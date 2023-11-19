using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DetectSelection : MonoBehaviour
{
    public Camera mainCamera;
    public GameObject placedObject;
    public TerrainGenerator terrainGenerator;
    
    [HideInInspector] public bool isPlaced = false;

    // Update is called once per frame
    void Update()
    {
        if (!isPlaced)
            UpdatePlacementPosition();
        else
        {
            placedObject = Instantiate(placedObject);
            isPlaced = false;
        }
        
        if (Input.GetMouseButtonDown(0))
            isPlaced = true;
    }

    void UpdatePlacementPosition()
    {
        GameObject selectedTile = null;
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject.tag == "Tile")
            {
                selectedTile = hit.collider.gameObject;
            }
        }

        if (selectedTile != null)
        {
            Vector3 position = selectedTile.transform.position;
            position.y += 1;
            placedObject.transform.position = position;
        }
    }
}
