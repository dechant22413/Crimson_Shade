using UnityEngine;

public class GhoulAudio : MonoBehaviour
{
    [Header("Sounds")]
    public AudioEventRef takeDamage;
    public AudioEventRef death;
    public AudioEventRef helmetImpact;
    public AudioEventRef stunCry;
    public AudioEventRef attack;
    public AudioEventRef cry;
    public AudioEventRef step;

    public void PlayTakeDamage() => takeDamage.Play(transform);
    public void PlayDeath() => death.Play(transform);
    public void PlayHelmetImpact() => helmetImpact.Play(transform);
    public void PlayStunCry() => stunCry.Play(transform);
    public void PlayAttack() => attack.Play(transform);
    public void PlayIdleCry() => cry.Play(transform);
    public void PlayStep() => step.Play(transform);
}
