using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BurningBorderEffect : MonoBehaviour
{
    public LayerMask burningLayer; 
    public Image topBorder;       
    public Image bottomBorder;    
    public Image leftBorder;      
    public Image rightBorder;     
    
    [Range(0.0f, 1.0f)]
    public float maxBorderAlpha = 0.5f;
    
    [Range(0.1f, 10.0f)]
    public float smoothTime = 0.3f; // Zeit in Sekunden für die Glättung der Übergänge
    
    private Transform playerTransform;
    
    // Aktuelle Alphawerte mit Smoothing
    private float topAlpha = 0f;
    private float rightAlpha = 0f;
    private float bottomAlpha = 0f;
    private float leftAlpha = 0f;
    
    // Velocity-Variablen für SmoothDamp
    private float topVelocity = 0f;
    private float rightVelocity = 0f;
    private float bottomVelocity = 0f;
    private float leftVelocity = 0f;
    
    // Speichert die vorherige Entscheidung für eine geringe Hysterese
    private Dictionary<GameObject, int> previousBorderSelection = new Dictionary<GameObject, int>();

    void Start()
    {
        playerTransform = Camera.main.transform;

        if (topBorder == null || bottomBorder == null || leftBorder == null || rightBorder == null)
        {
            Debug.LogError("Nicht alle Border Images sind im Inspector zugewiesen!");
            return;
        }

        SetBorderAlpha(0f);
    }

    void Update()
    {
        if (playerTransform == null) return;

        // Ziel-Alpha-Werte für diesen Frame
        float targetTopAlpha = 0f;
        float targetRightAlpha = 0f;
        float targetBottomAlpha = 0f;
        float targetLeftAlpha = 0f;
        
        // Finde alle GameObjects auf dem Burning Layer
        GameObject[] burningObjects = FindGameObjectsWithLayer(burningLayer);
        
        // Verwalte die Objekt-Tracking-Liste - entferne Objekte, die nicht mehr existieren
        List<GameObject> keysToRemove = new List<GameObject>();
        foreach (var key in previousBorderSelection.Keys)
        {
            if (!ObjectExistsInArray(key, burningObjects))
            {
                keysToRemove.Add(key);
            }
        }
        
        foreach (var key in keysToRemove)
        {
            previousBorderSelection.Remove(key);
        }

        foreach (GameObject burningObject in burningObjects)
        {
            // Prüfe, ob das Objekt den IgnoreBorder Tag hat und überspringe es in diesem Fall
            if (burningObject.CompareTag("IgnoreBorder"))
            {
                continue; // Dieses brennende Objekt wird ignoriert
            }

            // Berechne die Richtung vom Spieler zum brennenden Objekt
            Vector3 directionToBurningObject = (burningObject.transform.position - playerTransform.position);
            
            // Berechne den Winkel in Grad (0-360°) 
            float angleDegrees = GetAngleAroundYAxis(playerTransform.forward, directionToBurningObject);
            
            // Bestimme, welcher Rand aktiviert werden soll (0=top, 1=right, 2=bottom, 3=left)
            int borderIndex;
            
            // Wenn wir das Objekt bereits verfolgen, verwenden wir Hysterese
            if (previousBorderSelection.ContainsKey(burningObject))
            {
                int previousIndex = previousBorderSelection[burningObject];
                
                // Hier nutzen wir Hysterese: Nur wenn der Winkel deutlich vom vorherigen Border abweicht,
                // ändert sich der Border, ansonsten behalten wir den vorherigen bei
                borderIndex = GetBorderIndexWithHysteresis(angleDegrees, previousIndex);
            }
            else
            {
                // Für neue Objekte einfach den Border anhand des Winkels bestimmen
                borderIndex = GetBorderIndex(angleDegrees);
            }
            
            // Speichere die Auswahl für das nächste Frame
            previousBorderSelection[burningObject] = borderIndex;
            
            // Aktiviere den entsprechenden Rand
            switch (borderIndex)
            {
                case 0: // Top
                    targetTopAlpha = maxBorderAlpha;
                    break;
                case 1: // Right
                    targetRightAlpha = maxBorderAlpha;
                    break;
                case 2: // Bottom
                    targetBottomAlpha = maxBorderAlpha;
                    break;
                case 3: // Left
                    targetLeftAlpha = maxBorderAlpha;
                    break;
            }
        }
        
        // Smooth transition zu den Zielwerten
        topAlpha = Mathf.SmoothDamp(topAlpha, targetTopAlpha, ref topVelocity, smoothTime);
        rightAlpha = Mathf.SmoothDamp(rightAlpha, targetRightAlpha, ref rightVelocity, smoothTime);
        bottomAlpha = Mathf.SmoothDamp(bottomAlpha, targetBottomAlpha, ref bottomVelocity, smoothTime);
        leftAlpha = Mathf.SmoothDamp(leftAlpha, targetLeftAlpha, ref leftVelocity, smoothTime);
        
        // Aktualisiere die UI-Ränder
        SetImageAlpha(topBorder, topAlpha);
        SetImageAlpha(rightBorder, rightAlpha);
        SetImageAlpha(bottomBorder, bottomAlpha);
        SetImageAlpha(leftBorder, leftAlpha);
    }
    
    // Gibt den Border-Index für einen bestimmten Winkel zurück
    int GetBorderIndex(float angleDegrees)
    {
        if (angleDegrees > 315f || angleDegrees <= 45f) return 0; // Top
        if (angleDegrees > 45f && angleDegrees <= 135f) return 1; // Right
        if (angleDegrees > 135f && angleDegrees <= 225f) return 2; // Bottom
        return 3; // Left (225-315)
    }
    
    // Gibt den Border-Index mit Hysterese zurück
    int GetBorderIndexWithHysteresis(float angleDegrees, int previousIndex)
    {
        // Definiere einen Hysterese-Winkel, um rasches Wechseln zu verhindern
        const float hysteresisAngle = 10f;
        
        // Winkelgrenzen unter Berücksichtigung der Hysterese
        float[] lowerBoundaries = new float[4] { 315f - hysteresisAngle, 45f - hysteresisAngle, 135f - hysteresisAngle, 225f - hysteresisAngle };
        float[] upperBoundaries = new float[4] { 45f + hysteresisAngle, 135f + hysteresisAngle, 225f + hysteresisAngle, 315f + hysteresisAngle };
        
        // Spezialfall für die 0/360-Grenze
        if (previousIndex == 0 && angleDegrees > 270f)
            lowerBoundaries[0] = -hysteresisAngle; // Erlaube Werte knapp unter 0°
        
        if (previousIndex == 3 && angleDegrees < 45f)
            upperBoundaries[3] = 360f + hysteresisAngle; // Erlaube Werte knapp über 360°
            
        // Überprüfe, ob wir innerhalb des erweiterten Bereichs des vorherigen Borders bleiben
        int currentIndex = previousIndex;
        
        if (previousIndex == 0) {
            // Bei Top-Border den Wrap-Around-Fall berücksichtigen
            if (!((angleDegrees >= lowerBoundaries[0] && angleDegrees <= 360f) || 
                 (angleDegrees >= 0f && angleDegrees <= upperBoundaries[0]))) {
                currentIndex = GetBorderIndex(angleDegrees);
            }
        } else {
            // Für die anderen Borders einfacher zu prüfen
            if (!(angleDegrees >= lowerBoundaries[previousIndex] && 
                  angleDegrees <= upperBoundaries[previousIndex])) {
                currentIndex = GetBorderIndex(angleDegrees);
            }
        }
        
        return currentIndex;
    }
    
    // Prüft, ob ein Objekt in einem Array existiert
    bool ObjectExistsInArray(GameObject obj, GameObject[] array)
    {
        foreach (var item in array)
        {
            if (item == obj) return true;
        }
        return false;
    }

    // Setzt die Alpha-Transparenz aller Ränder auf einen bestimmten Wert
    void SetBorderAlpha(float alphaValue)
    {
        SetImageAlpha(topBorder, alphaValue);
        SetImageAlpha(bottomBorder, alphaValue);
        SetImageAlpha(leftBorder, alphaValue);
        SetImageAlpha(rightBorder, alphaValue);
    }

    // Hilfsfunktion zum Setzen der Alpha-Transparenz eines UI Images
    void SetImageAlpha(Image image, float alphaValue)
    {
        if (image != null)
        {
            Color color = image.color;
            color.a = alphaValue;
            image.color = color;
        }
    }

    // Hilfsfunktion, um alle GameObjects in einer bestimmten Layer zu finden
    GameObject[] FindGameObjectsWithLayer(LayerMask layer)
    {
        GameObject[] goArray = FindObjectsOfType<GameObject>();
        var goList = new System.Collections.Generic.List<GameObject>();
        for (var i = 0; i < goArray.Length; i++)
        {
            if (((1 << goArray[i].layer) & layer) != 0)
            {
                goList.Add(goArray[i]);
            }
        }
        if (goList.Count == 0)
        {
            return new GameObject[0];
        }
        return goList.ToArray();
    }

    // Hilfsfunktion, um den Winkel in Grad (0-360°) um die Y-Achse zwischen zwei Vektoren zu berechnen
    float GetAngleAroundYAxis(Vector3 forward, Vector3 targetDir)
    {
        float angle = Vector3.SignedAngle(forward, targetDir, Vector3.up);
        if (angle < 0)
        {
            angle += 360; // Winkel in den Bereich 0-360° bringen
        }
        return angle;
    }
}