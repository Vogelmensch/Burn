using UnityEngine;

public class WaterBall : MonoBehaviour
{
    private Rigidbody rb;
    private float throwForce;

    public void Initialize(Vector3 position, Vector3 direction, float force)
    {
        // Set the position and scale of the water ball
        transform.position = position;
        transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

        // Add a Rigidbody component to the sphere
        rb = gameObject.AddComponent<Rigidbody>();

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
        Burnable burnable = other.GetComponent<Burnable>();
        if (burnable != null)
        {
            // Perform any additional logic here, e.g., extinguish the fire
            burnable.WaterHit(50f);
        }
        
        // Destroy the water ball
        Destroy(gameObject);
    }
}