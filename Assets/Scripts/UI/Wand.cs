using UnityEngine;
using System.Collections;

public class Wand : MonoBehaviour
{
    [Header("Zauberstab-Einstellungen")]
    public GameObject zauberstabPrefab;    // Referenz zum Zauberstab-Prefab (wand01_yellow)
    private GameObject aktiverZauberstab;   // Aktuell angezeigter Zauberstab
    public Transform zauberstabPosition;    // Wo der Zauberstab erscheinen soll
    
    [Header("Bounce-Animation")]
    public bool animiereZauberstab = true;    // Zauberstab animieren
    public float schwebeGeschwindigkeit = 1.0f;   // Geschwindigkeit der Schwebung
    public float schwebeHoehe = 0.03f;            // Höhe der Schwebung
    
    [Header("Ein- und Auspack-Animation")]
    public float animationDauer = 0.5f;           // Dauer der Ein-/Auspack-Animation
    public bool spieleSound = true;              // Sound beim Ein-/Auspacken abspielen
    public AudioClip zauberstabSound;             // Sound beim Ein- und Auspacken
    [Range(0.1f, 2.0f)]
    public float soundVolume = 1.0f;              // Lautstärke des Sounds
    
    // Animation Status
    private bool istInAnimation = false;
    private bool istSichtbar = false;
    private Vector3 ausgangsPosition;
    private Vector3 ausgangsScale;
    private AudioSource audioSource;
    
    // Referenz zum UpPicker
    private UpPicker upPicker;
    private Burnable aktuellesBurnable;
    private bool traegtObjekt = false;

    void Start()
    {
        // UpPicker in der Szene finden
        upPicker = FindObjectOfType<UpPicker>();
        
        if (upPicker == null)
        {
            Debug.LogError("Wand.cs: Konnte UpPicker nicht in der Szene finden!");
        }
        
        // Wenn zauberstabPosition nicht gesetzt ist, erstelle eine Standard-Position
        if (zauberstabPosition == null)
        {
            GameObject positionObj = new GameObject("ZauberstabPosition");
            positionObj.transform.parent = Camera.main.transform;
            positionObj.transform.localPosition = new Vector3(0.4f, -0.3f, 0.6f); // Unten rechts im Bild
            zauberstabPosition = positionObj.transform;
        }
        
        ausgangsPosition = Vector3.zero; // Lokale Position relativ zur zauberstabPosition
        
        // AudioSource hinzufügen falls noch nicht vorhanden
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && spieleSound)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 0f; // 2D Sound
            audioSource.volume = 1.0f;     // Basis-Lautstärke (wird mit soundVolume multipliziert)
            audioSource.playOnAwake = false;
        }
    }

    void Update()
    {
        // Überprüfen, ob der Spieler ein Objekt trägt
        bool objektWirdGetragen = (upPicker != null && upPicker.IsCurrentlyCarrying());
        
        // Wenn der Zustand sich geändert hat und keine Animation läuft
        if (objektWirdGetragen != traegtObjekt && !istInAnimation)
        {
            traegtObjekt = objektWirdGetragen;
            
            if (traegtObjekt)
            {
                StartCoroutine(ZauberstabAuspacken());
            }
            else
            {
                StartCoroutine(ZauberstabEinpacken());
            }
        }
        
        // Wenn ein Objekt getragen wird und Zauberstab sichtbar ist, Zauberstab aktualisieren
        if (traegtObjekt && istSichtbar && aktiverZauberstab != null && !istInAnimation)
        {
            ZauberstabAussehendAktualisieren();
            
            if (animiereZauberstab)
            {
                ZauberstabAnimieren();
            }
        }
    }
    
    // Animation für den Zauberstab - nur Bouncing ohne Rotation
    private void ZauberstabAnimieren()
    {
        if (aktiverZauberstab == null) return;
        
        // Zeit-Faktoren für die Bewegung
        float zeit = Time.time;
        
        // Position berechnen mit leichtem Bounce-Effekt
        float yOffset = Mathf.Sin(zeit * schwebeGeschwindigkeit) * schwebeHoehe;
        float xOffset = Mathf.Sin(zeit * schwebeGeschwindigkeit * 0.7f) * (schwebeHoehe * 0.4f);
        float zOffset = Mathf.Cos(zeit * schwebeGeschwindigkeit * 0.5f) * (schwebeHoehe * 0.3f);
        
        // Position des Zauberstabs aktualisieren
        aktiverZauberstab.transform.localPosition = new Vector3(
            xOffset,
            yOffset,
            zOffset
        );
        
        // Sehr leichtes Neigen als Reaktion auf die Bewegung
        float xTilt = -xOffset * 10f; // Neigung entgegen der Bewegungsrichtung
        float zTilt = -zOffset * 10f; // Neigung entgegen der Bewegungsrichtung
        
        aktiverZauberstab.transform.localRotation = Quaternion.Euler(xTilt, 0f, zTilt);
    }
    
    // Auspack-Animation des Zauberstabs
    private IEnumerator ZauberstabAuspacken()
    {
        istInAnimation = true;
        
        // Zauberstab erstellen falls noch nicht vorhanden
        if (aktiverZauberstab == null && zauberstabPrefab != null)
        {
            aktiverZauberstab = Instantiate(zauberstabPrefab, zauberstabPosition);
            aktiverZauberstab.transform.localPosition = ausgangsPosition;
            
            // Initialen Zustand speichern
            ausgangsScale = aktiverZauberstab.transform.localScale;
            
            // Auf Null skalieren für Animation
            aktiverZauberstab.transform.localScale = Vector3.zero;
            
            // Zauberstab-Aussehen aktualisieren
            ZauberstabAussehendAktualisieren();
        }
        else if (aktiverZauberstab != null)
        {
            aktiverZauberstab.SetActive(true);
            aktiverZauberstab.transform.localScale = Vector3.zero;
        }
        
        // Sound abspielen
        if (spieleSound && audioSource != null && zauberstabSound != null)
        {
            audioSource.PlayOneShot(zauberstabSound, soundVolume);
        }
        
        // Start der Animation: Erscheinen und Rotation
        float startZeit = Time.time;
        float endZeit = startZeit + animationDauer;
        
        while (Time.time < endZeit)
        {
            float t = (Time.time - startZeit) / animationDauer;
            
            // Easing-Funktion für sanfteren Verlauf
            t = EaseOutBack(t);
            
            // Scale und Rotation animieren
            aktiverZauberstab.transform.localScale = Vector3.Lerp(Vector3.zero, ausgangsScale, t);
            aktiverZauberstab.transform.localRotation = Quaternion.Euler(0, 360 * t, 0);
            
            yield return null;
        }
        
        // Animation abschließen
        aktiverZauberstab.transform.localScale = ausgangsScale;
        aktiverZauberstab.transform.localRotation = Quaternion.identity;
        
        istSichtbar = true;
        istInAnimation = false;
    }
    
    // Einpack-Animation des Zauberstabs
    private IEnumerator ZauberstabEinpacken()
    {
        if (aktiverZauberstab == null) yield break;
        
        istInAnimation = true;
        
        // Sound abspielen
        if (spieleSound && audioSource != null && zauberstabSound != null)
        {
            audioSource.PlayOneShot(zauberstabSound, soundVolume);
        }
        
        // Start der Animation: Verschwinden und Rotation
        float startZeit = Time.time;
        float endZeit = startZeit + animationDauer;
        
        while (Time.time < endZeit)
        {
            float t = (Time.time - startZeit) / animationDauer;
            
            // Easing für sanfteren Verlauf
            t = EaseInBack(t);
            
            // Scale und Rotation animieren
            aktiverZauberstab.transform.localScale = Vector3.Lerp(ausgangsScale, Vector3.zero, t);
            aktiverZauberstab.transform.localRotation = Quaternion.Euler(0, 360 * t, 0);
            
            yield return null;
        }
        
        // Animation abschließen
        aktiverZauberstab.SetActive(false);
        
        istSichtbar = false;
        istInAnimation = false;
    }
    
    // Easing-Funktionen für sanftere Animationen
    private float EaseOutBack(float t)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1;
        
        return 1 + c3 * Mathf.Pow(t - 1, 3) + c1 * Mathf.Pow(t - 1, 2);
    }
    
    private float EaseInBack(float t)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1;
        
        return c3 * t * t * t - c1 * t * t;
    }
    
    // Zauberstab-Aussehen basierend darauf aktualisieren, ob das getragene Objekt brennt
    private void ZauberstabAussehendAktualisieren()
    {
        if (aktiverZauberstab == null || upPicker == null) return;
        
        GameObject getrageneObjekt = upPicker.GetCurrentlyCarriedObject();
        if (getrageneObjekt == null) return;
        
        // Prüfen, ob das getragene Objekt eine Burnable-Komponente hat und brennt
        Burnable burnable = getrageneObjekt.GetComponent<Burnable>();
        if (burnable == null)
        {
            burnable = getrageneObjekt.GetComponentInParent<Burnable>();
        }
        
        if (aktuellesBurnable != burnable)
        {
            aktuellesBurnable = burnable;
        }
        
        bool brennt = (burnable != null && burnable.isOnFire);
        
        // Partikeleffekte basierend auf dem Feuerstatus aktivieren/deaktivieren
        AktualisierePartikelEffekte(brennt);
        
        // Zauberstab-Material/Farbe basierend auf dem Feuerstatus ändern
        AktualisiereMaterialEffekte(brennt);
    }
    
    // Partikeleffekte aktualisieren
    private void AktualisierePartikelEffekte(bool brennt)
    {
        ParticleSystem[] particles = aktiverZauberstab.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem ps in particles)
        {
            // Wenn "fire" im Namen vorkommt, aktiviere es nur für brennende Objekte
            if (ps.name.ToLower().Contains("fire"))
            {
                ps.gameObject.SetActive(brennt);
            }
            // Wenn "magic" im Namen vorkommt, aktiviere es immer
            else if (ps.name.ToLower().Contains("magic"))
            {
                ps.gameObject.SetActive(true);
            }
        }
    }
    
    // Material-Effekte aktualisieren
    private void AktualisiereMaterialEffekte(bool brennt)
    {
        Renderer[] renderers = aktiverZauberstab.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
        {
            if (brennt)
            {
                // Rötliches Leuchten für brennende Objekte
                r.material.SetColor("_EmissionColor", new Color(1f, 0.3f, 0f) * 2f);
                r.material.EnableKeyword("_EMISSION");
            }
            else
            {
                // Bläuliches Leuchten für nicht brennende Objekte
                r.material.SetColor("_EmissionColor", new Color(0.3f, 0.5f, 1f) * 0.8f);
                r.material.EnableKeyword("_EMISSION");
            }
        }
        
        // Licht aktualisieren, falls vorhanden
        Light[] lights = aktiverZauberstab.GetComponentsInChildren<Light>();
        foreach (Light light in lights)
        {
            if (brennt)
            {
                light.color = new Color(1f, 0.6f, 0.2f); // Orange für Feuer
                light.intensity = 1.5f;
            }
            else
            {
                light.color = new Color(0.5f, 0.7f, 1f); // Blau für Magie
                light.intensity = 1.0f;
            }
        }
    }
        public GameObject GetAktiverZauberstab()
    {
        return aktiverZauberstab;
    }

// Prüft, ob der Zauberstab sichtbar ist
    public bool IstSichtbar()
    {
        return istSichtbar;
    }

// Zeigt oder versteckt den Zauberstab
    public void ZeigeZauberstab(bool zeigen)
    {
        if (zeigen && !istSichtbar && !istInAnimation)
        {
            StartCoroutine(ZauberstabAuspacken());
        }
        else if (!zeigen && istSichtbar && !istInAnimation)
        {
            StartCoroutine(ZauberstabEinpacken());
        }
    }
    public bool TraegtObjekt()
    {
        return traegtObjekt;
    }
    public Transform GetZauberstabSpitze()
{
    if (aktiverZauberstab == null) return null;
    
    // Suche nach einem Kind-Objekt, das "Tip" oder "Spitze" heißt
    Transform tipTransform = aktiverZauberstab.transform.Find("Tip");
    if (tipTransform == null)
        tipTransform = aktiverZauberstab.transform.Find("Spitze");
    
    // Wenn wir keines finden, erstelle ein neues an der vermuteten Position der Spitze
    if (tipTransform == null)
    {
        GameObject tipObject = new GameObject("Tip");
        tipTransform = tipObject.transform;
        tipTransform.SetParent(aktiverZauberstab.transform);
        
        // Vermute die Spitze am Ende des Zauberstabes
        Renderer renderer = aktiverZauberstab.GetComponent<Renderer>();
        if (renderer != null)
        {
            float länge = renderer.bounds.size.z;
            // Positioniere die Spitze am vorderen Ende des Zauberstabes
            tipTransform.localPosition = new Vector3(0, 0, länge * 0.5f);
        }
        else
        {
            // Standardposition, falls kein Renderer gefunden wird
            tipTransform.localPosition = new Vector3(0, 0, 0.25f);
        }
    }
    
    return tipTransform;
}
}