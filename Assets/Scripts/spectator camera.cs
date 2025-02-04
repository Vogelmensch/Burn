using UnityEngine;

public class spectatorcamera : MonoBehaviour
{
    // Speed for camera movement
    public float movementSpeed = 10f;

    // Sensitivity for camera rotation
    public float mouseSensitivity = 2f;

    // Store the current rotation of the camera
    private Vector2 currentRotation = new Vector2(-63.973f, -6.016f);

    void Start()
    {
        // Hide the cursor and lock it to the center of the screen
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Handle movement
        HandleMovement();

        // Handle rotation
        HandleRotation();
    }

    private void HandleMovement()
    {
        // Get the input for movement along X and Z axes
        float moveHorizontal = Input.GetAxis("Horizontal"); // A, D or Left, Right arrows
        float moveVertical = Input.GetAxis("Vertical");     // W, S or Up, Down arrows
        float moveUpDown = 0f;

        // Get the input for movement along Y axis
        if (Input.GetKey(KeyCode.Space)) // Space key for moving up
        {
            moveUpDown = 1f;
        }
        else if (Input.GetKey(KeyCode.LeftControl)) // Left Control key for moving down
        {
            moveUpDown = -1f;
        }

        // Calculate the movement vector
        Vector3 movement = new Vector3(moveHorizontal, moveUpDown, moveVertical) * movementSpeed * Time.deltaTime;

        // Apply the movement to the camera
        transform.Translate(movement);
    }

    private void HandleRotation()
    {
        // Get the mouse input for rotation
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Update the current rotation
        currentRotation.x -= mouseY;
        currentRotation.y += mouseX;

        // Apply the rotation to the camera
        transform.eulerAngles = currentRotation;
    }
}