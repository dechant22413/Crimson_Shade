using UnityEngine;

[CreateAssetMenu(fileName = "SurfaceData", menuName = "Game/Surface Data")]
public class SurfaceData : ScriptableObject
{
    public AudioEvent hitSound;
    public AudioEvent footstepSound;
}
