using UnityEngine;

public class waterBeam : MonoBehaviour
{
    public string waterName = "waterBeamPrefab";
    public bool isOn = false;
    public float sprayDistance = 100; // distance the water is sprayed
    public LayerMask BurningLayer = 1 << 6;
    public float timeBetweenHits = 1f; // time in between reducing temperature so it doesnt happen every frame

    private bool canHit = true; // helper variable to save if curretnly a temperature can be reduced
    private GameObject waterPrefab;
    private GameObject waterEffectInstance;
    private float duration = 10;
    private float cooldown = 3;

    void Start()
    {
        Activate();
    }

    void Update()
    {
        if (isOn && canHit) {
            // cast a ray forward 
            // TODO: change to the cone of the water effect instead of using a ray
            Ray ray = new Ray(transform.position, Vector3.forward);
            // check if the ray hits anything
            if (Physics.Raycast(ray, out RaycastHit hit)) {
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

    void Activate() {
        isOn = true;
        waterPrefab = Resources.Load<GameObject>(waterName);
        if (waterPrefab != null)
        {
            waterEffectInstance = Instantiate(waterPrefab, GetFrontCenter(), Quaternion.identity, transform);
            waterEffectInstance.transform.rotation = Quaternion.Euler(270, 0, 0); // Ensure water points forward
            waterEffectInstance.transform.localScale = new Vector3(1, sprayDistance / 50, 1);
            Debug.Log("Started water effect");
            Invoke(nameof(Deactivate), duration);
        }
        else 
        {
            Debug.LogError("Fire prefab is null, cannot instantiate fire effect.");
        }
    }

    void Deactivate() {
        isOn = false;
        Destroy(waterEffectInstance);
        Debug.Log("stopped water effect");

        ResetHit();
        Invoke(nameof(Activate), cooldown);
    }

    void ResetHit()
    {
        canHit = true;
    }

    protected virtual Vector3 GetFrontCenter()
    {
        Renderer renderer = GetComponent<Renderer>();
        return renderer.bounds.center + Vector3.forward * renderer.bounds.extents.x;
    }
}
