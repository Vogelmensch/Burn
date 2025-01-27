using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class UpPicker : MonoBehaviour
{
    Camera mainCamera;
    InputAction carryAction;
    InputAction shootAction;
    CarryAndShoot carriedObject;
    public float distanceToPickUp = 5.0f;
    public float shootingStrength = 50;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainCamera = GetComponent<Camera>();
        carryAction = InputSystem.actions.FindAction("Carry");
        shootAction = InputSystem.actions.FindAction("ObjectShoot");
    }

    // Update is called once per frame
    void Update()
    {
        if (!PauseMenu.isPaused)
        {
            if (carryAction.WasPressedThisFrame())
            {
                Debug.Log("Action was pressed");
                PickUp();
            }

            if (shootAction.WasPressedThisFrame())
            {
                carriedObject.Shoot(shootingStrength);
            }
        }
    }

    void PickUp()
    {
        if (carriedObject != null)
        {
            carriedObject.Drop();
            carriedObject = null;
            return;
        }

        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);

        // Cast a ray from center of screen and try to carry the first object it hits
        // Stolen from Igniter hehe
        Ray ray = mainCamera.ScreenPointToRay(screenCenter);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Debug.Log("Ray Hit Something");
            // Check if the clicked object is carryable
            CarryAndShoot carryAndShoot = hit.collider.GetComponent<CarryAndShoot>();
            if (carryAndShoot != null && Vector3.Distance(transform.position, hit.transform.position) < distanceToPickUp)
            {
                Debug.Log("Hit a carryable Item");
                carryAndShoot.Carry();
                carriedObject = carryAndShoot;
            }
        }
    }
}
