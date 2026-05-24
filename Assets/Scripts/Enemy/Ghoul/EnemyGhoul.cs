using UnityEngine;

public class EnemyGhoul : EnemyMelee
{
    [Header("Armor Hit Settings")]
    public float stunDuration = 2f;

    public override void ArmorHit()
    {
        //Bei Treffer auf Panzerung wird Stun() auf EnemyBase ausgelöst
        if (currentState == EnemyState.Dead) return;
        animator.SetBool(Animator.StringToHash("IsAttacking"), false);
        Stun(stunDuration);
    }

    protected override void Die()
    {
        base.Die();
        DissolveEffect dissolve = GetComponent<DissolveEffect>();
        if (dissolve != null)
            dissolve.StartDissolve();
    }
}
