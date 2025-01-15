using UnityEngine;

public class CameraController : MonoBehaviour
{
    private CharacterController controller;

    // Speed for camera movement
    public float movementSpeed = 10f;

    // Sensitivity for camera rotation
    public float mouseSensitivity = 2f;

    // Store the current rotation of the camera
    private Vector2 currentRotation = new Vector2(-63.973f, -6.016f);

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Handle movement
        HandleMovement();

        // Handle rotation
        if (Input.GetMouseButton(1)) // Right mouse button is pressed
        {
            HandleRotation();
        }
    }


    private void HandleMovement()
    {
        // Get the input for movement along X and Z axes
        float moveHorizontal = Input.GetAxis("Horizontal"); // A, D or Left, Right arrows
        float moveVertical = Input.GetAxis("Vertical");     // W, S or Up, Down arrows

        // Input for moving up and down
        float moveUp = 0f;
        if (Input.GetKey(KeyCode.E))
        {
            moveUp = 1f; // Move up
        }
        else if (Input.GetKey(KeyCode.Q))
        {
            moveUp = -1f; // Move down
        }


        // Calculate the movement vector
        Vector3 movement = new Vector3(moveHorizontal, moveUp, moveVertical);

        movement = Quaternion.Euler(currentRotation.y, currentRotation.x, 0f) * movement;

        // Don't Apply the movement to the camera
        //transform.Translate(movement * movementSpeed * Time.deltaTime, Space.Self);

        // Apply movement to Controller
        controller.Move(movement * Time.deltaTime * movementSpeed);

    }

    private void HandleRotation()
    {
        // Get mouse movement
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // Update rotation values
        currentRotation.x += mouseX * mouseSensitivity;
        currentRotation.y -= mouseY * mouseSensitivity;

        // Clamp the vertical rotation to avoid flipping
        currentRotation.x = Mathf.Repeat(currentRotation.x, 360);
        currentRotation.y = Mathf.Clamp(currentRotation.y, -90f, 90f);

        // Apply the rotation to the camera
        transform.rotation = Quaternion.Euler(currentRotation.y, currentRotation.x, 0f);

        Camera.main.transform.rotation = Quaternion.Euler(currentRotation.y, currentRotation.x, 0);
        if (Input.GetMouseButtonDown(0)){
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
