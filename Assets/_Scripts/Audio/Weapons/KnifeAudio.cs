using UnityEngine;

public class KnifeAudio : MonoBehaviour
{
    [Header("AudioSource Reference")]
    [SerializeField] private AudioSource knifeAudioSource;

    public void PlayKnifeSlash_001() => AudioManager.Instance.PlayAudio(SoundNames.Knife.KnifeSlash_001, knifeAudioSource);

    public void PlayKnifeSlash_002() => AudioManager.Instance.PlayAudio(SoundNames.Knife.KnifeSlash_002, knifeAudioSource);

    public void PlayKnifeSlash_003() => AudioManager.Instance.PlayAudio(SoundNames.Knife.KnifeSlash_003, knifeAudioSource);
}
