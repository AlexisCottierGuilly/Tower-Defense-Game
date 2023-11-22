using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DetectSelection : MonoBehaviour
{
    public Camera mainCamera;
    public GameObject placedObjectPrefab;
    public TerrainGenerator terrainGenerator;
    
    [HideInInspector] public bool isPlaced = false;
    [HideInInspector] public GameObject placedObject;
    private Vector3 currentRotation = Vector3.zero;

    void SetStructure(GameObject structurePrefab)
    {
        placedObjectPrefab = structurePrefab;
        currentRotation = Vector3.zero;
        isPlaced = false;

        Start();
    }

    void UnsetStructure()
    {
        placedObjectPrefab = null;
        currentRotation = Vector3.zero;
        isPlaced = false;
    }
    
    void Start()
    {
        if (placedObjectPrefab != null)
        {
            placedObject = Instantiate(placedObjectPrefab);
            placedObject.transform.eulerAngles = currentRotation;
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        if (placedObjectPrefab != null)
            if (!isPlaced)
                UpdatePlacementPosition();
            else
            {
                placedObject = Instantiate(placedObjectPrefab);
                placedObject.transform.eulerAngles = currentRotation;
                currentRotation = Vector3.zero;
                isPlaced = false;
            }
        
            if (Input.GetMouseButtonDown(0))
                isPlaced = true;
            
            if (Input.GetKeyDown(KeyCode.R))
            {
                currentRotation.y += 90;
                placedObject.transform.eulerAngles = currentRotation;
            }
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
        placedObject.transform.eulerAngles = currentRotation;
    }
}
