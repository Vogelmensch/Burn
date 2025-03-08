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
                Debug.DrawRay(transform.position, directionToBurnable * range, Color.red);
                //if (Physics.Raycast(transform.position, directionToBurnable, out RaycastHit hit, range))
                {
                    //if (hit.collider.GetComponent<Burnable>() == burnable)
                    {
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
        Debug.Log("Patroling");
        if (!walkPointSet) SearchWalkPoint();
        if (walkPointSet)
            agent.SetDestination(walkPoint);
        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }
    private void SearchWalkPoint()
    {
        // Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        // Create the walk point using the random offsets
        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        // Check if the walk point is on the ground
        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
        {
            // If the point is on the ground, set walkPointSet to true
            walkPointSet = true;
        }
    }
    private void GoToFire()
    {
        Debug.Log("Going to fire");
        agent.SetDestination(target.position);
    }
    private void PutOutFire()
    {
        Debug.Log("Putting out fire");
        agent.SetDestination(transform.position);
        transform.LookAt(target);
        if (!alreadyAttacked)
        {
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
        if (closestTarget != null)
        {
            Debug.Log("Found a target");
            target = closestTarget.transform;
        }

        fireInAttackRange = CheckForBurningObjects(attackRange).Count > 0;
        fireInSightRange = targets.Count > 0;

        if (!fireInSightRange && !fireInAttackRange) Patroling();
        if (fireInSightRange && !fireInAttackRange) GoToFire();
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
        float throwForce = 7f;
        float upwardAngle = 30f;
        direction = Quaternion.Euler(upwardAngle, 0, 0) * direction;
        waterBallScript.Initialize(position, direction, throwForce);

    }
}
