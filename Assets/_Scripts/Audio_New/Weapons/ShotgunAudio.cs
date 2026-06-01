using UnityEngine;

public class ShotgunAudio : MonoBehaviour
{
    [Header("Sounds")]
    public AudioEventRef reloadPartOne;
    public AudioEventRef reloadPartTwo;
    public AudioEventRef attack;
    public AudioEventRef empty;

    public void PlayReloadPartOne() => reloadPartOne.Play(transform);
    public void PlayReloadPartTwo() => reloadPartTwo.Play(transform);
    public void PlayAttack() => attack.Play(transform);
    public void PlayEmpty() => empty.Play(transform);
}
