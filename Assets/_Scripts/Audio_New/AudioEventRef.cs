using UnityEngine;

[System.Serializable]
public class AudioEventRef
{
    public AudioEvent audioEvent;

    [Range(0f, 2f)]
    public float volumeMultiplier = 1f;

    public void Play(Transform transform)
    {
        SoundFXManager.Instance.Play(audioEvent, transform, volumeMultiplier);
    }

    public AudioSource PlayLooping(Transform parent)
    {
        return SoundFXManager.Instance.PlayLooping(audioEvent, parent, volumeMultiplier);
    }
}
