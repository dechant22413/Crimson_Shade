using UnityEngine;
using System.Collections;

public class BackgroundMusic : MonoBehaviour
{
    [Header("Sounds")]
    public AudioEventRef track001;
    public AudioEventRef bossTrack;

    private AudioSource musicLoopSource;
    private Coroutine fadeCoroutine;

    private void Start()
    {
        PlayTrack001();
    }

    public void PlayTrack001()
    {
        if (musicLoopSource != null)
            return;

        musicLoopSource = track001.PlayLooping(transform);

        if (musicLoopSource != null)
        {
            musicLoopSource.volume = GetTargetVolume(track001);
        }
    }

    public void StopTrack001()
    {
        if (musicLoopSource != null)
        {
            SoundFXManager.Instance.StopLooping(musicLoopSource);
            musicLoopSource = null;
        }
    }

    public void SwitchToBossTrack()
    {
        StartCrossFade(bossTrack);
    }

    public void SwitchToAmbience()
    {
        StartCrossFade(track001);
    }

    private void StartCrossFade(AudioEventRef targetTrack)
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }

        fadeCoroutine = StartCoroutine(CrossFade(targetTrack));
    }

    private IEnumerator CrossFade(AudioEventRef targetTrack)
    {
        float fadeDuration = 1.5f;
        float elapsed = 0f;

        AudioSource oldSource = musicLoopSource;

        // Falls bereits derselbe Track läuft, nichts machen
        if (oldSource != null &&
            oldSource.clip != null &&
            targetTrack.audioEvent != null &&
            targetTrack.audioEvent.clips != null)
        {
            bool sameTrack = false;

            foreach (AudioClip clip in targetTrack.audioEvent.clips)
            {
                if (clip == oldSource.clip)
                {
                    sameTrack = true;
                    break;
                }
            }

            if (sameTrack)
                yield break;
        }

        // Ziel-Lautstärke aus AudioEvent + Multiplier
        float targetVolume = GetTargetVolume(targetTrack);

        // Neuen Track starten
        AudioSource newSource = targetTrack.PlayLooping(transform);

        if (newSource == null)
        {
            fadeCoroutine = null;
            yield break;
        }

        // Neuer Track startet bei 0
        newSource.volume = 0f;

        // Alte Lautstärke merken
        float oldVolume = oldSource != null ? oldSource.volume : 0f;

        musicLoopSource = newSource;

        // Crossfade
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / fadeDuration);

            // Alte Musik ausblenden
            if (oldSource != null)
            {
                oldSource.volume = Mathf.Lerp(oldVolume, 0f, t);
            }

            // Neue Musik einblenden
            newSource.volume = Mathf.Lerp(0f, targetVolume, t);

            yield return null;
        }

        // Sicherheit: finale Werte setzen
        newSource.volume = targetVolume;

        // Alten Track stoppen
        if (oldSource != null)
        {
            SoundFXManager.Instance.StopLooping(oldSource);
        }

        fadeCoroutine = null;
    }

    private float GetTargetVolume(AudioEventRef audioEventRef)
    {
        if (audioEventRef == null || audioEventRef.audioEvent == null)
            return 0f;

        return audioEventRef.audioEvent.volume * audioEventRef.volumeMultiplier;
    }
}