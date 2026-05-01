using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    public static PlayerAnimations Instance;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    [Header("References")]
    public Animator leftArmAnimator;
    public Animator rightArmAnimator;

    private static readonly int shoot = Animator.StringToHash("Shoot");
    private static readonly int dash = Animator.StringToHash("Dash");
    private static readonly int hit = Animator.StringToHash("Hit");

    private static readonly int moveY = Animator.StringToHash("moveY");
    private static readonly int Shoot = Animator.StringToHash("moveX");



    void Update()
    {
        
    }
}
