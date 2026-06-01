using UnityEngine;
using UnityEngine.AdaptivePerformance;

public class HomingProjectileAudio : MonoBehaviour
{
    [Header("Sounds")]
    public AudioEventRef charging;
    public AudioEventRef locomotion;
    public AudioEventRef hit;

    private AudioSource locmotionLoopSource;

    public void PLayHit() => hit.Play(transform);
    public void PlayCharging() => charging.Play(transform);
    public void PlayLocmotionSound()
    {
        if (locmotionLoopSource == null)
            locmotionLoopSource = locomotion.PlayLoopingAttached(transform);
    }

    public void StopHoverSound()
    {
        if (locmotionLoopSource != null)
        {
            SoundFXManager.Instance.StopLooping(locmotionLoopSource);
            locmotionLoopSource = null;
        }
    }
}
