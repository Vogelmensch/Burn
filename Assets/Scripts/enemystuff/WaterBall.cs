using UnityEngine;

public class WaterBall : MonoBehaviour
{
    private Rigidbody rb;
    private float throwForce;
    private LayerMask ignoreLayer = 2;

    public void Initialize(Vector3 position, Vector3 direction, float force)
    {
        // Set the position and scale of the water ball
        transform.position = position;
        transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

        // Add a Rigidbody component to the sphere
        rb = gameObject.AddComponent<Rigidbody>();

        // Set the layer to ignore raycasts
        gameObject.layer = ignoreLayer;

        // Apply a forward force to the sphere
        throwForce = force;
        rb.AddForce(direction * throwForce, ForceMode.Impulse);

        // Optionally, add a collider to detect collisions with burnable objects
        SphereCollider collider = gameObject.GetComponent<SphereCollider>();
        collider.isTrigger = true;

        // Destroy the sphere after a certain time to clean up
        Destroy(gameObject, 5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the collided object is burnable
        Burnable burnable = CheckBurnable(other.transform);

        if (burnable != null)
        {
            // Put out the fire

        }
        else
        {
            // If not burnable, just ignore the collision
            Debug.Log("Not burnable: " + other.name);
        }
        
        // Destroy the water ball
        Destroy(gameObject);
    }

    private Burnable CheckBurnable(Transform currentTransform)
    {
        if (currentTransform == null) return null;

        // Check if the current transform has a Burnable component
        Burnable burnable = currentTransform.GetComponent<Burnable>();
        if (burnable != null)
        {
            return burnable;
        }

        // Recursively check the parent
        return CheckBurnable(currentTransform.parent);
    }
}