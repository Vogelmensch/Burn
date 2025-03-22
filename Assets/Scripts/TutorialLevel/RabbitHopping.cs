using UnityEngine;

public class SimpleRabbitHopping : MonoBehaviour
{
    [Header("Bewegungseinstellungen")]
    public float hopSpeed = 1.0f;          // Geschwindigkeit des Hoppelns
    public float circleRadius = 0.5f;      // Radius des Kreises
    public float hopHeight = 0.1f;         // Höhe des Hoppelns
    
    private Vector3 startPosition;
    private float angle = 0;
    
    void Start()
    {
        startPosition = transform.position;
    }
    
    void Update()
    {
        // Kreisbewegung berechnen
        angle += hopSpeed * Time.deltaTime;
        
        // Position im Kreis
        float x = Mathf.Sin(angle) * circleRadius;
        float z = Mathf.Cos(angle) * circleRadius;
        
        // Hoppel-Höhe (auf und ab) mit Sinus-Welle
        float y = Mathf.Abs(Mathf.Sin(angle * 4)) * hopHeight;
        
        // Position setzen
        transform.position = startPosition + new Vector3(x, y, z);
        
        // Drehung in Bewegungsrichtung
        Vector3 lookDirection = new Vector3(Mathf.Cos(angle), 0, -Mathf.Sin(angle));
        transform.rotation = Quaternion.LookRotation(lookDirection);
    }
}