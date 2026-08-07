using UnityEngine;

public class ItemAudio : MonoBehaviour
{
    [Header("Sounds")]
    public AudioEventRef pickUp;

    public void PlayPickUp() => pickUp.Play(transform);

}

