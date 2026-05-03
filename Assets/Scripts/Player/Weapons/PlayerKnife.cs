using UnityEngine;

public class PlayerKnife : MonoBehaviour
{
    public void OnAnimationStart()
    {
        PlayerAnimations.Instance.IsLeftArmPlaying = true;
    }

    public void OnAnimationEnd()
    {
        PlayerAnimations.Instance.IsLeftArmPlaying = false;
    }
}
