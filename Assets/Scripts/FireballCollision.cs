using UnityEngine;

public class FireballCollision : MonoBehaviour
{
    public float fireBallHitTemperatureIncrease = 100f;
    void OnCollisionEnter(Collision collision) 
    {
        Burnable burnable = collision.collider.GetComponent<Burnable>();
        if (burnable != null) {
            burnable.IncreaseTemperature(fireBallHitTemperatureIncrease);
        }
        Destroy(gameObject);
    }
}
