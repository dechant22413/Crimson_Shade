using UnityEngine;

public class EnemyMelee : EnemyBase
{
    [Header("Animation")]
    public Animator animator;
    public float walkAnimationSpeed = 1f;
    public float chaseAnimationSpeed = 1.5f;

    [Header("Turn Settings")]
    [SerializeField] private float turnIndex = 8f;
    [SerializeField] private float maxAttackAngle = 60f;

    private static readonly int speedHash = Animator.StringToHash("Speed");
    private static readonly int speedMultiplierHash = Animator.StringToHash("SpeedMultiplier");
    private static readonly int isAttackingHash = Animator.StringToHash("IsAttacking");
    private static readonly int isInAttackRangeHash = Animator.StringToHash("IsInAttackRange");
    private static readonly int stunnedHash = Animator.StringToHash("Stunned");
    private static readonly int deathHash = Animator.StringToHash("Death");
    private static readonly int isInactiveHash = Animator.StringToHash("IsInactive");

    protected override void Update()
    {
        base.Update();

        float targetSpeed = currentState == EnemyState.Chase ? chaseSpeed : patrolSpeed;
        float speed01 = agent.velocity.magnitude / targetSpeed;
        animator.SetFloat(speedHash, speed01, 0.1f, Time.deltaTime);

        // SpeedMultiplier nur bei Chase ändern
        float multiplier = currentState == EnemyState.Chase ? chaseAnimationSpeed : walkAnimationSpeed;
        animator.SetFloat(speedMultiplierHash, multiplier);
    }

    protected override void OnStateChanged(EnemyState newState)
    {
        base.OnStateChanged(newState);

        animator.SetBool(isInactiveHash, newState == EnemyState.Inactive);
        animator.SetBool(stunnedHash, newState == EnemyState.Stunned);
        animator.SetBool(isInAttackRangeHash, newState == EnemyState.Attack);

        switch (newState)
        {
            case EnemyState.Stunned:
                animator.SetBool(isAttackingHash, false);
                break;
            case EnemyState.Chase:
            case EnemyState.Patrol:
            case EnemyState.Idle:
                animator.SetBool(isAttackingHash, false);
                break;
        }
    }

    protected override void OnFirstAttackReady()
    {
        animator.SetBool(isAttackingHash, true);
    }

    protected override void OnPlayerOutOfAttackRange()
    {
        animator.SetBool(isAttackingHash, false);
        animator.SetBool(isInAttackRangeHash, false);
        firstAttackDone = false;
        alreadyAttacked = false;
    }

    protected override void Inactive() => agent.SetDestination(transform.position);
    protected override void Idle() => agent.SetDestination(transform.position);
    protected override void Patrol() { }
    protected override void Chase() { }
    protected override void Dead() { }

    protected override void Stunned()
    {
        agent.SetDestination(transform.position);
    }

    protected override void Attack()
    {
        chaseDelayActive = false;
        agent.SetDestination(transform.position);

        Vector3 dir = (player.position - transform.position).normalized;
        dir.y = 0;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * turnIndex);

        if (!alreadyAttacked)
        {
            float angle = Vector3.Angle(transform.forward, dir);
            if (angle > maxAttackAngle) return;

            animator.SetBool(isAttackingHash, true);
            alreadyAttacked = true;
        }
    }

    public override void OnAttackHit()
    {
        if (Vector3.Distance(transform.position, player.position) <= attackRange)
            PlayerStatsAndUIPanel.Instance.ChangeLifePoints(-(int)attackDamage);
    }

    public override void ResetAttack() => alreadyAttacked = false;

    protected override void Die()
    {
        CancelInvoke();
        StopAllCoroutines();
        base.Die();
        animator.SetTrigger(deathHash);
    }

    protected override void GetAudioReference() { }
}