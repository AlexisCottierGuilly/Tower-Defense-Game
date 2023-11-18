using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public float moveSpeed = 200f;
    public float turnSpeed = 8f;
    public TerrainGenerator terrainGenerator;
    public Rigidbody rb;

    private float initialHeight;
    
    void Start()
    {
        SetHeight(transform.position.y);
    }

    void SetHeight(float height)
    {
        transform.position = new Vector3(transform.position.x, height, transform.position.z);
        initialHeight = height;
    }

    // Update is called once per frame
    void Update()
    {
        // if WASD, move in x and y
        Vector3 force = new Vector3(0, 0, 0);
        if (Input.GetKey(KeyCode.W))
            force += new Vector3(0, 0, 1);
        if (Input.GetKey(KeyCode.S))
            force += new Vector3(0, 0, -1);
        if (Input.GetKey(KeyCode.A))
            force += new Vector3(-1, 0, 0);
        if (Input.GetKey(KeyCode.D))
            force += new Vector3(1, 0, 0);
        
        rb.AddRelativeForce(force * moveSpeed);

        // if QE or the mouse is moved while the right mouse button is held down, rotate the camera
        Vector3 newRotation = new Vector3(0, 0, 0);
        if (Input.GetKey(KeyCode.Q))
            newRotation += new Vector3(0, -1, 0);
        if (Input.GetKey(KeyCode.E))
            newRotation += new Vector3(0, 1, 0);
        
        rb.AddTorque(newRotation * turnSpeed);
        
        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X");
            mouseX *= turnSpeed / 2f;
            transform.eulerAngles = new Vector3(
                transform.eulerAngles.x,
                transform.eulerAngles.y + mouseX,
                transform.eulerAngles.z
            );
        }

        // reset the camera height (changed with AddRelativeForce)
        transform.position = new Vector3(
            transform.position.x,
            initialHeight,
            transform.position.z
        );

        // check if camera is in bounds
        Rect bounds = terrainGenerator.GetBounds();
        if (transform.position.x < bounds.xMin)
            transform.position = new Vector3(bounds.xMin, transform.position.y, transform.position.z);
        if (transform.position.x > bounds.xMax)
            transform.position = new Vector3(bounds.xMax, transform.position.y, transform.position.z);
        if (transform.position.z < bounds.yMin)
            transform.position = new Vector3(transform.position.x, transform.position.y, bounds.yMin);
        if (transform.position.z > bounds.yMax)
            transform.position = new Vector3(transform.position.x, transform.position.y, bounds.yMax);
    }
}
