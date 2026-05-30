using UnityEngine;
using UnityEngine.AI;

public class EnemyMelee : EnemyBase
{
    #region Settings
    [Header("Animation")]
    public Animator animator;
    public float walkAnimationSpeed = 1f;

    [Header("TurnIndex and Attack Angle")]
    [SerializeField] private float turnIndex = 5f;
    [SerializeField] private float maxAttackAngle = 60f;
    #endregion

    #region Animator Variables
    private static readonly int speedHash = Animator.StringToHash("Speed");
    private static readonly int isInAttackRangeHash = Animator.StringToHash("IsInAttackRange");
    private static readonly int isAttackingHash = Animator.StringToHash("IsAttacking");
    private static readonly int stunnedHash = Animator.StringToHash("Stunned");
    private static readonly int deathHash = Animator.StringToHash("Death");
    private static readonly int isInactiveHash = Animator.StringToHash("IsInactive");
    #endregion 

    protected override void Update()
    {
        base.Update();

        float speed = agent.velocity.magnitude / chaseSpeed;
        float currentSpeed = animator.GetFloat(speedHash);
        float damp = speed < currentSpeed ? 0.25f : 0f;

        //speedHash des Animators wird gesetzt
        animator.SetFloat(speedHash, speed, damp, Time.deltaTime);

        if (chaseDelayActive)
        {
            animator.speed = 1f;
            return;
        }

        if (animator.IsInTransition(0))
        {
            animator.speed = 1f;
            return;
        }

        if (currentState == EnemyState.Patrol || currentState == EnemyState.Chase)
        {
            float normalizedSpeed = agent.velocity.magnitude / patrolSpeed;

            //Laufgeschwindigkeit des Animators wird je nach State verändert, um Animation Speeds an geschwindigkeit des Gegners anzupassen
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

    protected override void Inactive() => agent.SetDestination(transform.position); //Gegner bewegt sich nicht
    protected override void Idle() => agent.SetDestination(transform.position); //Gegner bewegt sich nicht
    protected override void Patrol() { }
    protected override void Chase() { }
    protected override void Dead() => isdead = true;

    protected override void Stunned()
    {
        agent.SetDestination(transform.position); //Gegner bewegt sich nicht
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
        //Wird durch Animation Event aufgerufen
        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            PlayerStatsAndUIPanel.Instance.DamagePlayer ((int)attackDamage);
        }
    }

    public override void ResetAttack()
    {
        alreadyAttacked = false;

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist > attackRange)
        {
            animator.SetBool(isAttackingHash, false);
            animator.SetBool(isInAttackRangeHash, false);
            firstAttackDone = false;
        }
    }

    protected override void Die()
    {
        //Alle laufenden Preozesse werden gestoppt
        CancelInvoke();
        StopAllCoroutines();
        base.Die();
        //Death Animation Trigger
        animator.SetTrigger(deathHash);
    }

    protected override void GetAudioReference() { }
}