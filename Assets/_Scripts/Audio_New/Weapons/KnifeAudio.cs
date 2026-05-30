using UnityEngine;

public class KnifeAudio : MonoBehaviour
{
    [Header("Sounds")]
    public AudioEventRef slash_001;
    public AudioEventRef slash_002;
    public AudioEventRef slash_003;

    public void PlayKnifeSlash_001() => slash_001.Play(transform);

    public void PlayKnifeSlash_002() => slash_002.Play(transform);

    public void PlayKnifeSlash_003() => slash_003.Play(transform);
}
