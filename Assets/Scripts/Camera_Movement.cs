using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class CameraController : MonoBehaviour
{
    private CharacterController controller;

    InputAction moveAction;
    InputAction rotateAction;
    public bool usingDavidsWhiteController = false;

    // Speed for camera movement
    public float movementSpeed = 10f;

    // Sensitivity for camera rotation
    public float mouseSensitivity = 2f;

    public float controllerSensitivity = 2f;

    // Store the current rotation of the camera
    private Vector2 currentRotation = new Vector2(-63.973f, -6.016f);
    private float lastRotation = 0; // used when playing with buggy controller :3

    void Start()
    {
        controller = GetComponent<CharacterController>();

        moveAction = InputSystem.actions.FindAction("Move");
        rotateAction = InputSystem.actions.FindAction("Look");
    }

    void Update()
    {
        // Handle movement
        HandleMovement();


        HandleRotation();


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

        movement.x = moveAction.ReadValue<Vector2>().x;
        movement.y = moveAction.ReadValue<Vector2>().y;

        // rotate
        if (usingDavidsWhiteController)
            movement = Quaternion.AngleAxis(lastRotation, Vector3.up) * movement;
        else
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
        // This code is work in progress. It's only for enabling a certain controller of mine. Don't worry about it.
        if (usingDavidsWhiteController)
        {
            var gamepad = Joystick.current; // Oder Gamepad.current, falls es ein Gamepad ist
            if (gamepad != null)
            {
                float rotateZ = gamepad.GetChildControl<AxisControl>("Z").ReadValue();

                rotateZ *= Time.deltaTime * controllerSensitivity;

                transform.Rotate(0, rotateZ, 0);
                lastRotation = rotateZ;
            }
            return;
        }


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

        // Update rotation values
        currentRotation.x += mouseX * mouseSensitivity;
        currentRotation.y -= mouseY * mouseSensitivity;

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
