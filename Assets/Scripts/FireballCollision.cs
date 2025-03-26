using UnityEngine;

public class FireballCollision : MonoBehaviour
{
    public float fireBallHitTemperatureIncrease = 100f;
    public GameObject explosionPrefab; // Prefab für den Explosionseffekt
    
    private bool hasCollided = false;
    
    // Setzt den Kollisionsstatus zurück
    private void ResetCollision()
    {
        hasCollided = false;
    }
    
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
                GameObject explosion = Instantiate(explosionPrefab, aufprallPosition, Quaternion.identity);
                // Zerstöre die Explosion nach 2 Sekunden
                Destroy(explosion, 2.0f);
            }
            
            // Nur bei Burnable-Objekten den Feuerball zerstören
            Debug.Log("FireballCollision: Zerstöre Feuerball nach Treffer auf Burnable");
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Viking") || collision.gameObject.CompareTag("EndTutorialBlock"))
        {
            Debug.Log("FireballCollision: Viking oder EndTutorialBlock getroffen!");
            
            // Kontaktpunkt für die Position der Explosion ermitteln
            ContactPoint contact = collision.contacts[0];
            Vector3 aufprallPosition = contact.point;
            
            // Explosion am Aufprallpunkt erzeugen
            if (explosionPrefab != null) 
            {
                Debug.Log("FireballCollision: Erstelle Explosion");
                GameObject explosion = Instantiate(explosionPrefab, aufprallPosition, Quaternion.identity);
                // Zerstöre die Explosion nach 2 Sekunden
                Destroy(explosion, 2.0f);
            }
            
            // EndTutorial-Event suchen und auslösen
            if (collision.gameObject.CompareTag("EndTutorialBlock"))
            {
                Level2Block block = collision.gameObject.GetComponent<Level2Block>();
                if (block != null)
                {
                    Debug.Log("FireballCollision: Level2Block gefunden und Event auslösen");
                    // Das Event wird automatisch ausgelöst, wenn das Objekt brennt
                }
            }
            else if (collision.gameObject.CompareTag("Viking"))
            {
                // Direktes Auslösen des EndTutorial-Skripts
                EndTutorial endTutorial = FindObjectOfType<EndTutorial>();
                if (endTutorial != null)
                {
                    Debug.Log("FireballCollision: Viking getroffen und EndTutorial ausgelöst");
                    endTutorial.SendMessage("FinishTut");
                }
            }
            
            // Feuerball zerstören
            Destroy(gameObject);
        }
        else
        {
            // Bei Nicht-Burnable-Objekten: Kollisionseffekt erzeugen und abprallen lassen
            Debug.Log("FireballCollision: Nicht-Burnable getroffen, Feuerball prallt ab");
            
            // Kontaktpunkt für die Position der Explosion ermitteln
            ContactPoint contact = collision.contacts[0];
            Vector3 aufprallPosition = contact.point;
            
            // Explosion am Aufprallpunkt erzeugen
            if (explosionPrefab != null) 
            {
                Debug.Log("FireballCollision: Erstelle Explosion");
                GameObject explosion = Instantiate(explosionPrefab, aufprallPosition, Quaternion.identity);
                // Zerstöre die Explosion nach 2 Sekunden
                Destroy(explosion, 2.0f);
            }
            
            // Abprallvektor berechnen
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Normale der Kollision verwenden, um korrekt abzuprallen
                Vector3 reflectionDir = Vector3.Reflect(rb.linearVelocity.normalized, contact.normal);
                float speed = rb.linearVelocity.magnitude;
                
                // 20% Geschwindigkeitsverlust beim Abprallen
                rb.linearVelocity = reflectionDir * speed * 0.8f;
                
                Debug.Log("FireballCollision: Feuerball abgeprallt mit Geschwindigkeit " + rb.linearVelocity.magnitude);
            }
            
            // Kurze Verzögerung, um mehrfache Kollisionen zu vermeiden
            Invoke("ResetCollision", 0.2f);
        }
    }
}