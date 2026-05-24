using UnityEngine;

public class PlayerKnife : Weapon
{
    [Header("References")]
    public Camera playerCam;

    [Header("Knife Stats")]
    public float attackRange = 3f;
    public float attackDamage;
    public int powerUpBonus = 2;

    public void Attack()
    {
        if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out RaycastHit hit, attackRange, Combined))
            ProcessHit(hit, attackDamage);
    }

    protected override void EnemyBodyHit(Vector3 pos, Collider col, float damage)
    {
        PlayerStatsAndUIPanel.Instance.ChangePowerUp(powerUpBonus);
        base.EnemyBodyHit(pos, col, damage);
    }

    protected override void EnemyHeadHit(Vector3 pos, Collider col, float damage)
    {
        PlayerStatsAndUIPanel.Instance.ChangePowerUp(powerUpBonus);
        base.EnemyHeadHit(pos, col, damage);
    }

    public void OnAnimationStart() => PlayerAnimations.Instance.IsLeftArmPlaying = true;
    public void OnAnimationEnd() => PlayerAnimations.Instance.IsLeftArmPlaying = false;
}