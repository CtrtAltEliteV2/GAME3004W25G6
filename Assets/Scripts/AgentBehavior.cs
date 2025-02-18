using UnityEngine;
using UnityEngine.AI;

public class AgentBehavior : MonoBehaviour
{
    [Header("Agent Settings")]
    [SerializeField] private float chaseRange = 10f;
    [SerializeField] private float stopDistance = 1.5f;
    [SerializeField] private float roamRadius = 15f;
    [SerializeField] private float roamWaitTime = 3f;

    private NavMeshAgent agent;
    private Transform player;
    private bool isChasing = false;
    private bool isRoaming = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Find player by tag
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("Player not found! Make sure your player has the 'Player' tag.");
        }

        StartRoaming();
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= chaseRange)
        {
            isChasing = true;
            isRoaming = false;
        }
        else if (distanceToPlayer > chaseRange + 2f) // Buffer to prevent toggling
        {
            isChasing = false;
            if (!isRoaming)
            {
                StartRoaming();
            }
        }

        if (isChasing)
        {
            ChasePlayer(distanceToPlayer);
        }
    }

    void ChasePlayer(float distance)
    {
        if (distance > stopDistance)
        {
            agent.SetDestination(player.position);
        }
        else
        {
            agent.ResetPath();
        }
    }

    void StartRoaming()
    {
        isRoaming = true;
        InvokeRepeating(nameof(RoamToRandomPoint), 0f, roamWaitTime);
    }

    void RoamToRandomPoint()
    {
        if (isChasing) return; // Stop roaming if chasing

        Vector3 randomDirection = Random.insideUnitSphere * roamRadius;
        randomDirection += transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, roamRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }
}
