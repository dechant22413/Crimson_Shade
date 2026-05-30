using UnityEngine;

public class ShotgunAudio : MonoBehaviour
{
    [Header("Sounds")]
    public AudioEventRef reload;
    public AudioEventRef attack;
    public AudioEventRef empty;

    public void PlayReload() => reload.Play(transform);

    public void PlayAttack() => attack.Play(transform);

    public void PlayEmpty() => empty.Play(transform);
}
