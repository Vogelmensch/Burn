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
    public float distanceWhileCarrying = 1.0f;

    // 0 -> ground; 1 -> 45°
    public float heightOfObjects = 0.6f;

    // Drag get applied only when carrying
    public float drag = 6;
    public float angularDrag = 1;



    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (isBeingCarried)
        {
            MoveInFrontOfPlayer();
        }
    }

    void MoveInFrontOfPlayer()
    {
        Vector3 playerPosition = playerCamera.transform.position;
        Quaternion playerRotation = playerCamera.transform.rotation;
        Vector3 force = playerPosition + playerRotation * new Vector3(0, heightOfObjects, 1) * distanceWhileCarrying - transform.position;
        force = 20 * force;


        rb.AddForce(force, ForceMode.Acceleration);
    }

    public void Shoot(float shootingStrength) 
    {
        Vector3 direction = transform.position - playerCamera.transform.position;
        rb.AddForce(shootingStrength * direction, ForceMode.Impulse);
        Drop();
    }

    public void Carry()
    {
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
