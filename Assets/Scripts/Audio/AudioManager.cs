using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    #region Singleton Initialization
    //Singleton
    public static AudioManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        Initialize();
    }
    #endregion

    [Header("References")]
    [SerializeField] private AudioSource audioSource;

    [Header("Sound Clip List")]
    [SerializeField] private List<Sound> sounds = new List<Sound>();
    private Dictionary<string, Sound> soundDict;

    private void Initialize()
    {
        soundDict = new Dictionary<string, Sound>();
        foreach (Sound s in sounds)
        {
            if (!soundDict.ContainsKey(s.name))
                soundDict.Add(s.name, s);
        }
    }

    public void PlayAudio(string soundName)
    {
        //Spielt Sound CLip von soundDictionary per String Input ab
        if (soundDict.TryGetValue(soundName, out Sound sound))
            audioSource.PlayOneShot(sound.clip, sound.volume);
        else
            Debug.LogWarning("Sound nicht gefunden: " + soundName);
    }
}

