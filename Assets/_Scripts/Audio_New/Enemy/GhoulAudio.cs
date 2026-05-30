using UnityEngine;

public class GhoulAudio : MonoBehaviour
{
    [Header("Sounds")]
    public AudioEventRef takeDamage;
    public AudioEventRef death;
    public AudioEventRef helmetImpact;
    public AudioEventRef attack;
    public AudioEventRef idleCry;
    public AudioEventRef chaseCry;

    public void PlayTakeDamage() => takeDamage.Play(transform);
    public void PlayDeath() => death.Play(transform);
    public void PlayHelmetImpact() => helmetImpact.Play(transform);
    public void PlayAttack() => attack.Play(transform);
    public void PlayIdleCry() => idleCry.Play(transform);
    public void PlayChaseCry() => chaseCry.Play(transform);

}
