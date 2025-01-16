using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class UpPicker : MonoBehaviour
{
    Camera mainCamera;
    InputAction carryAction;
    CarryAndShoot carriedObject;
    public float distanceToPickUp = 5.0f;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainCamera = GetComponent<Camera>();
        carryAction = InputSystem.actions.FindAction("Carry");
    }

    // Update is called once per frame
    void Update()
    {
        if (carryAction.WasPressedThisFrame()) {
            Debug.Log("Action was pressed");
            PickUp();
        }
    }

    void PickUp()
    {
        if (carriedObject != null) {
            carriedObject.Drop();
            carriedObject = null;
            return;
        }

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
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
