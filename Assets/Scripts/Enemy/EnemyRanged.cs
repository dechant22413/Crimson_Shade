using UnityEngine;

public class EnemyRanged : EnemyBase
{
    [Header("Animation")]
    public Animator animator;

    private static readonly int isAttackingHash = Animator.StringToHash("IsAttacking");
    private static readonly int deathHash = Animator.StringToHash("Death");
    private static readonly int isInactiveHash = Animator.StringToHash("IsInactive");

    protected override void OnStateChanged(EnemyState newState)
    {
        base.OnStateChanged(newState);

        animator.SetBool(isInactiveHash, newState == EnemyState.Inactive);

        switch (newState)
        {
            case EnemyState.Attack:
                if (firstAttackDone)
                    animator.SetBool(isAttackingHash, true);
                break;

            case EnemyState.Chase:
            case EnemyState.Patrol:
            case EnemyState.Idle:
            case EnemyState.Stunned:
                animator.SetBool(isAttackingHash, false);
                break;
        }
    }

    protected override void OnPlayerOutOfAttackRange()
    {
        animator.SetBool(isAttackingHash, false);
    }

    protected override void Inactive() => agent.SetDestination(transform.position);
    protected override void Idle() => agent.SetDestination(transform.position);
    protected override void Patrol() { }
    protected override void Chase() { }
    protected override void Stunned() => agent.SetDestination(transform.position);
    protected override void Dead() { }

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

    public override void ResetAttack() => alreadyAttacked = false;

    protected override void Die()
    {
        CancelInvoke();
        StopAllCoroutines();
        base.Die();
        animator.SetTrigger(deathHash);
    }
}
