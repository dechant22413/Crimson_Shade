using UnityEngine;

public class EnemyAnimationEvents : MonoBehaviour
{
    private EnemyBase enemy;

    //Animation Events, die von einem beliebigen Gegner aufgerufen werden können
    private void Awake()
    {
        enemy = GetComponentInParent<EnemyBase>();
    }

    public void OnAttackHit()
    {
        enemy.OnAttackHit();
    }

    public void ResetAttack()
    {
        enemy.ResetAttack();
    }

    public void SpawnProjectile()
    {
        enemy.SpawnProjectile();
    }
}
