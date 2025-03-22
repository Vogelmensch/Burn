using UnityEngine;
using UnityEngine.UI;

public class BurningBorderEffect : MonoBehaviour
{
    public LayerMask burningLayer; 
    public Image topBorder;       
    public Image bottomBorder;    
    public Image leftBorder;      
    public Image rightBorder;     

    private Transform playerTransform; 

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

        // Setze alle Ränder anfänglich auf unsichtbar
        SetBorderAlpha(0f);

        // Finde alle GameObjects auf dem Burning Layer
        GameObject[] burningObjects = FindGameObjectsWithLayer(burningLayer);

        foreach (GameObject burningObject in burningObjects)
        {
            // Berechne die Richtung vom Spieler zum brennenden Objekt
            Vector3 directionToBurningObject = (burningObject.transform.position - playerTransform.position);

            // Berechne den Winkel in Grad (0-360°) 
            float angleDegrees = GetAngleAroundYAxis(playerTransform.forward, directionToBurningObject);

            //Debug.Log($"Winkel zum brennenden Objekt {burningObject.name}: {angleDegrees}");

            // Zuordnung zu den Rändern
            if (angleDegrees > 315f || angleDegrees <= 45f) // Vor dem Spieler (TopBorder) ~ 315° bis 45°
            {
                SetImageAlpha(topBorder, 0.5f);
            }
            else if (angleDegrees > 45f && angleDegrees <= 135f) // Rechts vom Spieler (RightBorder) ~ 45° bis 135°
            {
                SetImageAlpha(rightBorder, 0.5f);
            }
            else if (angleDegrees > 135f && angleDegrees <= 225f) // Hinter dem Spieler (BottomBorder) ~ 135° bis 225°
            {
                SetImageAlpha(bottomBorder, 0.5f);
            }
            else if (angleDegrees > 225f && angleDegrees <= 315f) // Links vom Spieler (LeftBorder) ~ 225° bis 315°
            {
                SetImageAlpha(leftBorder, 0.5f);
            }
        }
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