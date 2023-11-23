using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum TileType
{
    Grass,
    Path
}


public class TileBehaviour : MonoBehaviour
{
    public GameObject placement;
    public GameObject structure;
    public TileType type;
    public Vector2 position;
    public bool selected = false;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
