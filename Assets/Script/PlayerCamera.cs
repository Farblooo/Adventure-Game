using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [Header("Sensitivity")]
    public float sensX = 100f;
    public float sensY = 100f;

    [Header("References")]
    public Transform orientation;   // This should be the player's body/orientation transform

    private float xRotation = 0f;
    private float yRotation = 0f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;   // Better than just visible = false
        Cursor.visible = false;
    }

    private void Update()
    {
        // Get mouse input
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        yRotation += mouseX;
        xRotation -= mouseY;

        // Clamp vertical rotation (looking up/down)
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Rotate the CAMERA (this object) for pitch + yaw
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);

        // Rotate only the orientation (player body) for horizontal movement
        if (orientation != null)
            orientation.rotation = Quaternion.Euler(0f, yRotation, 0f);
    }
}