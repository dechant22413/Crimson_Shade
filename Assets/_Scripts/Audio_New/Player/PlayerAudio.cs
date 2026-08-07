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

    [Header("Sounds")]
    public AudioEventRef jump;
    public AudioEventRef dash;
    public AudioEventRef heal;
    public AudioEventRef dashRecover;
    public AudioEventRef powerupFull;
    public AudioEventRef takeDamageFlesh;
    public AudioEventRef takeDamagePunch;
    public AudioEventRef footStep;


    public void PlayJump() => jump.Play(transform);

    public void PlayDash() => dash.Play(transform);

    public void PlayHeal() => heal.Play(transform);

    public void PlayDashRecover() => dashRecover.Play(transform);

    public void PlayTakeDamagePunch() => takeDamagePunch.Play(transform);

    public void PlayTakeDamageFlesh() => takeDamageFlesh.Play(transform);

    public void PlayPowerUpFull() => powerupFull.Play(transform);

    public void PlayFootStep() => footStep.Play(transform);
}
