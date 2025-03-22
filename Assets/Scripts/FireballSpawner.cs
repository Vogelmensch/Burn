using UnityEngine;
using UnityEngine.InputSystem;

public class FireballSpawner : MonoBehaviour
{
    [Header("Feuerball-Einstellungen")]
    public GameObject fireballPrefab;
    public Transform cameraTransform;
    public float constantSpeed = 15f;  // Konstante Fluggeschwindigkeit
    
    [Header("Wurfstärke-Einstellungen")]
    public int forceStrength = 150;    // Aktuelle Stärke (bestimmt Reichweite)
    public int forceLowerLimit = 100;  // Minimale Stärke
    public int forceUpperLimit = 350;  // Maximale Stärke
    public double forceChangeRate = 700; // Änderungsgeschwindigkeit
    
    [Header("Feuer-Interaktion")]
    public float fireAccessRadius = 10f;
    public float fireballThrowTemperatureDecrease = 100f;
    
    [Header("Visuelle & Audio-Effekte")]
    public ThrowStrengthIndicator throwStrengthIndicator;
    public AudioClip fireballCastSound;
    public GameObject castEffectPrefab;     // Effekt beim Zaubern/Werfen
    public GameObject explosionPrefab;      // Effekt beim Aufprall auf Objekte
    
    // Private Variablen
    private GameObject fireball;
    private Wand wandScript;
    private AudioSource audioSource;
    
    // Input Actions
    private InputAction increaseStrengthAction;
    private InputAction decreaseStrengthAction;
    private InputAction throwAction;

    void Start()
    {
        // Input Actions finden
        increaseStrengthAction = InputSystem.actions.FindAction("FireballIncrease");
        decreaseStrengthAction = InputSystem.actions.FindAction("FireballDecrease");
        throwAction = InputSystem.actions.FindAction("FireballThrow");
        
        // Zauberstab finden
        wandScript = FindObjectOfType<Wand>();
        
        // AudioSource vorbereiten
        PrepareAudioSource();
    }

    void PrepareAudioSource()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 0.6f; // Mix aus 2D und 3D Sound
            audioSource.volume = 0.8f;
            audioSource.playOnAwake = false;
        }
    }

    void Update()
    {
        ChangeStrength();
        HandleZauberstabVisiblity();
        CheckForFireballAction();
    }
    
    void HandleZauberstabVisiblity()
    {
        // Zauberstab sichtbar machen, wenn wir einen Feuerball vorbereiten
        if ((increaseStrengthAction.IsPressed() || decreaseStrengthAction.IsPressed()) && wandScript != null)
        {
            if (!wandScript.IstSichtbar())
            {
                wandScript.ZeigeZauberstab(true);
            }
        }
    }
    
    void CheckForFireballAction()
    {
        if (throwAction.WasPressedThisFrame())
        {
            if (wandScript != null && !wandScript.IstSichtbar())
            {
                // Zauberstab zuerst auspacken, dann schießen
                wandScript.ZeigeZauberstab(true);
                Invoke("TryFireball", 0.5f);  // Verzögerung für die Animation
            }
            else
            {
                TryFireball();
            }
        }
    }

    void ChangeStrength()
    {
        if (increaseStrengthAction.IsPressed())
        {
            forceStrength += (int)(forceChangeRate * Time.deltaTime);
            forceStrength = Mathf.Min(forceStrength, forceUpperLimit);
        }
        
        if (decreaseStrengthAction.IsPressed())
        {
            forceStrength -= (int)(forceChangeRate * Time.deltaTime);
            forceStrength = Mathf.Max(forceStrength, forceLowerLimit);
        }
        
        // Anzeige aktualisieren
        if (throwStrengthIndicator != null)
        {
            double amount = (double)forceStrength / (double)forceUpperLimit;
            throwStrengthIndicator.SetStrengthBar(amount);
        }
    }
    
    void TryFireball()
    {
        Burnable nearbyFire = FindNearbyFire();
        if (nearbyFire != null)
        {
            nearbyFire.FeedFireball();
            SpawnAndThrowFireball();
        }
        else if (throwStrengthIndicator != null)
        {
            StartCoroutine(throwStrengthIndicator.PrintNoFireMessage());
            // FIX: Pack wand away when there's no fire nearby
            Invoke("PackZauberstabEin", 0.5f);
        }
    }

    Burnable FindNearbyFire()
    {
        Collider[] nearbyObjects = Physics.OverlapSphere(cameraTransform.position, fireAccessRadius);
        foreach (Collider col in nearbyObjects)
        {
            Burnable burnable = col.GetComponent<Burnable>();
            if (burnable != null && burnable.isOnFire)
            {
                return burnable;
            }
        }
        return null;
    }

    void SpawnAndThrowFireball()
    {
        Vector3 spawnPosition;
        Quaternion spawnRotation;
        
        if (wandScript != null && wandScript.GetAktiverZauberstab() != null)
        {
            Transform zauberstab = wandScript.GetAktiverZauberstab().transform;
            
            // Zauberstab in Blickrichtung ausrichten
            Vector3 kameraRichtung = cameraTransform.forward;
            zauberstab.rotation = Quaternion.LookRotation(kameraRichtung, Vector3.up);
            
            // Position der Zauberstabspitze bestimmen
            Transform spitze = null;
            
            // Versuchen, die Methode GetZauberstabSpitze zu verwenden, falls vorhanden
            try {
                spitze = wandScript.GetZauberstabSpitze();
            }
            catch (System.Exception) {
                // Falls die Methode nicht existiert, ignoriere den Fehler
                spitze = null;
            }
            
            if (spitze != null)
            {
                // Verwende die definierte Spitze
                spawnPosition = spitze.position;
            }
            else
            {
                // Berechne die Spitze basierend auf dem Renderer
                Renderer renderer = zauberstab.GetComponent<Renderer>();
                if (renderer != null)
                {
                    float staebLaenge = renderer.bounds.size.z;
                    spawnPosition = zauberstab.position + zauberstab.forward * (staebLaenge * 0.5f);
                }
                else
                {
                    // Fallback: Einfacher Offset
                    spawnPosition = zauberstab.position + zauberstab.forward * 0.3f;
                }
            }
            
            // Ausrichtung genau in Kamerablickrichtung
            spawnRotation = Quaternion.LookRotation(kameraRichtung);
            
            // Zaubereffekt an der Stabspitze
            CreateCastEffect(spawnPosition, spawnRotation);
        }
        else
        {
            // Fallback falls kein Zauberstab verfügbar
            spawnPosition = cameraTransform.position + cameraTransform.forward * 1.5f;
            spawnRotation = Quaternion.LookRotation(cameraTransform.forward);
        }
        
        // Feuerball erstellen und werfen
        fireball = Instantiate(fireballPrefab, spawnPosition, spawnRotation);
        AddTrailToFireball(fireball);
        
        // Explosions-Prefab der FireballCollision-Komponente zuweisen
        FireballCollision collision = fireball.GetComponent<FireballCollision>();
        if (collision != null && explosionPrefab != null)
        {
            collision.explosionPrefab = explosionPrefab;
        }
        
        PlayFireballSound();
        ThrowFireball(fireball);
        
        // Zauberstab nach einer kurzen Verzögerung wieder einpacken
        Invoke("PackZauberstabEin", 0.8f);
    }
    
    void CreateCastEffect(Vector3 position, Quaternion rotation)
    {
        if (castEffectPrefab != null)
        {
            GameObject castEffect = Instantiate(castEffectPrefab, position, rotation);
            Destroy(castEffect, 1.5f);
        }
    }
    
    void PlayFireballSound()
    {
        if (fireballCastSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(fireballCastSound, 0.7f);
        }
    }

    void PackZauberstabEin()
    {
        if (wandScript != null && wandScript.IstSichtbar())
        {
            // Nur einpacken, wenn der Spieler kein Objekt trägt
            if (!wandScript.TraegtObjekt())
            {
                wandScript.ZeigeZauberstab(false);
            }
        }
    }

    void AddTrailToFireball(GameObject ball)
    {
        TrailRenderer existingTrail = ball.GetComponent<TrailRenderer>();
        
        if (existingTrail == null)
        {
            TrailRenderer trail = ball.AddComponent<TrailRenderer>();
            trail.time = 0.5f; 
            trail.widthMultiplier = 0.3f;
            trail.startWidth = 0.3f;
            trail.endWidth = 0.05f;
            trail.material = new Material(Shader.Find("Sprites/Default"));
            
            // Orangerot-Farbverlauf für den Trail
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { 
                    new GradientColorKey(new Color(1, 0.5f, 0), 0.0f), 
                    new GradientColorKey(new Color(1, 0.2f, 0), 0.5f),
                    new GradientColorKey(new Color(0.8f, 0.1f, 0), 1.0f) 
                },
                new GradientAlphaKey[] { 
                    new GradientAlphaKey(0.8f, 0.0f), 
                    new GradientAlphaKey(0.5f, 0.5f),
                    new GradientAlphaKey(0f, 1.0f) 
                }
            );
            trail.colorGradient = gradient;
        }
    }

    void ThrowFireball(GameObject ball)
    {
        Rigidbody rb = ball.GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Kein Rigidbody am Feuerball gefunden!");
            return;
        }
        
        // Richtung exakt wie Kamerablick
        Vector3 direction = cameraTransform.forward;
        
        // Konstante Geschwindigkeit
        rb.linearVelocity = direction * constantSpeed;
        
        // Lebensdauer basierend auf der Stärke
        float lifeTime = forceStrength / 100f;
        
        Debug.Log($"Feuerball: Richtung={direction}, Geschwindigkeit={constantSpeed}, Lebensdauer={lifeTime}s");
        
        Destroy(ball, lifeTime);
    }
}