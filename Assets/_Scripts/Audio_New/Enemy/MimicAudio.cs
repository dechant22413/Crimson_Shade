using UnityEngine;

public class MimicAudio : MonoBehaviour
{
    [Header("Sounds")]
    public AudioEventRef takeDamage;
    public AudioEventRef death;
    public AudioEventRef attack;
    public AudioEventRef step;
    public AudioEventRef mimicOpen001;
    public AudioEventRef mimicOpen002;

    public void PlayTakeDamage() => takeDamage.Play(transform);
    public void PlayDeath() => death.Play(transform);
    public void PlayAttack() => attack.Play(transform);
    public void PlayStep() => step.Play(transform);

    public void PlayMimicOpen()
    {
        mimicOpen001.Play(transform);
        mimicOpen002.Play(transform);
    }
}
