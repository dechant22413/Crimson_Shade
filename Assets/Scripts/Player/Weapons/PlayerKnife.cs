using UnityEngine;

public class PlayerKnife : Weapon
{
    #region Settings
    [Header("References")]
    public Camera playerCam;

    [Header("Knife Stats")]
    public float attackRange = 3f;
    public float attackDamage;
    public int powerUpBonus = 2;
    #endregion

    public void Attack()
    {
        //Wird über Animation Event des Messers aufgerufen
        if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out RaycastHit hit, attackRange, Combined))
            ProcessHit(hit, attackDamage);
    }

    protected override void EnemyBodyHit(Vector3 pos, Collider col, float damage)
    {
        //Enemy wird gedamaged, zusätzlich wird das PowerUp aufgeladen
        PlayerStatsAndUIPanel.Instance.ChangePowerUp(powerUpBonus);
        base.EnemyBodyHit(pos, col, damage);
    }

    protected override void EnemyHeadHit(Vector3 pos, Collider col, float damage)
    {
        //Doppelter Schaden bei Kopftreffer, Aufladen des PowerUps
        PlayerStatsAndUIPanel.Instance.ChangePowerUp(powerUpBonus);
        base.EnemyHeadHit(pos, col, damage);
    }

    //Werden über Animation Events des Messers aufgerufen
    public void OnAnimationStart() => PlayerAnimations.Instance.IsLeftArmPlaying = true;
    public void OnAnimationEnd() => PlayerAnimations.Instance.IsLeftArmPlaying = false;
}