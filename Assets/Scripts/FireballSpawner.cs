using UnityEngine;

public class FireballSpawner : MonoBehaviour
{
    public GameObject fireballPrefab;
    public Transform cameraTransform;


    // Can be modified by the player
    // Range: [~100, ~350]
    public int forceStrength = 150;
    public int forceLowerLimit = 100;
    public int forceUpperLimit = 350;
    public double forceChangeRate = 300;
    public float fireAccessRadius = 2f;
    public ThrowStrengthIndicator throwStrengthIndicator;
    private GameObject fireball;
    private bool upPressed = false;
    private bool downPressed = false;
    readonly KeyCode upKey = KeyCode.U;
    readonly KeyCode downKey = KeyCode.J;

    // Update is called once per frame
    void Update()
    {
        GetStrengthInput();
        ChangeStrength();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (IsFireNearby(fireAccessRadius))
            {
                fireball = SpawnBall();
                Throw(fireball);
            }
            else
            {
                StartCoroutine(throwStrengthIndicator.PrintNoFireMessage());
            }
        }

    }

    void GetStrengthInput()
    {
        if (Input.GetKeyDown(upKey))
        {
            upPressed = true;
        }
        if (Input.GetKeyUp(upKey))
        {
            upPressed = false;
        }
        if (Input.GetKeyDown(downKey))
        {
            downPressed = true;
        }
        if (Input.GetKeyUp(downKey))
        {
            downPressed = false;
        }
    }

    void ChangeStrength()
    {
        if (upPressed)
        {
            forceStrength += (int)(forceChangeRate * Time.deltaTime);
            if (forceStrength > forceUpperLimit)
            {
                forceStrength = forceUpperLimit; // Limit force strength
            }
        }
        if (downPressed)
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
        return Instantiate(fireballPrefab, cameraTransform.position, Quaternion.identity);
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

    bool IsFireNearby(float fireAccessRadius)
    {
        Collider[] nearbyObjects = Physics.OverlapSphere(cameraTransform.position, fireAccessRadius);
        foreach (Collider col in nearbyObjects)
        {
            Burnable burnable = col.GetComponent<Burnable>();
            if (burnable != null && burnable.isOnFire)
            {
                return true;
            }
        }
        return false;
    }
}
