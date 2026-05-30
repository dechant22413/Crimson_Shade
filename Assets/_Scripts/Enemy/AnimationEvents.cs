using UnityEngine;

public class AnimationEvents : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AudioSource audioSource;
    private EnemyBase enemy;

    private GhoulAudio ghoulAudio;

    //Animation Events, die von einem beliebigen Gegner, Gegenstand oder dem Spieler aufgerufen werden können
    #region Enemy Events
    private void Awake()
    {
        enemy = GetComponentInParent<EnemyBase>();

        ghoulAudio = GetComponentInParent<GhoulAudio>();
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

    #region GhoulAudio
    public void GhoulPlayAttackSound()
    {
        if (ghoulAudio == null) return;
        ghoulAudio.PlayAttack();
    }
    #endregion
    #endregion

    #region Player Events
    public void PlaySound(string soundName)
    {
        AudioManager.Instance.PlayAudio(soundName, audioSource);
    }
    #endregion 
}
