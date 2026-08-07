using UnityEngine;

public class DoorAudio : MonoBehaviour
{
    [Header("Sounds")]
    public AudioEventRef open;
    public void PlayOpenDoor() => open.Play(transform);

}
