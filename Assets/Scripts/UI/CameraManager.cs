using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public float moveSpeed = 200f;
    public float turnSpeed = 8f;
    public GameGenerator gameGenerator;
    public Rigidbody rb;

    private float initialHeight;
    private float additionalHeight = 0f;
    
    void Start()
    {
        SetHeight(transform.position.y);
    }

    public void SetHeight(float height)
    {
        transform.position = new Vector3(transform.position.x, height, transform.position.z);
        initialHeight = height;
    }

    // Update is called once per frame
    void Update()
    {
        float localDeltaTime = Time.deltaTime;
        if (localDeltaTime == 0f)
            localDeltaTime = Time.unscaledDeltaTime;
        
        if (Time.deltaTime != 0f)
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
            
            rb.AddRelativeForce(force * moveSpeed * localDeltaTime * 120f);
        }
        else
        {
            // move with WASD using the localDeltaTime, but without applying forces

            Vector3 move = new Vector3(0, 0, 0);
            if (Input.GetKey(KeyCode.W))
                move += new Vector3(0, 0, 1);
            if (Input.GetKey(KeyCode.S))
                move += new Vector3(0, 0, -1);
            if (Input.GetKey(KeyCode.A))
                move += new Vector3(-1, 0, 0);
            if (Input.GetKey(KeyCode.D))
                move += new Vector3(1, 0, 0);

            move *= moveSpeed * localDeltaTime / 6f;
            transform.position += transform.TransformDirection(move);
        }

        // if QE or the mouse is moved while the right mouse button is held down, rotate the camera
        /* Vector3 newRotation = new Vector3(0, 0, 0);
        if (Input.GetKey(KeyCode.Q))
            newRotation += new Vector3(0, -1, 0);
        if (Input.GetKey(KeyCode.E))
            newRotation += new Vector3(0, 1, 0);
        
        rb.AddTorque(newRotation * turnSpeed * localDeltaTime * 120f); */
        
        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X");
            //float mouseY = Input.GetAxis("Mouse Y");

            mouseX *= turnSpeed / 2f;
            //mouseY *= turnSpeed / 2f;

            transform.eulerAngles = new Vector3(
                transform.eulerAngles.x, //- mouseY,
                transform.eulerAngles.y + mouseX,
                transform.eulerAngles.z
            );
        }

        // space : go up
        // shift : go down

        if (Input.GetKey(KeyCode.Space))
            additionalHeight += moveSpeed / 6f * localDeltaTime;
        else if (Input.GetKey(KeyCode.LeftShift))
            additionalHeight -= moveSpeed / 6f * localDeltaTime;

        // reset the camera height (changed with AddRelativeForce)
        transform.position = new Vector3(
            transform.position.x,
            initialHeight + additionalHeight,
            transform.position.z
        );

        // the x rotation should change depending on additionnalHeight
        // if its higher, the camera should look more down (1/x function -> the higher, the slower it changes)
        // if its lower, the camera should look more up (1/x function -> the lower, the slower it changes)

        // if additionnal height is 0, the x angle should be 40.
        // If the add. height is -10, for example, the x rotation should go close to 0
        // If the add. height is 100, it should go close to 90.

        // rotx = 24(arctan(add. height)) + 50 // arctan in radians
        float rotX = 25f * Mathf.Atan(0.035f * additionalHeight) + 35f;

        transform.eulerAngles = new Vector3(
            rotX,
            transform.eulerAngles.y,
            transform.eulerAngles.z
        );

        // check if camera is in bounds
        Rect bounds = gameGenerator.GetBounds();
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
