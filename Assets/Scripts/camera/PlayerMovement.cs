using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float sneakSpeed; // Neue Variable für Sneak-Geschwindigkeit
    private float currentMoveSpeed; // Variable um die aktuelle Geschwindigkeit zu speichern, die verwendet wird
    public float groundDrag;
    [Header("Sneaking")] // Header für Sneaking-bezogene Variablen
    public KeyCode sneakKey = KeyCode.LeftShift; // Taste für Sneaking - Standardmäßig Left Shift
    public bool isSneaking { get; private set; } // Bool um zu verfolgen, ob der Spieler sneakt, mit öffentlichem Getter aber privatem Setter
    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    private LayerMask navMeshMask = 1 << 10;
    bool grounded;
    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown = 0.25f;
    bool readyToJump = true;
    public float extraGravityMultiplier = 2.5f;
    public float airMultiplier = 0.4f;
    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public Transform orientation;
    float horizontalInput;
    float verticalInput;
    Vector3 moveDirection;
    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        currentMoveSpeed = moveSpeed; // Initialisiere currentMoveSpeed mit der normalen moveSpeed 
    }
    private void Update(){
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround | navMeshMask);
        MyInput();
        SpeedControl();
        // handle drag
        if (grounded){
            rb.linearDamping = groundDrag;
        } else {
            rb.linearDamping = 0;
        }
        // jump
        if (Input.GetKeyDown(jumpKey) && readyToJump && grounded){
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }
    private void FixedUpdate(){
        MovePlayer();
    }
    private void MyInput(){
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        // Sneaking Input Handling
        if (Input.GetKey(sneakKey))
        {
            isSneaking = true;
            currentMoveSpeed = sneakSpeed; // Setze die aktuelle Geschwindigkeit auf die Sneak-Geschwindigkeit, wenn die Sneak-Taste gedrückt wird
        }
        else
        {
            isSneaking = false;
            currentMoveSpeed = moveSpeed; // Setze die aktuelle Geschwindigkeit zurück zur normalen Geschwindigkeit, wenn die Sneak-Taste losgelassen wird
        }
    }
    private void MovePlayer(){
        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        // on ground
        if (grounded){
            rb.AddForce(moveDirection.normalized * currentMoveSpeed * 10f, ForceMode.Force); // Verwende currentMoveSpeed anstelle von moveSpeed
        } else if (!grounded){
            rb.AddForce(moveDirection.normalized * currentMoveSpeed * 10f * airMultiplier, ForceMode.Force); // Verwende currentMoveSpeed hier auch
            rb.AddForce(Vector3.down * rb.mass * extraGravityMultiplier, ForceMode.Acceleration);
        }
    }
    private void SpeedControl(){
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        if (flatVel.magnitude > currentMoveSpeed){ // Verwende currentMoveSpeed für die Geschwindigkeitsbegrenzung
            Vector3 limitedVel = flatVel.normalized * currentMoveSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }
    private void Jump(){
        // reset y velocity
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    private void ResetJump(){
        readyToJump = true;
    }
}