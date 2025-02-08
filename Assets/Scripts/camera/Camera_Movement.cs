using UnityEngine;

public class CameraController : MonoBehaviour
{
<<<<<<< HEAD:Assets/Scripts/camera/Camera_Movement.cs
    public bool standStillWhenCarrying = false;

=======
>>>>>>> Tutorial:Assets/Scripts/Camera_Movement.cs
    // Speed for camera movement
    public float movementSpeed = 10f;

    // Sensitivity for camera rotation
    public float mouseSensitivity = 2f;

<<<<<<< HEAD:Assets/Scripts/camera/Camera_Movement.cs
    public float controllerSensitivity = 2f;


    private CharacterController controller;
    private UpPicker upPicker;
    InputAction moveAction;
    InputAction rotateAction;
    // Store the current rotation of the camera
    private Vector2 currentRotation = new Vector2(-63.973f, -6.016f);


    void Start()
    {
        controller = GetComponent<CharacterController>();
        upPicker = GetComponent<UpPicker>();

        moveAction = InputSystem.actions.FindAction("Move");
        rotateAction = InputSystem.actions.FindAction("Look");

        if (standStillWhenCarrying && upPicker == null)
            Debug.LogError("You need to apply an upPicker-object to the camera to use this option!");
    }

    void Update()
    {
        HandleRotation();
        HandleMovement();
=======
    // Store the current rotation of the camera
    private Vector2 currentRotation = new Vector2(-63.973f, -6.016f);

    void Update()
    {
        // Handle movement
        HandleMovement();

        // Handle rotation
        if (Input.GetMouseButton(1)) // Right mouse button is pressed
        {
            HandleRotation();
        }
>>>>>>> Tutorial:Assets/Scripts/Camera_Movement.cs
    }

    private void HandleMovement()
    {
<<<<<<< HEAD:Assets/Scripts/camera/Camera_Movement.cs
=======
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

>>>>>>> Tutorial:Assets/Scripts/Camera_Movement.cs
        // Calculate the movement vector
        Vector3 movement = new Vector3();

<<<<<<< HEAD:Assets/Scripts/camera/Camera_Movement.cs
        if (standStillWhenCarrying && upPicker.IsCurrentlyCarrying()) {
            movement.x = 0;
            movement.z = 0;
        } else {
            movement.x = moveAction.ReadValue<Vector2>().x;
            movement.z = moveAction.ReadValue<Vector2>().y;
        }

        // rotate
        movement = Quaternion.Euler(0, currentRotation.x, 0f) * movement;

        if (controller.isGrounded)
        {
            movement.y = 0;
        }
        else
        {
            movement.y = -5;
        }


        // Apply movement to Controller
        controller.Move(movement * Time.deltaTime * movementSpeed);
=======
        // Apply the movement to the camera
        transform.Translate(movement * movementSpeed * Time.deltaTime, Space.Self);
>>>>>>> Tutorial:Assets/Scripts/Camera_Movement.cs
    }

    private void HandleRotation()
    {
<<<<<<< HEAD:Assets/Scripts/camera/Camera_Movement.cs
        float mouseX, mouseY;
        // Use mouse if right mouse button is pressed
        if (Input.GetMouseButton(1))
        {
            mouseX = Input.GetAxis("Mouse X");
            mouseY = Input.GetAxis("Mouse Y");
        }
        // else, use Gamepad
        else
        {
            Vector2 rotateValue = rotateAction.ReadValue<Vector2>();
            mouseX = rotateValue.x;
            mouseY = rotateValue.y;
        }
=======
        // Get mouse movement
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
>>>>>>> Tutorial:Assets/Scripts/Camera_Movement.cs

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
