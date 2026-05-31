using UnityEngine;
using System.Collections;

public class EnemyGhoul : EnemyMelee
{
    [Header("Armor Hit Settings")]
    public float stunDuration = 2f;

    [Header("Cry Audio Settings")]
    [SerializeField] private Vector2 idleCryInterval = new Vector2(8f, 15f); 
    [SerializeField] private Vector2 chaseCryInterval = new Vector2(3f, 6f);

    private GhoulAudio ghoulAudio;
    private Coroutine cryRoutine;

    protected override void Start()
    {
        base.Start();
        cryRoutine = StartCoroutine(CryRoutine());
        ghoulAudio = GetComponent<GhoulAudio>();
    }
    public override void ArmorHit(bool stun)
    {
        if (currentState == EnemyState.Dead) return;
        animator.SetBool(Animator.StringToHash("IsAttacking"), false);

        if(!isStunnedFlag)
        {
            ghoulAudio.PlayHelmetImpact();
            ghoulAudio.PlayStunCry();
        }

        if (stun) Stun(stunDuration);
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);

        if(health != 0)
        {
            ghoulAudio.PlayTakeDamage();
        }
    }

    protected override void Die()
    {
        base.Die();
        DissolveEffect dissolve = GetComponent<DissolveEffect>();
        if (dissolve != null)
            dissolve.StartDissolve();

        ghoulAudio.PlayDeath();
    }

    private IEnumerator CryRoutine()
    {
        while (!isdead)
        {
            float waitTime;

            switch (currentState)
            {
                case EnemyState.Chase:
                    waitTime = Random.Range(chaseCryInterval.x, chaseCryInterval.y);
                    break;

                case EnemyState.Idle:
                case EnemyState.Patrol:
                    waitTime = Random.Range(idleCryInterval.x, idleCryInterval.y);
                    break;

                default:
                    yield return null;
                    continue;
            }

            yield return new WaitForSeconds(waitTime);

            if (currentState == EnemyState.Idle ||
                currentState == EnemyState.Patrol ||
                currentState == EnemyState.Chase)
            {
                ghoulAudio.PlayIdleCry();
            }
        }
    }
}