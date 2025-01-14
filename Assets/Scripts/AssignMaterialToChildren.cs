using UnityEngine;

public class AssignMaterialToChildren : MonoBehaviour
{
    public Material newMaterial; // The material to assign

    void Start()
    {
        if (newMaterial == null)
        {
            Debug.LogError("No material assigned to AssignMaterialToChildren script.");
            return;
        }

        AssignMaterial();
    }

    private void AssignMaterial()
    {
        // Iterate through all child objects
        Renderer[] childRenderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in childRenderers)
        {
            renderer.material = newMaterial; // Assign the new material
        }
    }
}
