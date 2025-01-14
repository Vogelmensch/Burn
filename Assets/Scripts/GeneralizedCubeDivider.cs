using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class GeneralizedCubeDivider : MonoBehaviour
{
    public GameObject smallCubePrefab; // Prefab for the smaller cubes
    public float cubeSize = 0.2f; // Size of each smaller cube
    public GameObject firePrefab; // Reference to the fire prefab
    public enum DivisionAlgorithm { Generalized, Explodable }
    public DivisionAlgorithm divisionAlgorithm = DivisionAlgorithm.Generalized;

    public float explosionForce = 20f;
    public float explosionRadius = 4f;
    public float explosionUpward = 0.4f;

    public float avgIgnitionTemperature = 100f; // Average temperature for ignition
    public float temperatureVariation = 10f; // Variation in ignition temperature
    public float heatPropagationRate = 0.01f; // Rate of heat transfer
    public float heatPropagationRadius = 1f; // Radius within which heat propagates
    public float durability = 5f; // Time before cubes disappear after ignition

    // Dictionary to organize cubes into grid cells
    private Dictionary<Vector3Int, List<BurnableCube>> grid = new Dictionary<Vector3Int, List<BurnableCube>>();
    private List<BurnableCube> activeBurningCubes = new List<BurnableCube>();

    // Grouping burning cubes for large animations
    private Dictionary<BurnableCube, AnimationGroup> animationGroupsMap = new Dictionary<BurnableCube, AnimationGroup>();

    private void Awake()
    {
        cellSize = heatPropagationRadius; // One cell per propagation radius
    }
    private float cellSize;
    private List<GameObject> smallerCubes = new List<GameObject>();
    private int destroyedCubeCount = 0;

    [Range(0f, 1f)]
    public float destructionThreshold = 0.2f; // Percentage of cubes destroyed before explosion

    private Rigidbody combinedRigidbody; // Combined rigidbody for the entire object
    private bool hasExploded = false; // Prevent multiple explosions

    private Material originalMaterial; // Material from the original object

    public static List<Burnable> allBurnables = new List<Burnable>();

    // Global registry of burnable cubes
    private static List<BurnableCube> allBurnableCubes = new List<BurnableCube>();

    void Start()
    {   
        GetComponent<MeshFilter>().mesh.UploadMeshData(false);
        // Get the material from the original object's renderer
        originalMaterial = GetComponent<Renderer>().material;

        // Choose division algorithm
        if (divisionAlgorithm == DivisionAlgorithm.Generalized)
        {
            DivideUsingGeneralized();
        }
        else if (divisionAlgorithm == DivisionAlgorithm.Explodable)
        {
            DivideUsingExplodable();
        }
        // Remove the original object's renderer and collider
        Destroy(GetComponent<Renderer>());
        Destroy(GetComponent<Collider>());

        // Add Rigidbody for collective gravity
        combinedRigidbody = gameObject.AddComponent<Rigidbody>();
        combinedRigidbody.mass = smallerCubes.Count * cubeSize;
    }

    private void DivideUsingGeneralized()
    {
        // Get object bounds
        Bounds bounds = GetComponent<Renderer>().bounds;
        Vector3 size = bounds.size;

        // Calculate the number of smaller cubes in each dimension
        int cubesX = Mathf.FloorToInt(size.x / cubeSize);
        int cubesY = Mathf.FloorToInt(size.y / cubeSize);
        int cubesZ = Mathf.FloorToInt(size.z / cubeSize);

        Vector3 startOffset = bounds.min; // Start from the minimum bounds

        for (int x = 0; x < cubesX; x++)
        {
            for (int y = 0; y < cubesY; y++)
            {
                for (int z = 0; z < cubesZ; z++)
                {
                    // Calculate the position for each smaller cube
                    Vector3 position = startOffset + new Vector3(x * cubeSize, y * cubeSize, z * cubeSize);

                    // Check if the position is inside the original object (for non-rectangular shapes)
                    if (IsInsideOriginalObject(position))
                    {
                        CreateCube(position);
                    }
                }
            }
        }
    }

    private void DivideUsingExplodable()
    {
        Bounds bounds = GetComponent<Renderer>().bounds;

        int cubesX = Mathf.FloorToInt(bounds.size.x / cubeSize);
        int cubesY = Mathf.FloorToInt(bounds.size.y / cubeSize);
        int cubesZ = Mathf.FloorToInt(bounds.size.z / cubeSize);

        Vector3 pivotOffset = bounds.min + new Vector3(
            (bounds.size.x - cubesX * cubeSize) / 2f,
            (bounds.size.y - cubesY * cubeSize) / 2f,
            (bounds.size.z - cubesZ * cubeSize) / 2f
        );

        for (int x = 0; x < cubesX; x++)
        {
            for (int y = 0; y < cubesY; y++)
            {
                for (int z = 0; z < cubesZ; z++)
                {
                    Vector3 position = pivotOffset + new Vector3(x * cubeSize, y * cubeSize, z * cubeSize);
                    CreateCube(position);
                }
            }
        }
    }

    private bool IsInsideOriginalObject(Vector3 position)
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        return IsInsideMesh(position, mesh);
    }

    private bool IsInsideMesh(Vector3 point, Mesh mesh)
    {
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        float totalSolidAngle = 0f;

        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 v0 = transform.TransformPoint(vertices[triangles[i]]);
            Vector3 v1 = transform.TransformPoint(vertices[triangles[i + 1]]);
            Vector3 v2 = transform.TransformPoint(vertices[triangles[i + 2]]);

            totalSolidAngle += ComputeTriangleSolidAngle(point, v0, v1, v2);
        }

        // A point is inside if the total solid angle is approximately 4π
        return Mathf.Abs(totalSolidAngle - 4 * Mathf.PI) < 4 * Mathf.PI - 0.01f;
    }

    private float ComputeTriangleSolidAngle(Vector3 p, Vector3 v0, Vector3 v1, Vector3 v2)
    {
        Vector3 a = v0 - p;
        Vector3 b = v1 - p;
        Vector3 c = v2 - p;

        float la = a.magnitude;
        float lb = b.magnitude;
        float lc = c.magnitude;

        float denominator = la * lb * lc +
                            Vector3.Dot(a, b) * lc +
                            Vector3.Dot(b, c) * la +
                            Vector3.Dot(c, a) * lb;

        float numerator = Vector3.Dot(a, Vector3.Cross(b, c));

        return Mathf.Atan2(numerator, denominator);
    }
    
    private void CreateCube(Vector3 position)
    {
        GameObject piece = Instantiate(smallCubePrefab);
        piece.transform.position = position;
        piece.transform.localScale = Vector3.one * cubeSize;

        BurnableCube burnable = piece.AddComponent<BurnableCube>();

        burnable.Initialize(
            avgIgnitionTemperature + UnityEngine.Random.Range(-temperatureVariation, temperatureVariation),
            durability,
            firePrefab,
            this // Pass reference to GeneralizedCubeDivider
        );
        burnable.OnDestroyed += OnCubeDestroyed;

        allBurnableCubes.Add(burnable);
        smallerCubes.Add(piece);
        // Add cube to the grid
        AddCubeToGrid(burnable);

        piece.transform.SetParent(transform);
    }

    private void OnCubeDestroyed(GameObject cube)
    {
        destroyedCubeCount++;

        // End animation when cube is destroyed
        BurnableCube burnableCube = cube.GetComponent<BurnableCube>();
        if (burnableCube != null)
        {
            AnimationGroup group = animationGroupsMap[burnableCube];
            group.numberOfCubes--;
            group.UpdateFireEffect(animationGroupsMap);
            animationGroupsMap.Remove(burnableCube);
            if (group.numberOfCubes > 0)
            {
                group.AssignNewTransform(animationGroupsMap);
            } 
        }

        // Check if destruction threshold is reached
        if (!hasExploded && (float)destroyedCubeCount / smallerCubes.Count >= destructionThreshold)
        {
            Explode();
        }
    }

    private void Explode()
    {
        hasExploded = true;

        // Detach and explode all smaller cubes
        foreach (GameObject cube in smallerCubes)
        {
            if (cube != null)
            {
                cube.transform.SetParent(null); // Detach from parent

                // Add Rigidbody for explosion physics
                Rigidbody rb = cube.GetComponent<Rigidbody>();
                if (rb == null)
                {
                    rb = cube.AddComponent<Rigidbody>();
                }

                // Apply explosion force
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, explosionUpward);
            }
        }

        // Remove the combined rigidbody
        Destroy(combinedRigidbody);
    }

    private void SpreadFire()
    {
        List<BurnableCube> newlyBurningCubes = new List<BurnableCube>();
        List<BurnableCube> cubesToRemove = new List<BurnableCube>();

        foreach (BurnableCube burningCube in activeBurningCubes)
        {
            if (burningCube == null || burningCube.BurnDurationExceeded())
            {
                cubesToRemove.Add(burningCube); // Mark for removal
                continue;
            }

            Vector3Int burningCell = GetGridPosition(burningCube.transform.position);

            // Create new group for the cube if none exists
            if (!animationGroupsMap.ContainsKey(burningCube))
            {
                AnimationGroup newGroup = new AnimationGroup(1, burningCube.GetTopCenter(), firePrefab, burningCube.transform);
                animationGroupsMap.Add(burningCube, newGroup);
            }

            // Used to add the cube to the largest group (i.e. the largest fire) in its neighbourhood
            AnimationGroup currentLargestGroup = animationGroupsMap[burningCube];

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    for (int z = -1; z <= 1; z++)
                    {
                        Vector3Int neighborCell = burningCell + new Vector3Int(x, y, z);

                        if (grid.TryGetValue(neighborCell, out List<BurnableCube> neighbors))
                        {
                            foreach (BurnableCube neighbor in neighbors)
                            {
                                // Assign cube to the largest animation group
                                if (neighbor != null && 
                                    neighbor.OnFire() && 
                                    animationGroupsMap.ContainsKey(neighbor) && 
                                    animationGroupsMap[neighbor].numberOfCubes >= currentLargestGroup.numberOfCubes)
                                {
                                    currentLargestGroup = animationGroupsMap[neighbor];
                                }

                                if (neighbor == null || neighbor.OnFire() || neighbor.BurnDurationExceeded()) continue;

                                float distance = Vector3.Distance(burningCube.transform.position, neighbor.transform.position);
                                if (distance <= heatPropagationRadius && HasUncoveredSide(neighbor))
                                {
                                    float heatTransfer = heatPropagationRate / Mathf.Max(distance, 0.1f);
                                    neighbor.IncreaseTemperature(heatTransfer * Time.deltaTime);

                                    if (neighbor.OnFire())
                                    {
                                        newlyBurningCubes.Add(neighbor);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (currentLargestGroup != animationGroupsMap[burningCube])
            {
                animationGroupsMap[burningCube].StopFireEffect();
                currentLargestGroup.numberOfCubes += 1;
                currentLargestGroup.UpdatePosition(burningCube.GetTopCenter());
                currentLargestGroup.UpdateFireEffect(animationGroupsMap);
                animationGroupsMap[burningCube] = currentLargestGroup;
            }

        }

        // Add newly burning cubes after iteration
        activeBurningCubes.AddRange(newlyBurningCubes);

        // Remove burned-out or null cubes after iteration
        foreach (BurnableCube cube in cubesToRemove)
        {
            RemoveCubeFromGrid(cube);
        }

        activeBurningCubes.RemoveAll(cube => cube == null || cube.BurnDurationExceeded());
    }


    private void Update()
    {
        // Process fire spread
        SpreadFire();
        // Update moving cubes in the grid
        UpdateMovingCubes();

        // Update fire effects
        foreach (AnimationGroup animationGroup in animationGroupsMap.Values)
        {
            animationGroup.UpdateFireEffect(animationGroupsMap);
        }
    }

    private Vector3Int GetGridPosition(Vector3 position)
    {
        return new Vector3Int(
            Mathf.FloorToInt(position.x / cellSize),
            Mathf.FloorToInt(position.y / cellSize),
            Mathf.FloorToInt(position.z / cellSize)
        );
    }

    public void AddCubeToGrid(BurnableCube cube)
    {
        Vector3Int cell = GetGridPosition(cube.transform.position);

        if (!grid.ContainsKey(cell))
        {
            grid[cell] = new List<BurnableCube>();
        }

        grid[cell].Add(cube);
    }

    public void RemoveCubeFromGrid(BurnableCube cube)
    {
        if (cube == null) return; // Ensure the cube reference is valid

        Vector3Int cell = GetGridPosition(cube.transform.position);

        if (grid.TryGetValue(cell, out List<BurnableCube> cellCubes))
        {
            cellCubes.Remove(cube);

            // Clean up empty cells
            if (cellCubes.Count == 0)
            {
                grid.Remove(cell);
            }
        }

        activeBurningCubes.Remove(cube); // Remove from active burning list, if present
    }

private bool HasUncoveredSide(BurnableCube cube) 
    {
        Vector3[] directions = {
            Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back
        };

        foreach (Vector3 dir in directions)
        {
            Ray ray = new Ray(cube.transform.position, dir);
            if (!Physics.Raycast(ray, transform.localScale.x * 1.1f))
            {
                return true; // At least one side is uncovered
            }
        }

        return false; // All sides are covered
    }

    private void UpdateMovingCubes()
    {
        foreach (BurnableCube cube in allBurnableCubes)
        {
            if (cube == null) continue;

            Vector3Int currentCell = GetGridPosition(cube.transform.position);
            if (!grid.TryGetValue(currentCell, out List<BurnableCube> cellCubes) || !cellCubes.Contains(cube))
            {
                // Cube has moved to a new cell, update grid
                RemoveCubeFromGrid(cube);
                AddCubeToGrid(cube);
            }
        }
    }

    public void AddToActiveBurningList(BurnableCube cube)
    {
        if (!activeBurningCubes.Contains(cube))
        {
            activeBurningCubes.Add(cube);
        }
    }

    private void RemoveCubeFromActiveBurningList(BurnableCube cube)
    {
        activeBurningCubes.Remove(cube);
    }


    /* 
        An AnimationGroup is a group of burning blocks located near each other.
        They use one big single flame effect together.
        The blocks which belong to a group are not stored in the group itself;
        instead, they are stored in the dictionary animationGroupsMap (see at class declaration above).
    */
    public class AnimationGroup
    {
        public int numberOfCubes;
        public Vector3 position;

        private GameObject fireEffectInstance;
        private GameObject firePrefab;

        private Transform transform;

        public AnimationGroup(int n, Vector3 pos, GameObject prefab, Transform tf)
        {
            this.numberOfCubes = n;
            this.position = pos;
            this.firePrefab = prefab;
            this.transform = tf;
        }

        public void UpdateFireEffect(Dictionary<BurnableCube, AnimationGroup> groupsMap)
        {
            if (numberOfCubes == 0)
            {
                StopFireEffect();
            }
            else if (fireEffectInstance == null)
            {
                StartFireEffect();
            }
            else if (fireEffectInstance.transform.parent == null)
            {
                AssignNewTransform(groupsMap);
            }
            else
            {
                fireEffectInstance.transform.position = position;
                fireEffectInstance.transform.localScale = Vector3.one * (int) (Math.Log(numberOfCubes) + 1);
            }
        }

        public void StopFireEffect()
        {
            if (fireEffectInstance != null)
            {
                Destroy(fireEffectInstance);
                fireEffectInstance = null; // Clear reference
            }
        }

        private void StartFireEffect()
        {
            if (firePrefab != null)
            {
                fireEffectInstance = Instantiate(firePrefab, position, Quaternion.identity, transform);
                fireEffectInstance.transform.localScale = Vector3.one * (int)(Math.Log(numberOfCubes) + 1);
            }
            else
            {
                Debug.LogWarning("Fire prefab is not assigned!");
            }
        }

        // Each animation needs one block of the group to hold the transform.
        // This means that every animation is linked to a specific cube.
        // If this cube gets destroyed, the animation needs to be inherited to another cube of the group.
        // This is why this function is called on a block's death (above in OnCubeDestroyed).
        public void AssignNewTransform(Dictionary<BurnableCube, AnimationGroup> groupsMap)
        {
            bool transformFound = false;

            // Assign new transform
            foreach (BurnableCube activeCube in groupsMap.Keys)
            {
                if (groupsMap[activeCube] == this)
                {
                    transformFound = true;
                    // Quickly assign a random block in the group as a parent, so the transform gets inherited to it
                    // This is basically like marrying an old person just to inherit their money when they die
                    fireEffectInstance.transform.SetParent(activeCube.transform);
                    break;
                }
            }
            if (!transformFound)
            {
                StopFireEffect();
            }
        }

        public void UpdatePosition(Vector3 pos)
        {
            position = (position * (numberOfCubes - 1) + pos) / numberOfCubes;
        }
    }
}