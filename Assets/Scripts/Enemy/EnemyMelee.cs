using UnityEngine;
using UnityEngine.AI;

public class EnemyMelee : EnemyBase
{
    [Header("Patrol Settings")]
    public float walkPointRange = 10f;
    public LayerMask groundLayer;

    [Header("Melee Settings")]
    public float attackDamage = 20f;
    public float timeBetweenAttacks = 1f;
    public float firstAttackDelay = 0.8f;

    private Vector3 walkPointA;
    private Vector3 walkPointB;
    private Vector3 currentWalkPoint;
    private bool walkPointsSet;
    private bool goingToB;
    private bool alreadyAttacked;
    private bool firstAttackDone;

    protected override void OnStateChanged(EnemyState newState)
    {
        if (newState == EnemyState.Attack && !firstAttackDone)
        {
            alreadyAttacked = true;
            firstAttackDone = true;
            Invoke(nameof(ResetAttack), firstAttackDelay);
        }

        if (newState == EnemyState.Chase || newState == EnemyState.Patrol)
            firstAttackDone = false; // zurücksetzen wenn Gegner den Spieler verliert
    }

    protected override void Inactive()
    {
        agent.SetDestination(transform.position);
    }

    protected override void Idle()
    {
        agent.SetDestination(transform.position);
        // Idle Animation hier später
    }

    protected override void Patrol()
    {
        if (!walkPointsSet)
            SearchWalkPoints();

        if (walkPointsSet && CanUpdateNav())
        {
            currentWalkPoint = goingToB ? walkPointB : walkPointA;
            agent.SetDestination(currentWalkPoint);

            if ((transform.position - currentWalkPoint).magnitude < 1f)
                goingToB = !goingToB;
        }
    }

    protected override void Chase()
    {
        if (CanUpdateNav())
            agent.SetDestination(player.position);
    }

    protected override void Attack()
    {
        agent.SetDestination(transform.position);
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));

        if (!alreadyAttacked)
        {
            // Melee Schaden
            PlayerStats.Instance.ChangeLifePoints(-(int)attackDamage);

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    private void SearchWalkPoints()
    {
        Vector3 pointA = GetValidWalkPoint();
        Vector3 pointB = GetValidWalkPoint();

        if (pointA != Vector3.zero && pointB != Vector3.zero)
        {
            walkPointA = pointA;
            walkPointB = pointB;
            walkPointsSet = true;
        }
    }

    private Vector3 GetValidWalkPoint()
    {
        // Mehrere Versuche damit der Punkt sinnvoll ist
        for (int i = 0; i < 10; i++)
        {
            float randomZ = Random.Range(-walkPointRange, walkPointRange);
            float randomX = Random.Range(-walkPointRange, walkPointRange);
            Vector3 candidate = new Vector3(transform.position.x + randomX, transform.position.y + 2f, transform.position.z + randomZ);

            if (Physics.Raycast(candidate, Vector3.down, 4f, groundLayer))
            {
                // Auch NavMesh prüfen damit der Punkt erreichbar ist
                if (UnityEngine.AI.NavMesh.SamplePosition(candidate, out NavMeshHit hit, 2f, NavMesh.AllAreas))
                    return hit.position;
            }
        }

        return Vector3.zero;
    }
}
