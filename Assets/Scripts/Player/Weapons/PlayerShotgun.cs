using UnityEngine;

public class PlayerShotgun : MonoBehaviour
{
    public void OnAnimationStart()
    {
        PlayerAnimations.Instance.IsRightArmPlaying = true;
    }

    public void OnAnimationEnd()
    {
        PlayerAnimations.Instance.IsRightArmPlaying = false;
    }
}
