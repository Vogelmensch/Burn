using UnityEngine;
using UnityEngine.InputSystem;

public class FireballSpawner : MonoBehaviour
{
    public GameObject fireballPrefab;
    public Transform cameraTransform;


    // Can be modified by the player
    // Range: [~100, ~350]
    public int forceStrength = 150;
    public int forceLowerLimit = 100;
    public int forceUpperLimit = 350;
    public double forceChangeRate = 700;
    public float fireAccessRadius = 2f;
    public float fireballThrowTemperatureDecrease = 100f;
    public Vector3 spawnShift = new Vector3(0,1,0);
    public ThrowStrengthIndicator throwStrengthIndicator;
    private GameObject fireball;

    // --- new actions :) ---
    InputAction increaseStrengthAction;
    InputAction decreaseStrengthAction;
    InputAction throwAction;

    void Start()
    {
        increaseStrengthAction = InputSystem.actions.FindAction("FireballIncrease");
        decreaseStrengthAction = InputSystem.actions.FindAction("FireballDecrease");
        throwAction = InputSystem.actions.FindAction("FireballThrow");
    }

    // Update is called once per frame
    void Update()
    {
        ChangeStrength();

        if (throwAction.WasPressedThisFrame())
        {
            Burnable nearbyFire = NearbyFire(fireAccessRadius);
            if (nearbyFire != null)
            {
                nearbyFire.FeedFireball();
                fireball = SpawnBall();
                Throw(fireball);
            }
            else
            {
                StartCoroutine(throwStrengthIndicator.PrintNoFireMessage());
            }
        }

    }

    void ChangeStrength()
    {
        if (increaseStrengthAction.IsPressed())
        {
            forceStrength += (int)(forceChangeRate * Time.deltaTime);
            if (forceStrength > forceUpperLimit)
            {
                forceStrength = forceUpperLimit; // Limit force strength
            }
        }
        if (decreaseStrengthAction.IsPressed())
        {
            forceStrength -= (int)(forceChangeRate * Time.deltaTime);
            if (forceStrength < forceLowerLimit)
            {
                forceStrength = forceLowerLimit;
            }
        }
        double amount = (double)forceStrength / (double)forceUpperLimit;
        throwStrengthIndicator.SetStrengthBar(amount);
    }

    GameObject SpawnBall()
    {
        return Instantiate(fireballPrefab, cameraTransform.position + spawnShift, Quaternion.identity);
    }

    void Throw(GameObject ball)
    {
        Rigidbody rb = ball.GetComponent<Rigidbody>();
        Vector3 force = new Vector3(0, 1, 1) * forceStrength; // throwing 45° upwards
        Vector3 eulerAngles = cameraTransform.eulerAngles; // throw where camera is looking
        eulerAngles.x = 0; // don't consider tilt, only rotation in xz-plane
        eulerAngles.z = 0;
        force = Quaternion.Euler(eulerAngles) * force; // apply rotation to force
        rb.AddForce(force);
    }


    Burnable NearbyFire(float fireAccessRadius)
    {
        Collider[] nearbyObjects = Physics.OverlapSphere(cameraTransform.position, fireAccessRadius);
        foreach (Collider col in nearbyObjects)
        {
            Burnable burnable = col.GetComponent<Burnable>();
            if (burnable != null && burnable.isOnFire)
            {
                return burnable;
            }
        }
        return null;
    }
}
