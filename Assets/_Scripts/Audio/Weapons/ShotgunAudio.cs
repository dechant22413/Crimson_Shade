using UnityEngine;

public class ShotgunAudio : MonoBehaviour
{
    [Header("AudioSource Reference")]
    [SerializeField] private AudioSource shotgunAudioSource;

    public void PlayReload() => AudioManager.Instance.PlayAudio(SoundNames.Shotgun.ShotgunReload, shotgunAudioSource);

    public void PlayAttack() => AudioManager.Instance.PlayAudio(SoundNames.Shotgun.ShotgunShot, shotgunAudioSource);

    public void PlayEmpty() => AudioManager.Instance.PlayAudio(SoundNames.Shotgun.ShotgunTriggerpull, shotgunAudioSource);
}
