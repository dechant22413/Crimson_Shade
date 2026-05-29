using UnityEngine;

public class GhoulAudio : MonoBehaviour
{
    [Header("AudioSource Reference")]
    [SerializeField] private AudioSource ghoulAudioSource;

    public void PlayTakeDamage() => AudioManager.Instance.PlayAudio(SoundNames.Ghoul.GhoulTakeDamage, ghoulAudioSource);
    public void PlayDeath() => AudioManager.Instance.PlayAudio(SoundNames.Ghoul.GhoulDeath, ghoulAudioSource);
    public void PlayHelmetImpact() => AudioManager.Instance.PlayAudio(SoundNames.Ghoul.HelmetImpact, ghoulAudioSource);
    public void PlayAttack() => AudioManager.Instance.PlayAudio(SoundNames.Ghoul.GhoulAttack, ghoulAudioSource);
}
