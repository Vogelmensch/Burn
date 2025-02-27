using UnityEngine;

public class waterBeam : MonoBehaviour
{
    public string waterName = "waterBeamPrefab";
    public bool isOn = false;
    public float sprayDistance = 100; // distance the water is sprayed
    public LayerMask BurningLayer = 6;
    public float timeBetweenHits = 0.25f; // time in between reducing temperature so it doesnt happen every frame

    private bool canHit = true; // helper variable to save if curretnly a temperature can be reduced
    private GameObject waterPrefab;
    private GameObject waterEffectInstance;
    private float duration = 10;
    private float cooldown = 3;

    void Update()
    {
        if (isOn && canHit) {
            // cast a ray forward 
            // TODO: change to the cone of the water effect instead of using a ray
            Ray ray = new Ray(transform.position, transform.forward);
            
            // Draw the ray in the Scene view for visualization
            Debug.DrawRay(ray.origin, ray.direction * sprayDistance, Color.blue);

            // check if the ray hits anything
            if (Physics.Raycast(ray, out RaycastHit hit, sprayDistance)) {
                // Check if the hit object is of type burnable
                Burnable burnable = hit.collider.GetComponent<Burnable>();
                if (burnable != null) {
                    // reduce temperature
                    burnable.RainHit();
                    canHit = false;
                    Invoke(nameof(ResetHit), timeBetweenHits);
                }
            }
        }
    }

    public void Activate() {
        isOn = true;
        waterPrefab = Resources.Load<GameObject>(waterName);
        if (waterPrefab != null)
        {
            Vector3 position = GetFrontCenter() + (transform.forward * 0.25f); // Adjust the offset as needed
            waterEffectInstance = Instantiate(waterPrefab, position, Quaternion.identity, transform);

            // ensure the water effect is facing the right direction
            waterEffectInstance.transform.rotation = transform.rotation * Quaternion.Euler(270, 0, 0);

            // scale the water effect to the desired length
            waterEffectInstance.transform.localScale = new Vector3(1, sprayDistance / 50, 1);
            // TODO: here we wait for 10 sec and reset the water but the object isnt actually put out for 10 seconds
            Invoke(nameof(Reset), cooldown);
        }
        else 
        {
            Debug.LogError("Fire prefab is null, cannot instantiate fire effect.");
        }
    }

    void Reset() {
        isOn = false;
        Destroy(waterEffectInstance);
        ResetHit();
        Activate();
    }

    void ResetHit()
    {
        canHit = true;
    }

    public void Deactivate() {
        isOn = false;
        canHit = true;
        Destroy(waterEffectInstance);
    }

    protected virtual Vector3 GetFrontCenter()
    {
        Renderer renderer = GetComponent<Renderer>();
        return renderer.bounds.center + Vector3.forward * renderer.bounds.extents.x;
    }
}
