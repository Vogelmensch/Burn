using UnityEngine;

public class FireballCollision : MonoBehaviour
{
    void OnCollisionEnter(Collision collision) 
    {
        Burnable burnable = collision.collider.GetComponent<Burnable>();
        if (burnable != null) {
            burnable.Ignite();
        }
        Destroy(gameObject);
    }
}
