using UnityEngine;
using UnityEngine.AI;

public class EnemyMelee : EnemyBase
{
    [Header("Animation")]
    public Animator animator;
    public float walkAnimationSpeed = 1f;

    [Header("Armor Hit Settings")]
    public float stunDuration = 2f;

    private static readonly int speedHash = Animator.StringToHash("Speed");
    private static readonly int isInAttackRangeHash = Animator.StringToHash("IsInAttackRange");
    private static readonly int isAttackingHash = Animator.StringToHash("IsAttacking");
    private static readonly int stunnedHash = Animator.StringToHash("Stunned");
    private static readonly int deathHash = Animator.StringToHash("Death");
    private static readonly int isInactiveHash = Animator.StringToHash("IsInactive");

    protected override void Update()
    {
        base.Update();

        float speed = agent.velocity.magnitude / chaseSpeed;
        float currentSpeed = animator.GetFloat(speedHash);
        float damp = speed < currentSpeed ? 0.25f : 0f;
        animator.SetFloat(speedHash, speed, damp, Time.deltaTime);

        if (chaseDelayActive)
        {
            animator.speed = 1f;
            return;
        }

        if (currentState == EnemyState.Patrol || currentState == EnemyState.Chase)
        {
            float normalizedSpeed = agent.velocity.magnitude / patrolSpeed;
            animator.speed = Mathf.Clamp(normalizedSpeed, 0.05f, chaseSpeed / patrolSpeed) * walkAnimationSpeed;
        }
        else
        {
            animator.speed = 1f;
        }
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


    protected override void OnPlayerOutOfAttackRange()
    {
        animator.SetBool(isAttackingHash, false);
        animator.SetBool(isInAttackRangeHash, false);

        CancelInvoke(nameof(EnableFirstAttack));

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
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));

        if (!alreadyAttacked)
        {
            animator.SetBool(isAttackingHash, true);
            alreadyAttacked = true;
        }
    }

    public override void OnAttackHit()
    {
        if (Vector3.Distance(transform.position, player.position) <= attackRange)
            PlayerStats.Instance.ChangeLifePoints(-(int)attackDamage);
    }

    public override void ResetAttack() => alreadyAttacked = false;

    public override void ArmorHit()
    {
        if (currentState == EnemyState.Dead) return;
        animator.SetBool(isAttackingHash, false);
        Stun(stunDuration);
    }

    protected override void Die()
    {
        CancelInvoke();
        StopAllCoroutines();
        base.Die();
        animator.SetTrigger(deathHash);

        DissolveEffect dissolve = GetComponent<DissolveEffect>();
        dissolve.StartDissolve();
    }
}