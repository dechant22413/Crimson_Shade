using UnityEngine;

public class ChestAudio : MonoBehaviour
{
    [Header("Sounds")]
    public AudioEventRef openLock;
    public AudioEventRef openLid;

    public void OpenLock() => openLock.Play(transform);
    public void OpenLid() => openLid.Play(transform);
}
