using UnityEngine;

public class EnemyAnimationEvents : MonoBehaviour
{
    private EnemyMelee enemy;

    private void Awake()
    {
        enemy = GetComponentInParent<EnemyMelee>();
    }

    public void OnAttackHit()
    {
        enemy.OnAttackHit();
    }

    public void ResetAttack()
    {
        enemy.ResetAttack();
    }
}
