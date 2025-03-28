using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public bool standStillWhenCarrying = false;

    // Speed for camera movement
    public float movementSpeed = 10f;

    // Sensitivity for camera rotation
    private float controllerSensitivity = 250f;
    private float mouseSensitivity = 30f;

    private CharacterController controller;
    private UpPicker upPicker;
    InputAction moveAction;
    InputAction controllerRotateAction;
    InputAction mouseRotateAction;
    // Store the current rotation of the camera
    private Vector2 currentRotation = new Vector2(-63.973f, -6.016f);


    void Start()
    {
        controller = GetComponent<CharacterController>();
        upPicker = GetComponent<UpPicker>();

        moveAction = InputSystem.actions.FindAction("Move");
        controllerRotateAction = InputSystem.actions.FindAction("LookController");
        mouseRotateAction = InputSystem.actions.FindAction("LookMouse");

        if (standStillWhenCarrying && upPicker == null)
            Debug.LogError("You need to apply an upPicker-object to the camera to use this option!");
    }

    void Update()
    {
        HandleRotation();
        HandleMovement();
    }

    private void HandleMovement()
    {
        // Calculate the movement vector
        Vector3 movement = new Vector3();

        if (standStillWhenCarrying && upPicker.IsCurrentlyCarrying())
        {
            movement.x = 0;
            movement.z = 0;
        }
        else
        {
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
    }

    private void HandleRotation()
    {
        if (PauseMenu.isPaused)
            return;

        // Controller
        float contX, contY;

        Vector2 contRotateValue = controllerRotateAction.ReadValue<Vector2>();
        contX = contRotateValue.x;
        contY = contRotateValue.y;

        // Update rotation values
        currentRotation.x += contX * controllerSensitivity * Time.deltaTime;
        currentRotation.y -= contY * controllerSensitivity * Time.deltaTime;

        // Mouse
        float mouseX, mouseY;

        Vector2 mouseRotateValue = mouseRotateAction.ReadValue<Vector2>();
        mouseX = mouseRotateValue.x;
        mouseY = mouseRotateValue.y;

        // Update rotation values
        currentRotation.x += mouseX * mouseSensitivity * Time.deltaTime;
        currentRotation.y -= mouseY * mouseSensitivity * Time.deltaTime;



        // Clamp the vertical rotation to avoid flipping
        currentRotation.x = Mathf.Repeat(currentRotation.x, 360);
        currentRotation.y = Mathf.Clamp(currentRotation.y, -90f, 90f);

        // Apply the rotation to the camera
        transform.rotation = Quaternion.Euler(currentRotation.y, currentRotation.x, 0f);

        Camera.main.transform.rotation = Quaternion.Euler(currentRotation.y, currentRotation.x, 0);
        if (Input.GetMouseButtonDown(0))
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
