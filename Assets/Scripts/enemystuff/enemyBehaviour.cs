using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class enemyBehaviour : MonoBehaviour
{

    [Header("Components")]
    public NavMeshAgent agent;
    private Transform target;
    public LayerMask whatIsGround;

    [Header("Patroling")]
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    [Header("Attacking")]
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    // States

    public float sightRange, attackRange;
    public bool fireInSightRange, fireInAttackRange;

    [SerializeField] private int BurningLayer = 6;
    private int BurningLayerMask;

    private const int IgnoreLayer = 2;
    private float viewAngle = 60f;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        BurningLayerMask = 1 << BurningLayer;
    }

    private List<Burnable> CheckForBurningObjects(float range)
    {
        List<Burnable> burnables = new List<Burnable>();
        Collider[] colliders = Physics.OverlapSphere(transform.position, range, BurningLayerMask);

        foreach (Collider col in colliders)
        {
            Burnable burnable = col.GetComponent<Burnable>();
            if (burnable != null && burnable.isOnFire)
            {
                Vector3 directionToBurnable = (burnable.transform.position - transform.position).normalized;
                float angleToBurnable = Vector3.Angle(transform.forward, directionToBurnable);

                // Check if the burnable is within the cone of vision
                if (angleToBurnable <= viewAngle)
                {
                    // Optional: Perform a raycast to ensure there are no obstacles
                    if (!Physics.Raycast(transform.position, directionToBurnable, out RaycastHit hit, range) || hit.collider.GetComponent<Burnable>() == burnable)
                    {
                        Debug.DrawRay(transform.position, directionToBurnable * range, Color.red);
                        burnables.Add(burnable);
                    }
                }
            }
        }

        return burnables;
    }

    private GameObject getClosest(List<Burnable> targets)
    {
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (Burnable target in targets)
        {
            if (target == null) continue;
            if (!target.isOnFire) continue;

            Vector3 diff = target.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = target.gameObject;
                distance = curDistance;
            }
        }
        return closest;
    }
    private void Patroling()
    {
        agent.isStopped = false;
        if (!walkPointSet) SearchWalkPoint();
        if (walkPointSet)
            agent.SetDestination(walkPoint);
        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }
    private void SearchWalkPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * walkPointRange; // Pick a random direction in a sphere
        randomDirection += transform.position; // Offset it by the enemy's current position

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, walkPointRange, NavMesh.AllAreas))
        {
            walkPoint = hit.position; // Set the walk point to a valid position on the NavMesh
            walkPointSet = true;
        }
    }
    private void GoToFire()
    {
        agent.isStopped = false;
        agent.SetDestination(target.position);
    }
    private void PutOutFire()
    {
        agent.isStopped = true;
        agent.ResetPath();
        if (!alreadyAttacked)
        {
            transform.LookAt(target);
            alreadyAttacked = true;
            ThrowWater();
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }
    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    private void Update()
    {
        // set the new target to the closest burning object
        List<Burnable> targets = CheckForBurningObjects(sightRange);
        GameObject closestTarget = getClosest(targets);
        float distanceToTarget = Mathf.Infinity;
        if (closestTarget != null)
        {
            Debug.Log("Found a target");
            target = closestTarget.transform;
            distanceToTarget = Vector3.Distance(transform.position, target.position);
        }

        fireInAttackRange = CheckForBurningObjects(attackRange).Count > 0;
        fireInSightRange = targets.Count > 0;

        if (!fireInSightRange && !fireInAttackRange && distanceToTarget > attackRange) Patroling();
        if (fireInSightRange && !fireInAttackRange && distanceToTarget > attackRange) GoToFire();
        if (fireInSightRange && fireInAttackRange) PutOutFire();
    }
    public void ThrowWater()
    {
        // Create a sphere
        GameObject waterBall = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        // make ball blue
        waterBall.GetComponent<Renderer>().material.color = Color.blue;

        // Add the WaterBall script to handle initialization and collisions
        WaterBall waterBallScript = waterBall.AddComponent<WaterBall>();

        // Initialize the water ball
        Vector3 position = transform.position + transform.up + transform.forward * 1.5f; // Position it in front of the enemy
        Vector3 direction = transform.forward;
        float throwForce = 5f;
        float upwardAngle = 30f;
        direction = Quaternion.Euler(upwardAngle, 0, 0) * direction;
        waterBallScript.Initialize(position, direction, throwForce);

    }
    private void OnDrawGizmosSelected()
{
    // Set the color for the viewfield visualization
    Gizmos.color = Color.yellow;

    // Draw the viewfield as two lines representing the edges of the cone
    float range = sightRange; // Use the sight range for the cone's length

    // Calculate the directions for the edges of the cone
    Vector3 leftBoundary = Quaternion.Euler(0, -viewAngle, 0) * transform.forward * range;
    Vector3 rightBoundary = Quaternion.Euler(0, viewAngle, 0) * transform.forward * range;

    // Draw the lines for the cone
    Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
    Gizmos.DrawLine(transform.position, transform.position + rightBoundary);

    // Optionally, draw a sphere to represent the range
    Gizmos.color = new Color(1, 1, 0, 0.2f); // Semi-transparent yellow
    Gizmos.DrawWireSphere(transform.position, range);
}
}
