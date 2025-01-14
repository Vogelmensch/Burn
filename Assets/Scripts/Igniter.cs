using UnityEngine;

public class Igniter : MonoBehaviour
{
    public Camera mainCamera;
    public int hasIgnite = 100;
    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main; // Use the main camera by default
        }
    }

    void Update()
    {
        // Check for mouse click
        if (Input.GetKeyDown(KeyCode.F)) // Left mouse button
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // Check if the clicked object is a of type Burnable
                Burnable burnable = hit.collider.GetComponent<Burnable>();
                if (burnable != null && hasIgnite >= 0)
                {
                    if (burnable.isOnFire)
                    {
                        burnable.Extinguish(); 
                    }
                    else
                    {
                        burnable.Ignite(); // Ignite the cube
                        hasIgnite -= 1;
                    }
                }
            }
        }
    }
}

