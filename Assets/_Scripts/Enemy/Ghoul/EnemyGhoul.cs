using UnityEngine;
using System.Collections;

public class EnemyGhoul : EnemyMelee
{
    [Header("Armor Hit Settings")]
    public float stunDuration = 2f;

    [Header("Cry Audio Settings")]
    [SerializeField] private Vector2 idleCryInterval = new Vector2(8f, 15f);
    [SerializeField] private Vector2 chaseCryInterval = new Vector2(3f, 6f);

    public GhoulAudio ghoulAudio;

    private Coroutine cryRoutine;

    protected override void Start()
    {
        base.Start();

        cryRoutine = StartCoroutine(CryRoutine());
    }

    public override void ArmorHit(bool stun)
    {
        //Bei Treffer auf Panzerung wird Stun() auf EnemyBase ausgelöst
        if (currentState == EnemyState.Dead) return;

        if(!isStunnedFlag && stun == true)
        {
            ghoulAudio.PlayHelmetImpact();
            ghoulAudio.PlayDeath();
        }

        if (stun == true)
        {
            Stun(stunDuration);
            animator.SetBool(Animator.StringToHash("IsAttacking"), false);
        }
    }

    protected override void Die()
    {
        base.Die();
        DissolveEffect dissolve = GetComponent<DissolveEffect>();
        if (dissolve != null)
            dissolve.StartDissolve();
    }

    public override void TakeDamage(float damage)
    {
        if (health == 0) return;
        base.TakeDamage(damage);

        if (health == 0)
        {
            ghoulAudio.PlayDeath();
            return;
        }
        ghoulAudio.PlayTakeDamage();
    }

    protected override void GetAudioReference()
    {
        ghoulAudio = GetComponent<GhoulAudio>();
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

            if (currentState == EnemyState.Idle || currentState == EnemyState.Patrol || currentState == EnemyState.Chase)
            {
                ghoulAudio.PlayIdleCry();
            }
        }
    }
}
