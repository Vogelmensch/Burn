using UnityEngine;

public class CameraBob : MonoBehaviour
{
    [Header("Bobbing Settings")]
    public float bobFrequency = 2f;  // Speed of the bobbing effect
    public float bobAmplitude = 0.1f; // Height of the bobbing effect

    [Header("Shake Settings")]
    public float shakeIntensity = 0.02f; // Maximum shake displacement
    public float shakeSpeed = 15f; // Speed of shake effect

    public GameObject reference; // Reference to the player object
    private PlayerMovement playerMovement;

    private float timer = 0f;
    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.localPosition;

        // Get the PlayerMovement component from the reference object
        playerMovement = reference.GetComponent<PlayerMovement>();
        if (playerMovement == null)
        {
            Debug.LogError("PlayerMovement component not found on the reference object!");
        }
    }

    void Update()
    {
        if (playerMovement != null && playerMovement.grounded && playerMovement.rb.linearVelocity.magnitude > 0.1f)
        {
            // Bobbing Effect
            timer += Time.deltaTime * bobFrequency;
            float bobOffset = Mathf.Sin(timer) * bobAmplitude;

            // Random Shake Effect (Perlin Noise for smooth randomness)
            float xShake = (Mathf.PerlinNoise(Time.time * shakeSpeed, 0) - 0.5f) * shakeIntensity;
            float yShake = (Mathf.PerlinNoise(0, Time.time * shakeSpeed) - 0.5f) * shakeIntensity;

            // Apply both bobbing and shake
            transform.localPosition = startPosition + new Vector3(xShake, bobOffset + yShake, 0);
        }
        else
        {
            // Reset when player is not moving
            timer = 0;
            transform.localPosition = startPosition;
        }
    }
}
