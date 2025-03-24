using UnityEngine;

public class brittleRock : MonoBehaviour
{
    private Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError("Rigidbody is missing! Adding one now.");
            rb = gameObject.AddComponent<Rigidbody>();
        }

        rb.isKinematic = true;  // Keep the rock static until collision
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.GetComponent<Rigidbody>() != null || other.gameObject.tag == "Player")
        {
            rb.isKinematic = false;
            rb.AddForce(Vector3.down * 5, ForceMode.Impulse);
        }
    }
}
