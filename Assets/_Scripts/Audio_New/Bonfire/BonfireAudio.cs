using UnityEngine;
using UnityEngine.AdaptivePerformance;

public class BonfireAudio : MonoBehaviour
{
    [Header("Sounds")]
    public AudioEventRef activate;
    public AudioEventRef loop;

    private AudioSource bonfireAudioSource;

    public void PlayActivate() => activate.Play(transform);

    public void PlayLoopSound()
    {
        if (bonfireAudioSource == null)
            bonfireAudioSource = loop.PlayLooping(transform);
    }

    public void StopLoopSound()
    {
        if (bonfireAudioSource != null)
        {
            SoundFXManager.Instance.StopLooping(bonfireAudioSource);
            bonfireAudioSource = null;
        }
    }

}
