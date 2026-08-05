using UnityEngine;
using UnityEngine.AdaptivePerformance;

public class BackgroundMusic : MonoBehaviour
{
    [Header("Sounds")]
    public AudioEventRef track001;

    private AudioSource musicLoopSource;

    private void Start()
    {
        PlayTrack001();
    }

    public void PlayTrack001()
    {
        if (musicLoopSource == null)
            musicLoopSource = track001.PlayLooping(transform);
    }

    public void StopTrack001()
    {
        if (musicLoopSource != null)
        {
            SoundFXManager.Instance.StopLooping(musicLoopSource);
            musicLoopSource = null;
        }
    }
}
