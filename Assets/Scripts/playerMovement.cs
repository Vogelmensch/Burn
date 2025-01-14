using UnityEngine;

public class playerMovement : MonoBehaviour
{
    public float speed = 5f; // Movement speed
    public float jumpHeight = 2f; // Jump height
    public float crouchHeight = 1f; // Crouch height
    public float gravity = -9.81f; // Gravity force
    public float mouseSensitivity = 100f; // Mouse sensitivity

    private CharacterController characterController;
    private Vector3 velocity;
    private bool isGrounded;
    private float originalHeight;
    private Camera playerCamera;
    private float xRotation = 0f;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        originalHeight = characterController.height;
        playerCamera = Camera.main;
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor to the center of the screen
    }

    void Update()
    {
        // Handle movement
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        characterController.Move(move * speed * Time.deltaTime);

        // Handle jumping
        isGrounded = characterController.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Ensure the player stays grounded
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);

        // Handle crouching
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            characterController.height = crouchHeight;
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            characterController.height = originalHeight;
        }

        // Handle mouse look
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Clamp the vertical rotation

        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
}