using UnityEngine;

public class FireballCollision : MonoBehaviour
{
    public float fireBallHitTemperatureIncrease = 100f;
    public GameObject explosionPrefab; // Prefab für den Explosionseffekt
    
    private bool hasCollided = false;
    
    void Start()
    {
        Debug.Log("FireballCollision: Feuerball wurde erstellt!");
    }
    
    void OnCollisionEnter(Collision collision) 
    {
        // Ignoriere Kollisionen mit der Kamera
        if (collision.gameObject.CompareTag("MainCamera") || collision.gameObject.name == "Main Camera")
            return;
            
        Debug.Log("FireballCollision: Kollision mit " + collision.gameObject.name);
        
        // Verhindere mehrfache Kollisionsbehandlung
        if (hasCollided)
            return;
            
        hasCollided = true;
        
        // Prüfen, ob das getroffene Objekt ein Burnable ist
        Burnable burnable = collision.collider.GetComponent<Burnable>();
        
        if (burnable != null) 
        {
            Debug.Log("FireballCollision: Burnable-Objekt getroffen!");
            
            // Kontaktpunkt für die Position der Explosion ermitteln
            ContactPoint contact = collision.contacts[0];
            Vector3 aufprallPosition = contact.point;
            
            // Temperatur des Burnable-Objekts erhöhen
            burnable.IncreaseTemperature(fireBallHitTemperatureIncrease);
            
            // Wenn das Objekt noch nicht brennt, zünde es an
            if (!burnable.isOnFire) 
            {
                burnable.Ignite();
            }
            
            // Explosion am Aufprallpunkt erzeugen
            if (explosionPrefab != null) 
            {
                Debug.Log("FireballCollision: Erstelle Explosion");
                Instantiate(explosionPrefab, aufprallPosition, Quaternion.identity);
            }
            
            // Nur bei Burnable-Objekten den Feuerball zerstören
            Debug.Log("FireballCollision: Zerstöre Feuerball nach Treffer auf Burnable");
            Destroy(gameObject);
        }
        else
        {
            // Bei Nicht-Burnable-Objekten: Kollisionseffekt, aber nicht zerstören
            // Hier kannst du z.B. einen leichten Aufprall-Effekt hinzufügen, falls gewünscht
            Debug.Log("FireballCollision: Nicht-Burnable getroffen, Feuerball bleibt erhalten");
            
            // Warte kurz, um mehrfache Kollisionen zu vermeiden
            hasCollided = false;
        }
    }
}