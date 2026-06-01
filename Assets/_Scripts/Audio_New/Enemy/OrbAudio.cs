using UnityEngine;

public class OrbAudio : MonoBehaviour
{
    [Header("Sounds")]
    public AudioEventRef takeDamage;
    public AudioEventRef death001;
    public AudioEventRef death002;
    public AudioEventRef hover;
    public AudioEventRef activeTransitition;
    public AudioEventRef inactiveTransition;

    private AudioSource hoverLoopSource;

    public void PlayTakeDamage() => takeDamage.Play(transform);
    public void PlayDeath001() => death001.Play(transform);
    public void PlayDeath002() => death002.Play(transform);
    public void PlayActiveTransition() => activeTransitition.Play(transform);
    public void PlayInactiveTransition() => inactiveTransition.Play(transform);
    public void PlayHoverSound()
    {
        if (hoverLoopSource == null)
            hoverLoopSource = hover.PlayLoopingAttached(transform);
    }

    public void StopHoverSound()
    {
        if (hoverLoopSource != null)
        {
            SoundFXManager.Instance.StopLooping(hoverLoopSource);
            hoverLoopSource = null;
        }
    }
}
