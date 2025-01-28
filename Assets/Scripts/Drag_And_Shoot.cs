using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class DragAndAutoThrow : MonoBehaviour
{
    private Rigidbody rb;
    private bool isDragging = false;
    private Vector3 dragStartPos;
    private Vector3 currentMouseWorldPos;

    private float dragStartTime;
    private float throwTime; 
    private float totalHoldTime; 

    public float throwForceMultiplier = 10f; 
    public float maxThrowForce = 5f;
    public float maxDragDistance = 5f; // Maximale Drag-Distanz
    public float maxHoldTime = 1.5f; 

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (isDragging)
        {
            // Berechne die Haltezeit
            float holdTime = Time.time - dragStartTime;

            // Logge die Haltezeit und die Drag-Distanz
            float dragDistance = Vector3.Distance(dragStartPos, currentMouseWorldPos);

            // Wenn die Haltezeit zu lang wird, das Objekt automatisch werfen
            if (holdTime > maxHoldTime)
            {
                ThrowObject(dragStartPos, currentMouseWorldPos);
                isDragging = false;
            }
            else
            {
                // Verfolge die Mausbewegung während des Dragging
                currentMouseWorldPos = GetMouseWorldPosition();

                // Berechne die Distanz zwischen Startpunkt und der aktuellen Position
                float distanceFromStart = Vector3.Distance(dragStartPos, currentMouseWorldPos);

                // Wenn maximale Drag-Distanz erreicht, werfe das Objekt sofort
                if (distanceFromStart > maxDragDistance)
                {
                    // Berechne die Position innerhalb der maximalen Drag-Distanz
                    Vector3 direction = (currentMouseWorldPos - dragStartPos).normalized;
                    currentMouseWorldPos = dragStartPos + direction * maxDragDistance;

                    // Wurf sofort auslösen
                    ThrowObject(dragStartPos, currentMouseWorldPos);
                    isDragging = false; // Stoppe das Dragging
                }
                else
                {
                    // Objekt weiterhin bewegen
                    rb.MovePosition(currentMouseWorldPos);
                }
            }
        }
    }






    private void OnMouseDown()
    {
        // Start des Dragging
        isDragging = true;
        dragStartPos = GetMouseWorldPosition();
        dragStartTime = Time.time; // Setze die Startzeit des Haltevorgangs
        totalHoldTime = 0f; // Zurücksetzen der gesamten Haltezeit
        rb.isKinematic = true; // Deaktiviere Physik während des Dragging
    }

    private void OnMouseUp()
    {
        if (isDragging)
        {
            // Loslassen und Wurf auslösen
            ThrowObject(dragStartPos, GetMouseWorldPosition());
            isDragging = false;
        }
    }

    private void ThrowObject(Vector3 startPos, Vector3 releasePos)
    {
        rb.isKinematic = false; 

        // Berechne Richtung und Wurfkraft
        Vector3 dragVector = releasePos - startPos;
        Vector3 forceDirection = dragVector.normalized;

        // Berechne Wurfkraft basierend auf der Drag-Distanz
        float dragDistance = dragVector.magnitude;
        float scaledForce = Mathf.Min(dragDistance, maxDragDistance) * throwForceMultiplier;

        // Begrenze maximale Wurfkraft
        scaledForce = Mathf.Min(scaledForce, maxThrowForce);

        // Berechne gesamte Haltezeit
        totalHoldTime = Time.time - dragStartTime;

        // Logging 
        Debug.Log($"Throw Time: {Time.time}s, Total Hold Time: {totalHoldTime}s");
        Debug.Log($"Drag Distance: {dragDistance}, Scaled Force: {scaledForce}");

        // Wurfkraft anwenden
        Vector3 force = forceDirection * scaledForce;
        rb.AddForce(force, ForceMode.Impulse);

        // Logging nach Wurf
        Debug.Log($"Magnitude: {force.magnitude}, Time Held: {totalHoldTime}s, Drag Distance: {dragDistance}");
    }

    private Vector3 GetMouseWorldPosition()
    {
        // Mausposition in Weltkoordinaten
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = Camera.main.WorldToScreenPoint(transform.position).z;

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);

        return worldPos;
    }
}
