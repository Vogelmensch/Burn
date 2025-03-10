using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro; // Füge diesen Namespace hinzu, wenn du TextMeshPro verwendest
// using UnityEngine.UI; // Füge diesen Namespace hinzu, wenn du das standardmäßige Text UI verwendest

public class UpPicker : MonoBehaviour
{
    Camera mainCamera;
    InputAction carryAction;
    InputAction shootAction;
    CarryAndShoot carriedObject;
    public float distanceToPickUp = 5.0f;
    public float shootingStrength = 50;

    [Header("UI Elemente")]
    public TextMeshProUGUI hitpointsTextDisplay_TMP; // Für TextMeshPro
    // public Text hitpointsTextDisplay_UI; // Für das standardmäßige Text UI. Wähle den entsprechenden Typ aus

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainCamera = GetComponent<Camera>();
        carryAction = InputSystem.actions.FindAction("Carry");
        shootAction = InputSystem.actions.FindAction("ObjectShoot");

        // Stelle sicher, dass die Hitpoints-Anzeige zu Beginn ausgeblendet ist
        if (hitpointsTextDisplay_TMP != null) // Überprüfe für TextMeshPro
        {
            hitpointsTextDisplay_TMP.gameObject.SetActive(false);
        }
        /*if (hitpointsTextDisplay_UI != null) // Überprüfe für das standardmäßige Text UI
        {
            hitpointsTextDisplay_UI.gameObject.SetActive(false);
        }*/
    }

    
    void Update()
    {
        if (carryAction.WasPressedThisFrame())
        {
            PickUp();
        }

        if (shootAction.WasPressedThisFrame() && IsCurrentlyCarrying())
        {
            carriedObject.Shoot(shootingStrength);
            carriedObject = null;
            UpdateHitpointsDisplay(); 
        }

        UpdateHitpointsDisplay(); 
    }

    void PickUp()
    {
        if (IsCurrentlyCarrying())
        {
            carriedObject.Drop();
            carriedObject = null;
            UpdateHitpointsDisplay(); 
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
            // Try objects parent
            if (carryAndShoot == null)
                carryAndShoot = hit.collider.GetComponentInParent<CarryAndShoot>();

            if (carryAndShoot != null && Vector3.Distance(transform.position, hit.transform.position) < distanceToPickUp)
            {
                carryAndShoot.Carry();
                carriedObject = carryAndShoot;
            }
        }
        UpdateHitpointsDisplay(); 
    }

    public bool IsCurrentlyCarrying()
    {
        return carriedObject != null;
    }

    void UpdateHitpointsDisplay()
    {
        if (IsCurrentlyCarrying() && carriedObject != null)
        {
            Burnable burnable = carriedObject.GetComponent<Burnable>();
            if (burnable != null)
            {
                if (hitpointsTextDisplay_TMP != null) 
                {
                    hitpointsTextDisplay_TMP.text = "Hitpoints: " + burnable.hitPoints.ToString("F0"); // "F0" formatiert die Zahl ohne Dezimalstellen
                    hitpointsTextDisplay_TMP.gameObject.SetActive(true);
                }
                /*if (hitpointsTextDisplay_UI != null) 
                {
                    hitpointsTextDisplay_UI.text = "Hitpoints: " + burnable.hitPoints.ToString("F0");
                    hitpointsTextDisplay_UI.gameObject.SetActive(true);
                }*/
            }
            else
            {
                if (hitpointsTextDisplay_TMP != null) 
                {
                    hitpointsTextDisplay_TMP.gameObject.SetActive(false);
                }
                /*if (hitpointsTextDisplay_UI != null) 
                {
                    hitpointsTextDisplay_UI.gameObject.SetActive(false);
                }*/
            }
        }
        else
        {
            if (hitpointsTextDisplay_TMP != null) 
            {
                hitpointsTextDisplay_TMP.gameObject.SetActive(false);
            }
            /*if (hitpointsTextDisplay_UI != null) 
            {
                hitpointsTextDisplay_UI.gameObject.SetActive(false);
            }*/
        }
    }
}