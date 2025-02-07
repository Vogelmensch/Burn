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
        if (carryAction.WasPressedThisFrame()) {
            PickUp();
        }

        if (shootAction.WasPressedThisFrame() && IsCurrentlyCarrying()) {
            carriedObject.Shoot(shootingStrength);
            carriedObject = null;
        }
    }

    void PickUp()
    {
        if (IsCurrentlyCarrying()) {
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
                // Check if the clicked object is carryable
                CarryAndShoot carryAndShoot = hit.collider.GetComponent<CarryAndShoot>();
                if (carryAndShoot != null && Vector3.Distance(transform.position, hit.transform.position) < distanceToPickUp)
                {
                    carryAndShoot.Carry();
                    carriedObject = carryAndShoot;
                }
            }
    }

    public bool IsCurrentlyCarrying()
    {
        return carriedObject != null;
    }
}
