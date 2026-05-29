using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    #region Singleton Initialization
    public static AudioManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        Initialize();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void CreateInstance()
    {
        if (Instance != null) return;
        AudioManager prefab = Resources.Load<AudioManager>("AudioManager");
        if (prefab != null)
            Instantiate(prefab);
    }
    #endregion

    #region Settings
    [Header("Legacy Sounds (bestehende Liste bleibt erhalten)")]
    [SerializeField] private List<Sound> sounds = new List<Sound>();

    [Header("Sound Groups")]
    [SerializeField] private List<SoundGroup> soundGroups = new List<SoundGroup>();
    #endregion

    private Dictionary<string, Sound> soundDict;
    private Dictionary<string, SoundPatch> patchDict;

    private void Initialize()
    {
        soundDict = new Dictionary<string, Sound>();
        patchDict = new Dictionary<string, SoundPatch>();

        // Legacy sounds
        foreach (Sound s in sounds)
            if (!soundDict.ContainsKey(s.name))
                soundDict.Add(s.name, s);

        // Sounds und Patches aus Gruppen
        foreach (SoundGroup group in soundGroups)
        {
            foreach (Sound s in group.sounds)
                if (!soundDict.ContainsKey(s.name))
                    soundDict.Add(s.name, s);

            foreach (SoundPatch patch in group.patches)
                if (!patchDict.ContainsKey(patch.name))
                    patchDict.Add(patch.name, patch);
        }
    }

    public void PlayAudio(string soundName, AudioSource source)
    {
        // Erst in Patches suchen
        if (patchDict.TryGetValue(soundName, out SoundPatch patch))
        {
            if (patch.sounds.Count == 0) return;
            Sound random = patch.sounds[Random.Range(0, patch.sounds.Count)];
            source.PlayOneShot(random.clip, random.volume);
            return;
        }

        // Dann in Sounds suchen
        if (soundDict.TryGetValue(soundName, out Sound sound))
        {
            source.PlayOneShot(sound.clip, sound.volume);
            return;
        }

        Debug.LogWarning("Sound nicht gefunden: " + soundName);
    }
}

