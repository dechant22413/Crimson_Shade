using System.Runtime.CompilerServices;
using UnityEngine;

public class EnemyRanged : EnemyBase
{
    [Header("Animation")]
    public Animator animator;

    [Header("Projectile")]
    public GameObject projectilePrefab;
    public GameObject projectileSpawn;

    [Header("Inactive Settings")]
    public float inactiveDelay = 5f;
    private bool canGoInactive;

    private static readonly int isAttackingHash = Animator.StringToHash("IsAttacking");
    private static readonly int deathHash = Animator.StringToHash("Death");
    private static readonly int isInactiveHash = Animator.StringToHash("IsInactive");

    private OrbAudio orbAudio;
    private AudioSource hoverAudioSource;

    protected override void Start()
    {
        base.Start();

        if(currentState != EnemyState.Inactive)
        {
            orbAudio.PlayHoverSound();
        }
    }

    protected override void OnStateChanged(EnemyState newState)
    {
        base.OnStateChanged(newState);

        if (newState == EnemyState.Inactive)
        {
            orbAudio.StopHoverSound();
            orbAudio.PlayInactiveTransition();
        }

        else
        {
            orbAudio.PlayHoverSound();
            orbAudio.PlayActiveTransition();
        }

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

        if (newState == EnemyState.Patrol)
        {
            canGoInactive = true;
            Invoke(nameof(GoInactive), inactiveDelay);
        }
        else
        {
            canGoInactive = false;
            CancelInvoke(nameof(GoInactive));
        }
    }

    private void GoInactive()
    {
        if (canGoInactive)
            SetState(EnemyState.Inactive);
    }

    protected override void OnPlayerOutOfAttackRange()
    {
        animator.SetBool(isAttackingHash, false);

        CancelInvoke(nameof(EnableFirstAttack));

        firstAttackDone = false;
        alreadyAttacked = false;
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

    public override void SpawnProjectile()
    {
        GameObject proj = Instantiate(projectilePrefab, projectileSpawn.transform.position, projectileSpawn.transform.rotation);
        proj.GetComponent<Homing_Projectile>().Launch();
    }

    public override void ResetAttack() => alreadyAttacked = false;

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);

        if (health != 0)
        {
            orbAudio.PlayTakeDamage();
        }
    }

    protected override void Die()
    {
        CancelInvoke();
        StopAllCoroutines();
        base.Die();
        animator.SetTrigger(deathHash);

        DissolveEffect dissolve = GetComponent<DissolveEffect>();
        dissolve.StartDissolve();

        orbAudio.PlayDeath001();
        orbAudio.PlayDeath002();

        orbAudio.StopHoverSound();
    }

    protected override void GetAudioReference() 
    {
        orbAudio = GetComponent<OrbAudio>();
    }
}
