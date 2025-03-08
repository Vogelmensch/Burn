using UnityEngine;
using System.Collections;

public class waterBeam : MonoBehaviour
{
    public bool activateOnStart = false;
    public string waterName = "waterBeamPrefab";
    private bool isOn = false;
    public float sprayDistance = 100; // distance the water is sprayed
    public LayerMask BurningLayer = 6;
    private float timeBetweenHits = 0.5f; // time in between reducing temperature so it doesnt happen every frame
    private bool canHit = true; // helper variable to save if curretnly a temperature can be reduced
    private GameObject waterPrefab;
    private GameObject waterEffectInstance;
    private float duration = 9; // duration of the water beam
    private float cooldown = 5; // cooldown before waterbeam turns back on
    private float cooldownAmount = 20; // amount to reduce temperature by
    void Start()
    {
        if(activateOnStart) {
            Activate();
        }
    }
    private void Update()
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
                    burnable.WaterHit(cooldownAmount);
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
            StartCoroutine(TurnOffAfterDelay());            
        }
        else 
        {
            Debug.LogError("Fire prefab is null, cannot instantiate fire effect.");
        }
    }

    private IEnumerator TurnOffAfterDelay()
    {
        yield return new WaitForSeconds(duration);
        isOn = false;
        Destroy(waterEffectInstance);
        ResetHit();
        yield return new WaitForSeconds(cooldown);
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
        StopCoroutine(TurnOffAfterDelay());
    }

    protected virtual Vector3 GetFrontCenter()
    {
        Renderer renderer = GetComponent<Renderer>();
        return renderer.bounds.center + Vector3.forward * renderer.bounds.extents.x;
    }
}
