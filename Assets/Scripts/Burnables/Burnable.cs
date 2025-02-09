using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

/*
                        --- OVERVIEW OF BURNABLES ---

                Ign.Temp.   Max.Temp.   Heat.Trans. Spr.Rad.    HitPoints   Temp.Dec.atRainHit   

    Standard:   100         200         10          1           100    
    Book:       60          120         5           1           100              
    Crate:      100         200         10          1           200
    Wood Pile:  150         400         20          1.5         400
    Hay:        60          150         30          1           100
    Tree:       300         600         30          3           1000
    Stone:      200         300         10          0           450
    Door:       400         450         10          1           200         
    Roof:       300         400         2           2           250         0
*/



public class Burnable : MonoBehaviour
{
    [Header("Metadata")]
    public string fireName = "firePrefab"; // Name of the fire prefab file in the resources folder
    private GameObject firePrefab; // Reference to the fire particle  prefab
    public GameObject burningCubePrefab; // Prefab for the burning cubes

    public float colorationReduction = 100f; // Divides the interpolation % by this value
    // New fire spreading variables
    // IDEA:
    // temperatures of Burnables stay constant unless it's burning (increase) or raining (decrease)
    // Burnables transfer heat to nearby Burnables only when they're burning
    
    protected float ignitionTemperature = 100;
    protected float maxTemperature = 200;
    protected float temperatureIncreaseCoefficient = 10; // amount of temp increase per second when burning
    protected float temperatureDecreaseAtRainHit = 60; // amount of temp decrease per raindrop hit
    protected float temperatureDecreaseAtFireballThrow = 100f;
    protected float heatTransferCoefficient = 10; // amount of heat transferred to nearby objects per second when burning
    [Header("Fire Variables")]
    public float hitPoints = 100;
    public float temperature = 0;
    public float spreadRadius = 2f; // Radius for spreading
    public bool isOnFire = false;
    protected float damageCoefficient = 10; // amount of hitpoints lost per second when burning (rounded later)
    // End new variables

    private float cubeSize = 0.07f; // MAXIMAL Size of each smaller cube
    private float cubeSizeMin = 0.01f;
    private int maxNumberOfCubes = 32;

    [Header("Explosion Variables")]

    public float explosionForce = 0f; // Force of the explosion
    public float explosionRadius = 4f; // Radius of the explosion
    public float explosionUpward = 0.4f; // Upward modifier for the explosion force

    private GameObject fireEffectInstance;
    private int BurningLayer = 6;

    protected virtual void Start()
    {
        GeneralizedCubeDivider.allBurnables.Add(this);

        // Add some randomness to Hitpoints
        float deviation = hitPoints * 0.2f;
        hitPoints += UnityEngine.Random.Range(-deviation, +deviation);

        if (burningCubePrefab == null)
        {
            burningCubePrefab = Resources.Load<GameObject>("MedievalWoodCube");
        }
    }

    void OnDestroy()
    {
        if (GeneralizedCubeDivider.allBurnables.Contains(this))
        {
            GeneralizedCubeDivider.allBurnables.Remove(this);
            Debug.Log($"Removed {name} from allBurnables.");
        }
    }

    protected virtual void Update()
    {
        UpdateHelper();
    }

    protected void UpdateHelper()
    {
        // Change color of object based on temperature
        GetComponent<Renderer>().material.color = Color.Lerp(GetComponent<Renderer>().material.color, Color.red, (temperature / maxTemperature) / colorationReduction);

        // Start fire
        if (temperature >= ignitionTemperature && !isOnFire) Ignite();

        // Stop fire
        if (temperature < ignitionTemperature && isOnFire) Extinguish();

        // Destroy
        if (hitPoints <= 0)
        {
            Explode();
        }


        if (isOnFire)
        {
            temperature += temperatureIncreaseCoefficient * Time.deltaTime; // increase temp when burning

            hitPoints -= damageCoefficient * Time.deltaTime; // decrease hitpoints when burning

            if (fireEffectInstance != null)
            {
                fireEffectInstance.transform.rotation = Quaternion.Euler(0, 0, 0); // Ensure fire effect points upward
            }
            SpreadHeat();
        }

        // keep temperature in interval [0, maxTemperature]
        if (temperature < 0)
            temperature = 0;
        else if (temperature > maxTemperature)
            temperature = maxTemperature;
    }

    public void Ignite()
    {
        if (!isOnFire)
        {
            isOnFire = true;
            temperature = ignitionTemperature;
            StartFireEffect();

            GameOver gameOver = UnityEngine.Object.FindAnyObjectByType<GameOver>(); // Eventuell performance-probleme
            if (gameOver != null)
            {
                gameOver.NotifyFireStarted();
            }
        }
    }

    public void IncreaseTemperature(float amount)
    {
        temperature += amount;
    }

    // Decrease temp when raindrop hits object
    public void RainHit()
    {
        temperature -= temperatureDecreaseAtRainHit; // decrease temp when raining
    }

    public void FeedFireball()
    {
        temperature -= temperatureDecreaseAtFireballThrow;
    }


    public void Extinguish()
    {
        if (isOnFire)
        {
            isOnFire = false;
            temperature = 0;
            if (fireEffectInstance != null)
            {
                Destroy(fireEffectInstance);
            }
        }
    }

    protected virtual void StartFireEffect()
    {
        firePrefab = Resources.Load<GameObject>(fireName);
        if (firePrefab != null)
        {
            fireEffectInstance = Instantiate(firePrefab, GetTopCenter(), Quaternion.identity, transform);

            // set scale relative to world size
            Vector3 globalSize = GetComponent<Renderer>().bounds.size;

            float sum = 0;
            for (int i = 0; i < 3; i++)
            {
                sum += globalSize[i];
            }

            fireEffectInstance.transform.localScale = sum / 3 * Vector3.one;
        }
        else
        {
            Debug.LogError("Fire prefab is null, cannot instantiate fire effect.");
        }
    }

    protected virtual Vector3 GetTopCenter()
    {
        Renderer renderer = GetComponent<Renderer>();
        return renderer.bounds.center + Vector3.up * renderer.bounds.extents.y;
    }

    // Once started, the coroutine runs forever until it gets stopped by Extinguish()
    private void SpreadHeat()
    {
        Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, spreadRadius);
        foreach (Collider col in nearbyObjects)
        {
            if (col.transform != transform && col.transform.parent != transform)
            {
                Burnable burnable = col.GetComponent<Burnable>();
                if (burnable != null && !IsWallInBetween(burnable))
                {
                    float distance = Vector3.Distance(transform.position, col.transform.position);
                    burnable.IncreaseTemperature(Math.Abs(heatTransferCoefficient * Time.deltaTime * (spreadRadius - distance) / spreadRadius));
                }
            }
        }
    }

    // Checks whether between this Burnable and another is a wall
    // For it to work, you need to add specific layers to the walls
    protected bool IsWallInBetween(Burnable destination, int wallLayer = 8)
    {
        if (destination is Door)
            return false;

        Vector3 direction = destination.transform.position - transform.position;
        float distance = direction.magnitude;
        int layerMask = 1 << wallLayer;

        return Physics.Raycast(transform.position, direction, distance, layerMask);
    }

    protected virtual void Explode()
    {
        Extinguish();

        Bounds bounds = GetComponent<Renderer>().bounds;
        Vector3 size = bounds.size;

        float[] sizes = { size.x, size.y, size.z };
        float smallestEdge = sizes.Min<float>();

        // If Object is thin, the cubes are smaller
        if (smallestEdge < cubeSize && smallestEdge > cubeSizeMin)
            cubeSize = smallestEdge;

        int cubesX = Mathf.FloorToInt(size.x / cubeSize);
        int cubesY = Mathf.FloorToInt(size.y / cubeSize);
        int cubesZ = Mathf.FloorToInt(size.z / cubeSize);

        // total number of cubes should be lower than maximum:
        int totalNumberOfCubes = cubesX * cubesY * cubesZ;

        // If total number of cubes is higher in this object, the chance of actually spawning a cube is lower
        float cubeSpawnChance = maxNumberOfCubes / ((float)totalNumberOfCubes);

        Vector3 startOffset = bounds.min;

        for (int x = 0; x < cubesX; x++)
        {
            for (int y = 0; y < cubesY; y++)
            {
                for (int z = 0; z < cubesZ; z++)
                {
                    Vector3 position = startOffset + new Vector3(x * cubeSize, y * cubeSize, z * cubeSize);
                    if (burningCubePrefab != null)
                    {
                        float randomNumber = UnityEngine.Random.Range(0.0f, 1.0f);
                        if (randomNumber < cubeSpawnChance)
                            CreateCube(position);
                    }
                }
            }
        }

        GeneralizedCubeDivider.allBurnables.Remove(this);
        Destroy(gameObject);
    }

    // Create burning or non-burning cube
    // added non-burning for water-barrels
    private void CreateCube(Vector3 position)
    {
        GameObject cube = Instantiate(burningCubePrefab, position, Quaternion.identity);
        cube.transform.localScale = Vector3.one * cubeSize;

        Rigidbody rb = cube.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = cube.AddComponent<Rigidbody>();
        }

        rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, explosionUpward);
    }
}
