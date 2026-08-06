using UnityEngine;

public class BackgroundNoises : MonoBehaviour
{
    [Header("Sounds")]
    public AudioEventRef noise001;

    private AudioSource musicLoopSource;

    private void Start()
    {
        PlayNoise001();
    }

    public void PlayNoise001()
    {
        if (musicLoopSource == null)
            musicLoopSource = noise001.PlayLooping(transform);
    }

    public void StopNoise001()
    {
        if (musicLoopSource != null)
        {
            SoundFXManager.Instance.StopLooping(musicLoopSource);
            musicLoopSource = null;
        }
    }
}
