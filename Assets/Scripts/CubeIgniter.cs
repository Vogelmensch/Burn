using UnityEngine;

public class CubeIgniter : MonoBehaviour
{
    public Camera mainCamera;

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
                // Check if the clicked object is a BurnableCube
                BurnableCube burnableCube = hit.collider.GetComponent<BurnableCube>();
                if (burnableCube != null)
                {
                    burnableCube.Ignite(); // Ignite the cube
                }
            }
        }
    }
}

