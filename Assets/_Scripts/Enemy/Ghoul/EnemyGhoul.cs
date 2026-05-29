using UnityEngine;

public class EnemyGhoul : EnemyMelee
{
    [Header("Armor Hit Settings")]
    public float stunDuration = 2f;

    public GhoulAudio ghoulAudio;

    public override void ArmorHit(bool stun)
    {
        //Bei Treffer auf Panzerung wird Stun() auf EnemyBase ausgelöst
        if (currentState == EnemyState.Dead) return;

        if(!isStunnedFlag && stun == true)
        {
            ghoulAudio.PlayHelmetImpact();
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
        base.TakeDamage(damage);

        ghoulAudio.PlayTakeDamage();
    }

    protected override void GetAudioReference()
    {
        ghoulAudio = GetComponent<GhoulAudio>();
    }
}
