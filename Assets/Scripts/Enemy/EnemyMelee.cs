using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMelee : EnemyBase
{
    [Header("Animation")]
    public Animator animator;
    public string attackClipName = "Ghoul_Standing_Attack_001";

    [Header("Patrol Settings")]
    public float walkPointRange = 10f;
    public float waitAtPointDuration = 1.5f;
    public float chaseDelay = 0.5f;
    public LayerMask groundLayer;

    [Header("Melee Settings")]
    public float attackDamage = 20f;
    public float firstAttackDelay = 0.8f;

    [Header("Armor Hit Settings")]
    public float stunDuration = 2f;

    private static readonly int speedHash = Animator.StringToHash("Speed");
    private static readonly int attackHash = Animator.StringToHash("Attack");
    private static readonly int isAttackingHash = Animator.StringToHash("IsAttacking");
    private static readonly int stunnedHash = Animator.StringToHash("Stunned");
    private static readonly int deathHash = Animator.StringToHash("Death");

    private Vector3 walkPointA;
    private Vector3 walkPointB;
    private Vector3 currentWalkPoint;
    private bool walkPointsSet;
    private bool goingToB;
    private bool alreadyAttacked;
    private bool firstAttackDone;
    private float timeBetweenAttacks;
    private float waitTimer;
    private bool isWaiting;
    private bool chaseDelayActive;

    protected override void Start()
    {
        base.Start();

        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == attackClipName)
            {
                timeBetweenAttacks = clip.length;
                break;
            }
        }
    }

    protected override void Update()
    {
        base.Update();
        float speed = agent.velocity.magnitude / chaseSpeed;
        float currentSpeed = animator.GetFloat(speedHash);
        float damp = speed < currentSpeed ? 0.25f : 0f;
        animator.SetFloat(speedHash, speed, damp, Time.deltaTime);
    }

    protected override void UpdateState()
    {
        if (alreadyAttacked) return;
        base.UpdateState();
    }

    protected override void OnStateChanged(EnemyState newState)
    {
        base.OnStateChanged(newState);

        if (newState == EnemyState.Stunned)
        {
            animator.SetBool(isAttackingHash, false);
            animator.ResetTrigger(attackHash);
            animator.SetBool(stunnedHash, true);
            CancelInvoke(nameof(ResetAttack));
            CancelInvoke(nameof(StartFirstAttack));
            alreadyAttacked = true;
        }
        else
        {
            animator.SetBool(stunnedHash, false);
        }

        if (newState == EnemyState.Attack && !firstAttackDone)
        {
            alreadyAttacked = true;
            firstAttackDone = true;
            Invoke(nameof(StartFirstAttack), firstAttackDelay);
        }

        if (newState == EnemyState.Attack && firstAttackDone)
        {
            alreadyAttacked = false;
            animator.SetBool(isAttackingHash, true);
            animator.ResetTrigger(attackHash);
            animator.SetTrigger(attackHash);
        }

        if (newState == EnemyState.Chase)
        {
            agent.SetDestination(transform.position);
            StartCoroutine(ChaseDelay());
        }

        if (newState == EnemyState.Chase || newState == EnemyState.Patrol)
        {
            firstAttackDone = false;
            animator.SetBool(isAttackingHash, false);
            animator.ResetTrigger(attackHash);
        }
    }

    protected override void Inactive() => agent.SetDestination(transform.position);
    protected override void Idle() => agent.SetDestination(transform.position);
    protected override void Dead() { }

    protected override void Stunned()
    {
        agent.SetDestination(transform.position);
    }

    protected override void Patrol()
    {
        if (!walkPointsSet) SearchWalkPoints();
        if (!walkPointsSet) return;

        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f) isWaiting = false;
            return;
        }

        if (CanUpdateNav())
        {
            currentWalkPoint = goingToB ? walkPointB : walkPointA;
            agent.SetDestination(currentWalkPoint);

            if ((transform.position - currentWalkPoint).magnitude < 1f)
            {
                goingToB = !goingToB;
                isWaiting = true;
                waitTimer = waitAtPointDuration;
                if (!goingToB) walkPointsSet = false;
            }
        }
    }

    protected override void Chase()
    {
        if (chaseDelayActive) return;
        if (CanUpdateNav()) agent.SetDestination(player.position);
    }

    protected override void Attack()
    {
        chaseDelayActive = false;
        agent.SetDestination(transform.position);
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));

        if (!alreadyAttacked)
        {
            animator.SetBool(isAttackingHash, true);
            animator.ResetTrigger(attackHash);
            animator.SetTrigger(attackHash);
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    public void OnAttackHit()
    {
        if (Vector3.Distance(transform.position, player.position) <= attackRange)
            PlayerStats.Instance.ChangeLifePoints(-(int)attackDamage);
    }

    public override void ArmorHit()
    {
        if (currentState == EnemyState.Dead) return;
        animator.SetBool(isAttackingHash, false);
        animator.ResetTrigger(attackHash);
        Stun(stunDuration);
    }

    protected override void Die()
    {
        base.Die();
        animator.SetTrigger(deathHash);
    }

    private void ResetAttack() => alreadyAttacked = false;
    private void StartFirstAttack() => alreadyAttacked = false;

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
        else
        {
            Debug.LogWarning("Keine gültigen Walkpoints gefunden – groundLayer korrekt gesetzt?");
        }
    }

    private Vector3 GetValidWalkPoint()
    {
        for (int i = 0; i < 10; i++)
        {
            float randomX = Random.Range(-walkPointRange, walkPointRange);
            float randomZ = Random.Range(-walkPointRange, walkPointRange);
            Vector3 candidate = new Vector3(transform.position.x + randomX, transform.position.y + 10f, transform.position.z + randomZ);

            if (Physics.Raycast(candidate, Vector3.down, out RaycastHit rayHit, 20f, groundLayer))
                if (NavMesh.SamplePosition(rayHit.point, out NavMeshHit navHit, 2f, NavMesh.AllAreas))
                    return navHit.position;
        }
        return Vector3.zero;
    }

    private IEnumerator ChaseDelay()
    {
        chaseDelayActive = true;
        float timer = chaseDelay;
        while (timer > 0f)
        {
            if (currentState != EnemyState.Chase)
            {
                chaseDelayActive = false;
                yield break;
            }
            Vector3 dir = (player.position - transform.position).normalized;
            dir.y = 0;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 5f);
            timer -= Time.deltaTime;
            yield return null;
        }
        chaseDelayActive = false;
    }
}