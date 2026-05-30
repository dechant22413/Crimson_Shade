using UnityEngine;

[System.Serializable]
public class AudioEventRef
{
    public AudioEvent audioEvent;
    [Range(0f, 1f)] public float volumeOverride = 1f;

    public void Play(Transform transform)
    {
        SoundFXManager.Instance.Play(audioEvent, transform, volumeOverride);
    }
}
