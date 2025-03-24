using UnityEngine;
using System.Linq;
public class explodableRock : MonoBehaviour
{
    private float cubeSizeMin = 0.1f;
    private int maxNumberOfCubes = 10;
    private float cubeSize = 0.2f;
    private float explosionForce = 5f;
    private float explosionRadius = 4;
    private float explosionUpward = 0.4f;

    public GameObject cubePrefab;
    // completely robbed this from burnable.cs
    public void Explode()
    {
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
                    if (cubePrefab != null)
                    {
                        float randomNumber = UnityEngine.Random.Range(0.0f, 1.0f);
                        if (randomNumber < cubeSpawnChance)
                            CreateCube(position);
                    }
                }
            }
        }
        Destroy(gameObject);
    }
    private void CreateCube(Vector3 position)
    {
        GameObject cube = Instantiate(cubePrefab, position, Quaternion.identity);
        cube.transform.localScale = Vector3.one * cubeSize;

        Rigidbody rb = cube.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = cube.AddComponent<Rigidbody>();
        }

        rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, explosionUpward);
    }
}
