using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private AudioSource audioSource;

    [SerializeField] private List<Sound> sounds = new List<Sound>();

    private Dictionary<string, Sound> soundDict;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Dictionary bauen für schnellen Zugriff
        soundDict = new Dictionary<string, Sound>();

        foreach (Sound s in sounds)
        {
            if (!soundDict.ContainsKey(s.name))
                soundDict.Add(s.name, s);
        }
    }

    public void PlayAudio(string soundName)
    {
        if (soundDict.TryGetValue(soundName, out Sound sound))
        {
            audioSource.PlayOneShot(sound.clip, sound.volume);
        }
        else
        {
            Debug.LogWarning("Sound nicht gefunden: " + soundName);
        }
    }
}

