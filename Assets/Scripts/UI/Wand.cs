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
    
    [Header("Debug-Einstellungen")]
    public bool debugModus = true;               // Debug-Modus aktivieren
    
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

    // Referenz zum AudioManager
    private AudioManager audioManager;

    // Findet den AudioManager beim Aufwachen
    private void Awake(){
        GameObject audioObj = GameObject.FindGameObjectWithTag("Audio");
        if(audioObj != null) {
            audioManager = audioObj.GetComponent<AudioManager>();
            if (debugModus) Debug.Log("[Wand] AudioManager gefunden");
        } else {
            Debug.LogWarning("[Wand] Kein GameObject mit Tag 'Audio' gefunden!");
        }
    }

    void Start()
    {   
        Debug.Log("[Wand] Skript wird initialisiert");
        
        // UpPicker in der Szene finden
        FindeUpPicker();
        
        // Position und Audio initialisieren
        InitialisierePosUndAudio();
        
        // Debug-Ausgabe
        if (debugModus) Debug.Log("[Wand] Start abgeschlossen. UpPicker gefunden: " + (upPicker != null));
    }

    void Update()
    {
        // Überprüfen, ob der Spieler ein Objekt trägt
        PruefeObjektStatus();
        
        // Zauberstab aktualisieren wenn nötig
        AktualisiereZauberstab();
    }
    
    // Sucht den UpPicker in der Szene
    private void FindeUpPicker()
    {
        upPicker = FindObjectOfType<UpPicker>();
        if (upPicker == null)
        {
            Debug.LogError("[Wand] Konnte UpPicker nicht in der Szene finden!");
        }
    }
    
    // Initialisiert die Position und den AudioSource-Komponenten
    private void InitialisierePosUndAudio()
    {
        // Wenn zauberstabPosition nicht gesetzt ist, erstelle eine Standard-Position
        if (zauberstabPosition == null)
        {
            GameObject positionObj = new GameObject("ZauberstabPosition");
            positionObj.transform.parent = Camera.main.transform;
            positionObj.transform.localPosition = new Vector3(0.4f, -0.3f, 0.6f); // Unten rechts im Bild
            zauberstabPosition = positionObj.transform;
            
            if (debugModus) Debug.Log("[Wand] Neue ZauberstabPosition erstellt");
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
            
            if (debugModus) Debug.Log("[Wand] AudioSource hinzugefügt");
        }
    }
    
    // Prüft, ob der Spieler ein Objekt trägt und passt den Status an
    private void PruefeObjektStatus()
    {
        if (upPicker == null) return;
        
        bool objektWirdGetragen = upPicker.IsCurrentlyCarrying();
        
        // Wenn der Zustand sich geändert hat und keine Animation läuft
        if (objektWirdGetragen != traegtObjekt)
        {
            // Alte Animation abbrechen, falls noch aktiv
            if (istInAnimation)
            {
                StopAllCoroutines();
                istInAnimation = false;
                if (debugModus) Debug.Log("[Wand] Alte Animation abgebrochen");
            }
            
            traegtObjekt = objektWirdGetragen;
            
            if (traegtObjekt)
            {
                StartCoroutine(ZauberstabAuspacken());
                if (debugModus) Debug.Log("[Wand] Objekt aufgenommen, zeige Zauberstab");
            }
            else
            {
                StartCoroutine(ZauberstabEinpacken());
                if (debugModus) Debug.Log("[Wand] Objekt abgelegt, verstecke Zauberstab");
            }
        }
    }
    
    // Aktualisiert den Zauberstab, wenn er sichtbar ist
    private void AktualisiereZauberstab()
    {
        // Kein Update nötig, wenn Zauberstab nicht sichtbar, Animation läuft, oder kein Objekt aktiv ist
        if (!istSichtbar || aktiverZauberstab == null || istInAnimation) return;
        
        // Wenn ein Objekt getragen wird
        if (traegtObjekt)
        {
            ZauberstabAussehendAktualisieren();
            
            if (animiereZauberstab)
            {
                ZauberstabAnimieren();
            }
        }
        // Notfallkorrektur: Zauberstab ausblenden, wenn kein Objekt getragen wird
        else if (aktiverZauberstab.activeSelf)
        {
            ZauberstabEinpacken();
            if (debugModus) Debug.Log("[Wand] Notfall-Ausblenden");
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
        Debug.Log("[Wand] ZauberstabAuspacken gestartet!");

        istInAnimation = true;
        
        // Vorhandenen Zauberstab entfernen, um sauberen Start zu garantieren
        if (aktiverZauberstab != null)
        {
            Destroy(aktiverZauberstab);
            aktiverZauberstab = null;
        }
        
        // Neuer Zauberstab - Prüfen ob Prefab vorhanden
        if (zauberstabPrefab == null)
        {
            Debug.LogError("[Wand] Zauberstab-Prefab fehlt!");
            istInAnimation = false;
            yield break;
        }
        
        // Zauberstab erstellen
        aktiverZauberstab = Instantiate(zauberstabPrefab, zauberstabPosition);
        aktiverZauberstab.transform.localPosition = ausgangsPosition;
        
        // Initialen Zustand speichern
        ausgangsScale = aktiverZauberstab.transform.localScale;
        
        // Auf Null skalieren für Animation
        aktiverZauberstab.transform.localScale = Vector3.zero;
        
        // Zauberstab-Aussehen aktualisieren
        ZauberstabAussehendAktualisieren();
        
        // Sound abspielen
        if (spieleSound)
        {
            if (audioManager != null && audioManager.pickUp != null)
            {
                audioManager.PlaySound(audioManager.pickUp);
                if (debugModus) Debug.Log("[Wand] AudioManager Sound abgespielt");
            }
            else if (audioSource != null && zauberstabSound != null)
            {
                audioSource.PlayOneShot(zauberstabSound, soundVolume);
                if (debugModus) Debug.Log("[Wand] Fallback-Sound abgespielt");
            }
            else
            {
                if (debugModus) Debug.LogWarning("[Wand] Kein Sound verfügbar");
            }
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
        
        if (debugModus) Debug.Log("[Wand] Auspacken abgeschlossen. Sichtbar: " + istSichtbar);
    }
    
    // Einpack-Animation des Zauberstabs
    private IEnumerator ZauberstabEinpacken()
    {
        if (aktiverZauberstab == null) 
        {
            istSichtbar = false;
            istInAnimation = false;
            if (debugModus) Debug.LogWarning("[Wand] ZauberstabEinpacken: Zauberstab ist null!");
            yield break;
        }
        
        istInAnimation = true;
        
        if (debugModus) Debug.Log("[Wand] ZauberstabEinpacken gestartet!");
        
        // Sound abspielen
        if (spieleSound)
        {
            if (audioManager != null && audioManager.pickUp != null)
            {
                audioManager.PlaySound(audioManager.pickUp);
            }
            else if (audioSource != null && zauberstabSound != null)
            {
                audioSource.PlayOneShot(zauberstabSound, soundVolume);
            }
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
        
        if (debugModus) Debug.Log("[Wand] Einpacken abgeschlossen");
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
            if (debugModus) Debug.Log("[Wand] Neues Burnable-Objekt: " + (burnable != null ? burnable.name : "keins"));
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
    
    // Öffentliche Methoden für den Zugriff von außen
    
    // Gibt den aktiven Zauberstab zurück
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
    
    // Gibt zurück, ob ein Objekt getragen wird
    public bool TraegtObjekt()
    {
        return traegtObjekt;
    }
    
    // Gibt die Spitze des Zauberstabs zurück (oder erstellt sie, falls nicht vorhanden)
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
            
            if (debugModus) Debug.Log("[Wand] Neue Zauberstabspitze erstellt");
        }
        
        return tipTransform;
    }}