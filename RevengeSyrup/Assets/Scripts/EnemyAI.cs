using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAiTutorial : MonoBehaviour
{
    [SerializeField] private GameObject projectile;
    [SerializeField] private Transform spawnPoint;

    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float sightRange = 5f;
    [SerializeField] private float walkPointRange = 5f;

    private NavMeshAgent agent;
    private Transform player;

    private Vector3 walkPoint;
    private bool walkPointSet;
    private bool alreadyAttacked;

    private int health = 5;
    private LayerMask whatIsGround;

    private Renderer enemyRenderer;
    private Color originalColor;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Origin").transform;
        whatIsGround = LayerMask.GetMask("Ground");

        // Get the Renderer and store the original color
        enemyRenderer = GetComponent<Renderer>();
        if (enemyRenderer != null)
        {
            originalColor = enemyRenderer.material.color;
        }
    }

    void Update()
    {
        //Check if the enemy is close enough to the player to start chasing
        if (Vector3.Distance(transform.position, player.position) < sightRange && walkPointSet == false)
        {
            ChasePlayer();
        }
        //Check if the enemy is not close enough to the player and should wander
        else if (walkPointSet == false)
        {
            if (Vector3.Distance(transform.position, walkPoint) < 1f)
                SearchWalkPoint();
            else
                WalkToWalkPoint();
        }

        //Check if the enemy is close enough to the player to attack
        if (Vector3.Distance(transform.position, player.position) < attackRange && !alreadyAttacked)
        {
            alreadyAttacked = true;
            FireBullet();
            Invoke(nameof(ResetAttack), 1f);
        }
    }

    void WalkToWalkPoint()
    {
        agent.SetDestination(walkPoint);

        //Calculate the distance to the walk point
        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    void SearchWalkPoint()
    {
        //Calculate random point in range
        float randomZ = UnityEngine.Random.Range(-walkPointRange, walkPointRange);
        float randomX = UnityEngine.Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    void FireBullet()
    {
        //Create a new instance of the bullet
        GameObject spawnedBullet = Instantiate(projectile);
        //Get the Rigidbody component from the spawned bullet
        Rigidbody rb = spawnedBullet.GetComponent<Rigidbody>();
        //Set the position of the bullet
        rb.position = spawnPoint.position;
        //Add a force to the bullet in the forward direction
        rb.AddForce(transform.forward * 32f, ForceMode.Impulse);

        // Add an upward force to the bullet if player is above the AI
        if (player.position.y > transform.position.y)
            rb.AddForce(transform.up * 8f, ForceMode.Impulse);
        Destroy(spawnedBullet, 5);
    }

    void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        ChangeColor(Color.yellow); // Flash the enemy when hit

        if (health <= 0) DestroyEnemy();
    }

    void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    public void ChangeColor(Color color)
    {
        if (enemyRenderer != null)
        {
            enemyRenderer.material.color = color;
            Invoke(nameof(ResetColor), 0.2f); // Revert color after 0.2 seconds
        }
    }

    void ResetColor()
    {
        if (enemyRenderer != null)
        {
            enemyRenderer.material.color = originalColor;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
