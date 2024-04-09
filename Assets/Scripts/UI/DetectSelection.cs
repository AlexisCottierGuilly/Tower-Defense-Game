using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Tower
{
    Basic,
    Splash,
    Ultimate
}

public class DetectSelection : MonoBehaviour
{
    public GameGenerator gameGenerator;
    
    [Header("Prefabs")]
    public List<TowerPrefab> towerPrefabs = new List<TowerPrefab>();

    [Header("Other")]
    public Camera mainCamera;
    public GameObject placedObjectPrefab;

    [HideInInspector] public bool canPlace = false;
    [HideInInspector] public GameObject placedObject;
    [HideInInspector] public Vector2 position = Vector2.zero;
    private Vector3 currentRotation = new Vector3(0, 30, 0);

    public void SetStructure(GameObject structurePrefab)
    {
        UnsetStructure();
        
        placedObjectPrefab = structurePrefab;
        currentRotation = new Vector3(0, 30, 0);
        canPlace = true;

        Start();
    }

    public void UnsetStructure()
    {
        placedObjectPrefab = null;
        Destroy(placedObject);
        placedObject = null;
        currentRotation = new Vector3(0, 30, 0);
        canPlace = false;
        position = Vector2.zero;
    }
    
    void Start()
    {
        if (placedObjectPrefab != null)
        {
            placedObject = Instantiate(placedObjectPrefab);
            placedObject.transform.eulerAngles = currentRotation;
            placedObject.transform.localScale *= GameManager.instance.towerSize;
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.gameState != GameState.Game)
            return;
        
        if (placedObjectPrefab != null && placedObject != null)
            if (Input.GetMouseButtonDown(0) && canPlace)
            {
                GameObject newObject = Instantiate(
                    placedObjectPrefab,
                    placedObject.transform.position,
                    Quaternion.identity
                );
                newObject.transform.localScale *= GameManager.instance.towerSize;
                newObject.transform.eulerAngles = currentRotation;
                if (!gameGenerator.PlaceTower(position, newObject))
                    Destroy(newObject);

                currentRotation = new Vector3(0, 30, 0);
                placedObject.transform.eulerAngles = currentRotation;
            }
            
            if (Input.GetKeyDown(KeyCode.R))
            {
                currentRotation.y += 60;
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

        if (selectedTile != null) //added by mic not effecient
        {
            Vector3 worldPos = selectedTile.GetComponent<TileBehaviour>().placement.transform.position;
            position = selectedTile.GetComponent<TileBehaviour>().position;
            if (placedObject != null)
            {
                worldPos.y += placedObject.transform.localScale.y / 2f;
                placedObject.transform.position = worldPos;
            }
        }

        canPlace = gameGenerator.CanPlace(position);
        if (placedObjectPrefab.GetComponent<TowerBehaviour>().data.cost > GameManager.instance.gold)
            canPlace = false;

        Color color = Color.green;

        if (!canPlace)
            color = Color.red;
        
        if (placedObject != null) //added by mic not effecient
        {
            placedObject.GetComponent<Renderer>().material.SetColor("_Color", color);
            foreach (Renderer child in placedObject.GetComponentsInChildren<Renderer>())
            child.material.SetColor("_Color", color);
            placedObject.transform.eulerAngles = currentRotation;
        }
    }
}
