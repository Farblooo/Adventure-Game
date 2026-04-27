using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMovement : MonoBehaviour
{

    public float mouseSensitivity = 100f;
    public Transform playerBody;

    float xRotation = 0f;
    float YRotation = 0f;
    Camera cam;

    void Start()
    {
        //Locking the cursor to the middle of the screen and making it invisible
        Cursor.lockState = CursorLockMode.Locked;
        cam = Camera.main;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime; /* initial sensitivity is very low so we have to times it with mouse sensitivity
                                                                                      * delta time make so your sens doesn't get messed up when the frame rate varies */
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime; 

        //control rotation around x axis (Look up and down)
        xRotation -= mouseY; //y rotation is subtraacted because looking up decreases y axis for some reason
        

        //control rotation around y axis (Look up and down)
        YRotation += mouseX;

        //prevent cameras from moving more than over 90 degrees on x axis
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); //Prevent xRoatation from going under -90f or over 90f

        //applying both rotations
        cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        playerBody.transform.Rotate(Vector3.up * mouseX); //make so player body doesn't rotate vertically

        transform.Rotate(Vector3.up * mouseX);
    }
}