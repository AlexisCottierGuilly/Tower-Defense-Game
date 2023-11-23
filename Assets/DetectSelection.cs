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
    [HideInInspector] public Vector2 position = Vector2.zero;
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
        position = Vector2.zero;
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
            if (Input.GetMouseButtonDown(0) && canPlace)
            {
                GameObject newObject = Instantiate(
                    placedObjectPrefab,
                    placedObject.transform.position,
                    Quaternion.identity
                );
                newObject.transform.eulerAngles = currentRotation;
                if (!terrainGenerator.PlaceTower(position, newObject))
                    Destroy(newObject);

                currentRotation = Vector3.zero;
                placedObject.transform.eulerAngles = currentRotation;
            }
            
            if (Input.GetKeyDown(KeyCode.R))
            {
                currentRotation.y += 90;
                placedObject.transform.eulerAngles = currentRotation;
            }

            UpdatePlacementPosition();
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
            Vector3 worldPos = selectedTile.transform.position;
            position = selectedTile.GetComponent<TileBehaviour>().position;

            worldPos.y += 1;
            placedObject.transform.position = worldPos;
        }

        canPlace = terrainGenerator.CanPlace(position);
        Color color = Color.green;

        if (!canPlace)
            color = Color.red;
        
        placedObject.GetComponent<Renderer>().material.SetColor("_Color", color);
        foreach (Renderer child in placedObject.GetComponentsInChildren<Renderer>())
            child.material.SetColor("_Color", color);
        placedObject.transform.eulerAngles = currentRotation;
    }
}
