using UnityEngine;

public class AnimationEvents : MonoBehaviour
{
    private EnemyBase enemy;

    private GhoulAudio ghoulAudio;
    private KnifeAudio knifeAudio;
    private ShotgunAudio shotgunAudio;
    private ChestAudio chestAudio;

        private void Awake()
    {
        enemy = GetComponentInParent<EnemyBase>();

        ghoulAudio = GetComponentInParent<GhoulAudio>();
        knifeAudio = GetComponentInParent<KnifeAudio>();
        shotgunAudio = GetComponentInParent<ShotgunAudio>();
        chestAudio = GetComponentInParent<ChestAudio>();
    }
    //Animation Events, die von einem beliebigen Gegner, Gegenstand oder dem Spieler aufgerufen werden können
    #region Enemy Events

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

    #region GhoulAudio
    public void GhoulPlayAttackSound()
    {
        if (ghoulAudio == null) return;
        ghoulAudio.PlayAttack();
    }
    #endregion
    #endregion

    #region Player Events

    #endregion

    #region weapons Events
    #region Shotgun
    public void ShotgunPlayReload()
    {
        if (shotgunAudio == null) return;
        shotgunAudio.PlayReload();
    }

    public void ShotgunPlayAttack()
    {
        if (shotgunAudio == null) return;
        shotgunAudio.PlayAttack();
    }
    #endregion

    #region Knife
    public void KnifePlaySlashSound_001()
    {
        if (knifeAudio == null) return;
        knifeAudio.PlayKnifeSlash_001();
    }

    public void KnifePlaySlashSound_002()
    {
        if (knifeAudio == null) return;
        knifeAudio.PlayKnifeSlash_002();
    }

    public void KnifePlaySlashSound_003()
    {
        if (knifeAudio == null) return;
        knifeAudio.PlayKnifeSlash_003();
    }
    #endregion
    #endregion Events

    #region Chest
    public void ChestPlayOpenLid()
    {
        if (chestAudio == null) return;
        chestAudio.OpenLid();
    }

    public void ChestPlayOpenLock()
    {
        if (chestAudio == null) return;
        chestAudio.OpenLock();
    }
    #endregion 
}
