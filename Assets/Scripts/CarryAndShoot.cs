using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class CarryAndShoot : MonoBehaviour
{
    private bool isBeingCarried = false;
    private Rigidbody rb;
    public GameObject playerCamera;
    public float distanceWhileCarrying = 1.5f;
    public float rotationSpeed = 100f; // Speed of rotation

    Quaternion initialRelativeRotation;
    // 0 -> ground; 1 -> 45°
    public float heightOfObjects = 0.6f;

    // Drag get applied only when carrying
    public float drag = 6;
    public float angularDrag = 1;



    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerCamera = GameObject.Find("PlayerCam");
    }

    void Update()
    {
        if (isBeingCarried)
        {
            MoveInFrontOfPlayer();
            HandleRotationInput();
        }
    }

    private void HandleRotationInput()
    {
        if (Input.GetKey(KeyCode.I))
        {
            RotateObject(playerCamera.transform.right);
        }
        if (Input.GetKey(KeyCode.K))
        {
            RotateObject(-playerCamera.transform.right);
        }
        if (Input.GetKey(KeyCode.J))
        {
            RotateObject(playerCamera.transform.up);
        }
        if (Input.GetKey(KeyCode.L))
        {
            RotateObject(-playerCamera.transform.up);
        }
    }
    private void RotateObject(Vector3 direction)
    {
        // Create a rotation quaternion based on the input direction and rotation speed
        Quaternion rotation = Quaternion.AngleAxis(rotationSpeed * Time.deltaTime, direction);

        // Update the initialRelativeRotation by multiplying it with the new rotation
        initialRelativeRotation = rotation * initialRelativeRotation;
    }

    void MoveInFrontOfPlayer()
    {
        Vector3 playerPosition = playerCamera.transform.position;
        Quaternion playerRotation = playerCamera.transform.rotation;
        Vector3 force = playerPosition + playerRotation * new Vector3(0, heightOfObjects, 1) * distanceWhileCarrying - transform.position;
        force = 20 * force;


        rb.AddForce(force, ForceMode.Acceleration);

        // make sure objects orientation stays the same
        transform.rotation = playerCamera.transform.rotation * initialRelativeRotation;
    }

    public void Shoot(float shootingStrength) 
    {
        Vector3 direction = transform.position - playerCamera.transform.position;
        rb.AddForce(shootingStrength * direction, ForceMode.Impulse);
        Drop();
    }

    public void Carry()
    {
        // Store the initial relative rotation between the object and the player's camera
        initialRelativeRotation = Quaternion.Inverse(playerCamera.transform.rotation) * transform.rotation;
        isBeingCarried = true;

        // When carrying, we need a high drag 
        // Otherwise, the object doesn't stop oscillating around wildly
        rb.linearDamping = drag;
        rb.angularDamping = angularDrag;
    }

    public void Drop()
    {
        isBeingCarried = false;

        // re-apply original drag
        rb.linearDamping = 0;
        rb.angularDamping = 0.05f;
    }

}
