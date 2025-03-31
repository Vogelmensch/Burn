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
    public bool fireInSightRange ,fireInAttackRange;

    [Header("Animation")]
    public Animator animator;
    private bool isWalking = false;
    private bool isRunning = false;
    private bool isAttacking = false;
    private float speed = 1f;

    [SerializeField] private int BurningLayer = 6;
    [SerializeField] private int ignoreLayer = 2;
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
                    // Perform a raycast to ensure there are no obstacles
                    if (Physics.Raycast(transform.position, directionToBurnable, out RaycastHit hit, range, ~(1 << IgnoreLayer)))
                    {
                        Debug.DrawRay(transform.position, directionToBurnable * range, Color.red);
                        // Check if the hit object is the burnable
                        //if (hit.collider.GetComponent<Burnable>() == burnable)
                        // keine Ahnung warum das nicht funktioniert hat mit der Zeile oben drüber aber mit der hier unten scheint es zu funktionieren also bleibt uns nur beten das es auch wirklich funktioniert
                        if (hit.collider.gameObject.layer == BurningLayer)
                        {
                            burnables.Add(burnable);
                        }
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
        // Handle animation
        isWalking = true;
        isRunning = false;
        isAttacking = false;
        speed = 1f;
        agent.speed = speed;
        animator.SetFloat("speed", speed);
        animator.SetBool("puttingOutFire", false);

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
        // Handle animation
        isWalking = false;
        isRunning = true;
        isAttacking = false;
        speed = 3f;
        agent.speed = speed;
        animator.SetFloat("speed", speed);
        animator.SetBool("puttingOutFire", false);

        agent.isStopped = false;
        agent.SetDestination(target.position);
    }
    private void PutOutFire()
    {
        // Handle animation
        isWalking = false;
        isRunning = false;
        isAttacking = true;
        animator.SetBool("puttingOutFire", isAttacking);

        agent.isStopped = true;
        agent.ResetPath();
        if (!alreadyAttacked)
        {
            transform.LookAt(target);
            alreadyAttacked = true;
            ThrowWater();
            target.GetComponent<Burnable>().WaterHit(50f);
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }
    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    private void Update()
    {
        if (!alreadyAttacked){
        // we need to set these to false because oherwise they just stay true once they are set to true
        fireInAttackRange = false;
        fireInSightRange = false;
        // set the new target to the closest burning object
        List<Burnable> targets = CheckForBurningObjects(sightRange);
        Debug.Log("Targets: " + targets.Count);
        GameObject closestTarget = getClosest(targets);
        float distanceToTarget = Mathf.Infinity;
        if (closestTarget != null)
        {
            Debug.Log("Found a target");
            target = closestTarget.transform;
            distanceToTarget = Vector3.Distance(transform.position, target.position);
            fireInSightRange = true;
            fireInAttackRange = distanceToTarget < attackRange;
        }
        // Fire not visible and not in attack range -> patrol
        if (!fireInSightRange && !fireInAttackRange && distanceToTarget > attackRange) Patroling();

        // Fire visible but not in attack range -> go to fire
        if (fireInSightRange && !fireInAttackRange && distanceToTarget > attackRange) GoToFire();

        // Fire visible and in attack range -> put out fire
        if (fireInSightRange && fireInAttackRange) PutOutFire();
        }
    }
    public void ThrowWater()
    {
        if (target == null) return;

        // Create a sphere
        GameObject waterBall = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        // Make the ball blue
        waterBall.GetComponent<Renderer>().material.color = Color.blue;

        // Add the WaterBall script to handle initialization and collisions
        WaterBall waterBallScript = waterBall.AddComponent<WaterBall>();

        // Calculate the position and direction
        Vector3 startPosition = transform.position + transform.up + transform.forward * 1.5f; // Position it in front of the enemy
        Vector3 targetPosition = target.position;

        // Add height offset based on the target's collider
        Collider targetCollider = target.GetComponent<Collider>();
        if (targetCollider != null)
        {
            targetPosition.y += targetCollider.bounds.extents.y; // Add half the height of the collider
        }

        // Calculate the direction and force needed to hit the target
        Vector3 direction = targetPosition - startPosition;
        float distance = direction.magnitude;
        direction.Normalize();

        // Physics calculations for projectile motion
        float gravity = Physics.gravity.y; // Gravity value
        float heightDifference = targetPosition.y - startPosition.y;

        // Calculate the required throw force
        float angle = 45f; // Optimal angle for projectile motion
        float throwForce = Mathf.Sqrt((distance * Mathf.Abs(gravity)) / Mathf.Sin(2 * Mathf.Deg2Rad * angle));
        throwForce = throwForce * 1.5f; // Adjust the force to make it more powerful
        // Adjust the direction to include the vertical component
        float angleToTarget = Mathf.Atan2(heightDifference, distance) * Mathf.Rad2Deg;
        direction = Quaternion.Euler(-angleToTarget, 0, 0) * direction;

        // Initialize the water ball
        waterBallScript.Initialize(startPosition, direction, throwForce);
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
