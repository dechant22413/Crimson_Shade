using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "AudioEvent", menuName = "Audio/Audio Event")]
public class AudioEvent : ScriptableObject
{
    public AudioClip[] clips;
    [Range(0f, 1f)] public float volume = 1f;
    [Range(0.5f, 1.5f)] public float pitchMin = 1f;
    [Range(0.5f, 1.5f)] public float pitchMax = 1f;
    [Range(0f, 1f)] public float spatialBlend = 1f;
    public bool is2D = false;
    public AudioMixerGroup mixerGroup;

    public float GetSpatialBlend()
    {
        // Wenn is2D aktiv ist, wird spatialBlend ignoriert und 0 zurückgegeben
        return is2D ? 0f : spatialBlend;
    }
}