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
        Vector3 force = playerPosition + playerRotation * new Vector3(0, 0.4f, 1) * distanceWhileCarrying - transform.position;
        force = 20 * force;


        rb.AddForce(force, ForceMode.Acceleration);
    }

    public void Carry()
    {
        isBeingCarried = true;
    }

    public void Drop()
    {
        isBeingCarried = false;
    }

}
