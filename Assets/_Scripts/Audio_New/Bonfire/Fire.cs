using UnityEngine;

public class Fire : MonoBehaviour
{
    private BonfireAudio fireAudio;
    void Start()
    {
        fireAudio = GetComponent<BonfireAudio>();

        fireAudio.PlayLoopSound();
    }
}
