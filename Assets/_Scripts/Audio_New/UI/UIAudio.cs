using UnityEngine;
using UnityEngine.EventSystems;

public class UIAudio : MonoBehaviour, IPointerDownHandler
{
    [Header("Sounds")]
    public AudioEventRef click001;
    public AudioEventRef click002;
    public AudioEventRef start;

    [Header("Settings")]
    [SerializeField] private bool playStartSound = false;

    private bool skipNextHoverSound = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        // Beim Klicken merken: der nächste Highlighted-Eintritt
        // kommt vom Loslassen, nicht von einem echten Hover-Beginn
        skipNextHoverSound = true;
    }

    public void PlayClick001()
    {
        click001.Play(transform);
        Debug.Log("Played");
    }

    public void PlayCLick002()
    {
        if (skipNextHoverSound)
        {
            skipNextHoverSound = false;
            return;
        }

        click002.Play(transform);
    }

    public void PlayStart()
    {
        if (!playStartSound) return;
        start.Play(transform);
    }
}