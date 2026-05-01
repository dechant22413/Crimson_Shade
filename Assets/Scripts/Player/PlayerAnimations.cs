using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    public static PlayerAnimations Instance;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    [Header("References")]
    public Animator leftArmAnimator;
    public Animator rightArmAnimator;

    private static readonly int reload = Animator.StringToHash("Reload");
    private static readonly int shoot = Animator.StringToHash("Shoot");
    private static readonly int flip = Animator.StringToHash("Flip");
    private static readonly int fingerRoll = Animator.StringToHash("FingerRoll");

    private static readonly int hit1 = Animator.StringToHash("Hit1");
    private static readonly int hit2 = Animator.StringToHash("Hit2");
    private static readonly int hit3 = Animator.StringToHash("Hit3");
    private static readonly int[] hitTriggers = { hit1, hit2, hit3 };
    private static readonly string[] hitStates = { "Hit1", "Hit2", "Hit3" };

    private bool TryPlay(Animator animator, int triggerHash, string stateName)
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName(stateName)) return false;
        if (animator.IsInTransition(0)) return false;

        foreach (var param in animator.parameters)
            if (param.type == AnimatorControllerParameterType.Trigger)
                animator.ResetTrigger(param.nameHash);

        animator.SetTrigger(triggerHash);
        return true;
    }

    public void Reload() => TryPlay(rightArmAnimator, reload, "Reload");
    public void Shoot() => TryPlay(rightArmAnimator, shoot, "Shoot");
    public void Flip() => TryPlay(rightArmAnimator, flip, "Flip");
    public void FingerRoll() => TryPlay(leftArmAnimator, fingerRoll, "FingerRoll");

    public void Hit()
    {
        var stateInfo = leftArmAnimator.GetCurrentAnimatorStateInfo(0);
        bool anyHitPlaying = stateInfo.IsName("Hit1") || stateInfo.IsName("Hit2") || stateInfo.IsName("Hit3");
        if (anyHitPlaying || leftArmAnimator.IsInTransition(0)) return;

        foreach (var param in leftArmAnimator.parameters)
            if (param.type == AnimatorControllerParameterType.Trigger)
                leftArmAnimator.ResetTrigger(param.nameHash);

        int index = Random.Range(0, hitTriggers.Length);
        leftArmAnimator.SetTrigger(hitTriggers[index]);
    }
}