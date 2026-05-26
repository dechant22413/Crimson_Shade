using UnityEngine;

public class AnimationEvents : MonoBehaviour
{
    private EnemyBase enemy;

    //Animation Events, die von einem beliebigen Gegner, Gegenstand oder dem Spieler aufgerufen werden können
    #region Enemy Events
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
    #endregion

    #region Player Events
    public void PlaySound(string soundName)
    {
        AudioManager.Instance.PlayAudio(soundName);
    }
    #endregion 
}
