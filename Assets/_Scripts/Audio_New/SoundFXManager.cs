using UnityEngine;
using UnityEngine.Audio;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager Instance;

    [SerializeField] private AudioSource audioClipObject;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    // Einzelner Clip mit optionalem Volume Override
    public void Play(AudioClip clip, Transform spawnTransform, float volume = 1f, float volumeOverride = -1f)
    {
        float finalVolume = volumeOverride >= 0 ? volumeOverride : volume;
        AudioSource source = SpawnSource(spawnTransform);
        source.clip = clip;
        source.volume = finalVolume;
        source.Play();
        Destroy(source.gameObject, clip.length);
    }

    // AudioEvent – Volume auf dem Asset, Override möglich
    public void Play(AudioEvent audioEvent, Transform spawnTransform, float volumeOverride = -1f)
    {
        if (audioEvent == null || audioEvent.clips.Length == 0) return;

        AudioClip clip = audioEvent.clips[Random.Range(0, audioEvent.clips.Length)];
        float finalVolume = volumeOverride >= 0 ? volumeOverride : audioEvent.volume;

        AudioSource source = SpawnSource(spawnTransform);
        source.clip = clip;
        source.volume = finalVolume;
        source.pitch = Random.Range(audioEvent.pitchMin, audioEvent.pitchMax);

        if (audioEvent.mixerGroup != null)
            source.outputAudioMixerGroup = audioEvent.mixerGroup;

        source.Play();
        Destroy(source.gameObject, clip.length);
    }

    private AudioSource SpawnSource(Transform spawnTransform)
    {
        return Instantiate(audioClipObject, spawnTransform.position, Quaternion.identity);
    }
}
