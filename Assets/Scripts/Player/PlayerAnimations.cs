using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    #region Singelton Initialization
    //Singelton
    public static PlayerAnimations Instance;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }
    #endregion

    [Header("References")]
    public Animator leftArmAnimator;
    public Animator rightArmAnimator;

    public bool IsLeftArmPlaying { get; set; }
    public bool IsRightArmPlaying { get; set; }

    private int hitindex = 0;

    #region Animator Strings
    private static readonly int reload = Animator.StringToHash("Reload");
    private static readonly int shoot = Animator.StringToHash("Shoot");
    private static readonly int flip = Animator.StringToHash("Flip");
    private static readonly int fingerRoll = Animator.StringToHash("FingerRoll");
    private static readonly int hit1 = Animator.StringToHash("Hit1");
    private static readonly int hit2 = Animator.StringToHash("Hit2");
    private static readonly int hit3 = Animator.StringToHash("Hit3");
    private static readonly int[] hitTriggers = { hit1, hit2, hit3 };
    #endregion

    //Methoden für das Abspielen der Animationen, die daraufhin Animation Events auslösen
    public void PlayReload() => rightArmAnimator.SetTrigger(reload);
    public void PlayShoot() => rightArmAnimator.SetTrigger(shoot);
    public void PlayFlip() => rightArmAnimator.SetTrigger(flip);
    public void PlayFingerRoll() => leftArmAnimator.SetTrigger(fingerRoll);
    public void PlayHit()
    {
        //Abspielen einer Combo aus drei Animationen
        leftArmAnimator.SetTrigger(hitTriggers[hitindex]);

        hitindex++;

        if (hitindex > 2)
        {
            hitindex = 0;
        }
    }
}