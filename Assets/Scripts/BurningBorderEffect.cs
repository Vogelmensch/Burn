using UnityEngine;
using UnityEngine.UI;

public class BurningBorderEffect : MonoBehaviour
{
    public LayerMask burningLayer; // Der Layer, der alle brennenden Objekte enthält
    public Image topBorder;       // UI Image für den oberen Bildschirmrand
    public Image bottomBorder;    // UI Image für den unteren Bildschirmrand
    public Image leftBorder;      // UI Image für den linken Bildschirmrand
    public Image rightBorder;     // UI Image für den rechten Bildschirmrand
    public float edgeThreshold = 0.8f; // Schwellenwert, ab wann der Rand als "nah genug" am Bildschirmrand betrachtet wird

    private Transform playerTransform; // Transform des Spielers (oder der Kamera)

    void Start()
    {
        // Finde das Transform des Hauptspielers (angenommen, es ist die Hauptkamera)
        playerTransform = Camera.main.transform;

        // Stelle sicher, dass die Border Images vorhanden sind
        if (topBorder == null || bottomBorder == null || leftBorder == null || rightBorder == null)
        {
            Debug.LogError("Nicht alle Border Images sind im Inspector zugewiesen!");
            return;
        }

        // Stelle sicher, dass die Border Images anfänglich unsichtbar sind
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
            // Berechne die Richtung vom Spieler zum brennenden Objekt (normalisiert)
            Vector3 directionToBurningObject = (burningObject.transform.position - playerTransform.position).normalized;

            // Überprüfe, ob das brennende Objekt in der Nähe eines Bildschirmrandes ist und aktiviere den entsprechenden Rand
            if (directionToBurningObject.z > edgeThreshold) // Objekt ist 'vor' dem Spieler (in Blickrichtung) - Oberer Rand
            {
                SetBorderAlphaForDirection(Vector3.forward, 0.5f); // Oberer Rand aktivieren
            }
            if (directionToBurningObject.z < -edgeThreshold) // Objekt ist 'hinter' dem Spieler - Unterer Rand
            {
                 SetBorderAlphaForDirection(Vector3.back, 0.5f); // Unterer Rand aktivieren
            }
            if (directionToBurningObject.x > edgeThreshold) // Objekt ist rechts vom Spieler - Rechter Rand
            {
                 SetBorderAlphaForDirection(Vector3.right, 0.5f); // Rechter Rand aktivieren
            }
            if (directionToBurningObject.x < -edgeThreshold) // Objekt ist links vom Spieler - Linker Rand
            {
                 SetBorderAlphaForDirection(Vector3.left, 0.5f); // Linker Rand aktivieren
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

    // Setzt die Alpha-Transparenz für einen bestimmten Rand basierend auf der Richtung
    void SetBorderAlphaForDirection(Vector3 direction, float alphaValue)
    {
        if (direction == Vector3.forward) SetImageAlpha(topBorder, alphaValue);
        else if (direction == Vector3.back) SetImageAlpha(bottomBorder, alphaValue);
        else if (direction == Vector3.left) SetImageAlpha(leftBorder, alphaValue);
        else if (direction == Vector3.right) SetImageAlpha(rightBorder, alphaValue);
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
}