using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    #region Singleton Initialization

    public static AudioManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        Initialize();
    }

    #endregion

    #region Settings

    [Header("Sound Clip List")]
    [SerializeField] private List<Sound> sounds = new List<Sound>();

    private Dictionary<string, Sound> soundDict;

    #endregion

    private void Initialize()
    {
        soundDict = new Dictionary<string, Sound>();

        foreach (Sound s in sounds)
        {
            if (!soundDict.ContainsKey(s.name))
                soundDict.Add(s.name, s);
        }
    }

    public void PlayAudio(string soundName, AudioSource source)
    {
        if (soundDict.TryGetValue(soundName, out Sound sound))
        {
            source.PlayOneShot(sound.clip, sound.volume);
        }
        else
        {
            Debug.LogWarning("Sound nicht gefunden: " + soundName);
        }
    }
}

