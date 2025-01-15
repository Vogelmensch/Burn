using System;
using UnityEngine;
using UnityEngine.UIElements;

public class CarryAndShoot : MonoBehaviour
{
    private bool isBeingCarried = false;
    private Rigidbody rb;
    public GameObject playerCamera;
    public float distanceToPlayer = 1.0f;


    void Start() 
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Joystick1Button4))
        {
            isBeingCarried = !isBeingCarried;
        }

        if (isBeingCarried)
        {
            MoveInFrontOfPlayer();
        }
    }

    void MoveInFrontOfPlayer()
    {
        Vector3 playerPosition = playerCamera.transform.position;
        Quaternion playerRotation = playerCamera.transform.rotation;
        Vector3 force = playerPosition + playerRotation * new Vector3(0,0.4f,1) * distanceToPlayer - transform.position;
        force = 20 * force;


        rb.AddForce(force, ForceMode.Acceleration);
    }
}
