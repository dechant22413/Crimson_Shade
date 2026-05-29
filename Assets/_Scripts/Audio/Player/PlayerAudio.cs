using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    #region Singleton Initialization
    //Singleton
    public static PlayerAudio Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }
    #endregion 

    [Header("AudioSource Reference")]
    [SerializeField] private AudioSource playerAudioSource;

    public void PlayJump() => AudioManager.Instance.PlayAudio(SoundNames.Player.Jump, playerAudioSource);

    public void PlayDash() => AudioManager.Instance.PlayAudio(SoundNames.Player.Dash, playerAudioSource);

    public void PlayHeal() => AudioManager.Instance.PlayAudio(SoundNames.Player.Heal, playerAudioSource);

    public void PlayDashRecover() => AudioManager.Instance.PlayAudio(SoundNames.Player.DashRecover, playerAudioSource);

    public void PlayTakeDamagePunch() => AudioManager.Instance.PlayAudio(SoundNames.Player.TakeDamagePunch, playerAudioSource);

    public void PlayTakeDamageFlesh() => AudioManager.Instance.PlayAudio(SoundNames.MaterialImpacts.FleshHit, playerAudioSource);

    public void PlayPowerUpFull() => AudioManager.Instance.PlayAudio(SoundNames.Player.PowerUpFull, playerAudioSource);
}
