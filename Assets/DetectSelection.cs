using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DetectSelection : MonoBehaviour
{
    public Camera mainCamera;
    public GameObject placedObjectPrefab;
    public TerrainGenerator terrainGenerator;

    [HideInInspector] public bool canPlace = false;
    [HideInInspector] public GameObject placedObject;
    private Vector3 currentRotation = Vector3.zero;

    void SetStructure(GameObject structurePrefab)
    {
        placedObjectPrefab = structurePrefab;
        currentRotation = Vector3.zero;
        canPlace = true;

        Start();
    }

    void UnsetStructure()
    {
        placedObjectPrefab = null;
        currentRotation = Vector3.zero;
        canPlace = false;
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
            UpdatePlacementPosition();
        
            if (Input.GetMouseButtonDown(0) && canPlace)
            {
                GameObject newObject = Instantiate(placedObjectPrefab, placedObject.transform.position, Quaternion.identity);
                newObject.transform.eulerAngles = currentRotation;

                currentRotation = Vector3.zero;
                placedObject.transform.eulerAngles = currentRotation;
            }
            
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
            Vector2 gridPosition = selectedTile.GetComponent<TileBehaviour>().position;
            canPlace = terrainGenerator.CanPlace(gridPosition);
            Color color = Color.green;

            if (!canPlace)
                color = Color.red;
            
            placedObject.GetComponent<Renderer>().material.SetColor("_Color", color);
            foreach (Renderer child in placedObject.GetComponentsInChildren<Renderer>())
                child.material.SetColor("_Color", color);

            position.y += 1;
            placedObject.transform.position = position;
        }

        placedObject.transform.eulerAngles = currentRotation;
    }
}
