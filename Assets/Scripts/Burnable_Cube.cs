using System.Collections;
using UnityEngine;

public class BurnableCube : MonoBehaviour
{
    private float durability; // Time before the cube is destroyed after ignition
    public GameObject firePrefab; // Reference to the fire prefab
    private GameObject fireEffectInstance; // Instance of the fire effect

    public delegate void CubeDestroyedHandler(GameObject cube);
    public event CubeDestroyedHandler OnDestroyed;

    public float temperature = 0f; // Current temperature of the cube
    private float ignitionTemperature; // Specific temperature this cube needs to ignite
    public bool isOnFire = false; // Whether the cube is on fire
    private float burnDuration = 0f; // Timer to track burn duration
    private GeneralizedCubeDivider divider; // Reference to the divider

    public void Initialize(float ignitionTemperature, float durability, GameObject firePrefab, GeneralizedCubeDivider divider)
    {
        this.ignitionTemperature = ignitionTemperature;
        this.durability = durability;
        this.firePrefab = firePrefab;
        this.divider = divider; // Cache the reference
    }


    private void Update()
    {
        if (isOnFire)
        {
            // If ignited, count down to destruction
            burnDuration += Time.deltaTime;
            if (burnDuration >= durability)
            {
                DestroyCube();
            }
        }
        else
        {
            // Check if the cube should ignite based on its temperature
            if (temperature >= ignitionTemperature)
            {
                Ignite();
            }
        }
    }

    public void IncreaseTemperature(float amount)
    {
        if (!isOnFire)
        {
            temperature += amount;
        }
    }

    public void Ignite()
    {
        if (!isOnFire)
        {
            isOnFire = true;

            // Use the cached divider reference to add this cube to the active burning list
            if (divider != null)
            {
                divider.AddToActiveBurningList(this);
            }
        }
    }

    private void StartFireEffect()
    {
        if (firePrefab != null)
        {
            fireEffectInstance = Instantiate(firePrefab, GetTopCenter(), Quaternion.identity, transform);
            fireEffectInstance.transform.rotation = Quaternion.Euler(-90, 0, 0); // Ensure fire points upward
        }
        else
        {
            Debug.LogWarning("Fire prefab is not assigned!");
        }
    }

    public Vector3 GetTopCenter()
    {
        Renderer renderer = GetComponent<Renderer>();
        return renderer.bounds.center + Vector3.up * renderer.bounds.extents.y;
    }

    private void DestroyCube()
    {
        if (OnDestroyed != null)
        {
            OnDestroyed.Invoke(gameObject);
        }
        Destroy(gameObject);
    }
    public bool OnFire()
    {
        return isOnFire;
    }

    public bool BurnDurationExceeded()
    {
        if (isOnFire)
        {
            burnDuration += Time.deltaTime;
            if (burnDuration >= durability)
            {
                return true;
            }
        }

        return false;
    }

    private float IgnitionTemperature()
    {
        // Return the cube's specific ignition temperature
        return ignitionTemperature;
    }
}
