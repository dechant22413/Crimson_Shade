using UnityEngine;
using UnityEngine.Audio;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager Instance;

    [SerializeField] private AudioSource audioClipObject;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    #region AudioCLip (Aktuell nicht gebraucht)
    public void Play(AudioClip clip, Transform spawnTransform, float volume = 1f)
    {
        AudioSource source = SpawnSource(spawnTransform);

        source.clip = clip;
        source.volume = volume;

        source.Play();

        Destroy(source.gameObject, clip.length);
    }
    #endregion

    public void Play(AudioEvent audioEvent, Transform spawnTransform, float volumeMultiplier = 1f)
    {
        if (audioEvent == null || audioEvent.clips.Length == 0)
            return;

        AudioClip clip = audioEvent.clips[Random.Range(0, audioEvent.clips.Length)];

        AudioSource source = SpawnSource(spawnTransform);

        source.clip = clip;
        source.volume = audioEvent.volume * volumeMultiplier;
        source.pitch = Random.Range(audioEvent.pitchMin, audioEvent.pitchMax);

        if (audioEvent.mixerGroup != null)
            source.outputAudioMixerGroup = audioEvent.mixerGroup;

        Debug.Log($"Audio Volume: {audioEvent.volume * volumeMultiplier}");
        source.Play();

        Destroy(source.gameObject, clip.length);
    }

    public AudioSource PlayLooping(AudioEvent audioEvent, Transform parent, float volumeMultiplier = 1f)
    {
        if (audioEvent == null || audioEvent.clips.Length == 0)
            return null;

        AudioClip clip = audioEvent.clips[Random.Range(0, audioEvent.clips.Length)];

        AudioSource source = Instantiate(audioClipObject, parent.position, Quaternion.identity);

        source.clip = clip;
        source.volume = audioEvent.volume * volumeMultiplier;
        source.pitch = Random.Range(audioEvent.pitchMin, audioEvent.pitchMax);
        source.loop = true;

        if (audioEvent.mixerGroup != null)
            source.outputAudioMixerGroup = audioEvent.mixerGroup;

        source.Play();

        return source;
    }

    public AudioSource PlayLoopingAttached(AudioEvent audioEvent, Transform parent, float volumeMultiplier = 1f)
    {
        if (audioEvent == null || audioEvent.clips.Length == 0)
            return null;

        AudioClip clip = audioEvent.clips[Random.Range(0, audioEvent.clips.Length)];

        AudioSource source = Instantiate(audioClipObject, parent);

        source.transform.localPosition = Vector3.zero;
        source.clip = clip;
        source.volume = audioEvent.volume * volumeMultiplier;
        source.pitch = Random.Range(audioEvent.pitchMin, audioEvent.pitchMax);
        source.loop = true;

        if (audioEvent.mixerGroup != null)
            source.outputAudioMixerGroup = audioEvent.mixerGroup;

        source.Play();

        return source;
    }

    public void StopLooping(AudioSource source)
    {
        if (source == null)
            return;

        source.Stop();
        Destroy(source.gameObject);
    }

    private AudioSource SpawnSource(Transform spawnTransform)
    {
        return Instantiate(audioClipObject, spawnTransform.position, Quaternion.identity);
    }
}