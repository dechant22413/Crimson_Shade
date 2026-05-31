using System.Runtime.ConstrainedExecution;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerActions : MonoBehaviour
{
    #region Singleton
    public static PlayerActions Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    #endregion

    [Header("References")]
    public InputActionReference jumpAction;
    public InputActionReference dashAction;
    public InputActionReference leftAttackAction;
    public InputActionReference rightAttackAction;
    public InputActionReference powerUpAction;
    public InputActionReference reloadAction;

    [Header("Weapon Slots")]
    public Weapon leftHandWeapon;
    public Weapon rightHandWeapon;

    [Header("Dash Settings")]
    [SerializeField] private float dashForce = 20f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashStrain = 10f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce;

    private void OnEnable()
    {
        jumpAction.action.Enable();
        dashAction.action.Enable();
        leftAttackAction.action.Enable();
        rightAttackAction.action.Enable();
        powerUpAction.action.Enable();
        reloadAction.action.Enable();

        jumpAction.action.performed += Jump;
        dashAction.action.performed += Dash;
        powerUpAction.action.performed += PowerUp;
        reloadAction.action.performed += Reload;

        leftAttackAction.action.performed += LeftAttackPerformed;
        leftAttackAction.action.canceled += LeftAttackCanceled;

        rightAttackAction.action.performed += RightAttackPerformed;
        rightAttackAction.action.canceled += RightAttackCanceled;
    }

    private void OnDisable()
    {
        jumpAction.action.performed -= Jump;
        dashAction.action.performed -= Dash;
        powerUpAction.action.performed -= PowerUp;
        reloadAction.action.performed -= Reload;

        leftAttackAction.action.performed -= LeftAttackPerformed;
        leftAttackAction.action.canceled -= LeftAttackCanceled;

        rightAttackAction.action.performed -= RightAttackPerformed;
        rightAttackAction.action.canceled -= RightAttackCanceled;

        jumpAction.action.Disable();
        dashAction.action.Disable();
        leftAttackAction.action.Disable();
        rightAttackAction.action.Disable();
        powerUpAction.action.Disable();
        reloadAction.action.Disable();
    }

    private void LeftAttackPerformed(InputAction.CallbackContext context)
    {
        leftHandWeapon?.OnAttackPressed();
    }

    private void LeftAttackCanceled(InputAction.CallbackContext context)
    {
        leftHandWeapon?.OnAttackReleased();
    }

    private void RightAttackPerformed(InputAction.CallbackContext context)
    {
        rightHandWeapon?.OnAttackPressed();
    }

    private void RightAttackCanceled(InputAction.CallbackContext context)
    {
        rightHandWeapon?.OnAttackReleased();
    }

    private void Reload(InputAction.CallbackContext context)
    {
        rightHandWeapon?.OnReload();
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (!PlayerMovement.Instance.IsGrounded())
            return;

        PlayerMovement.Instance.SetVerticalVelocity(jumpForce);

        PlayerAudio.Instance.PlayJump();
    }

    private void Dash(InputAction.CallbackContext context)
    {
        if (PlayerStatsAndUIPanel.Instance.GetStamina() < dashStrain)
            return;

        Vector2 input = PlayerMovement.Instance.GetMoveInput();

        Transform cam = PlayerMovement.Instance.cameraTransform;

        Vector3 forward = cam.forward;
        Vector3 right = cam.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        Vector3 moveDir =
            forward * input.y +
            right * input.x;

        if (moveDir == Vector3.zero)
            moveDir = -forward;

        moveDir.Normalize();

        PlayerMovement.Instance.StartDash(
            moveDir,
            dashForce,
            dashDuration);

        PlayerStatsAndUIPanel.Instance.UseStamina(
            dashStrain);

        PlayerAudio.Instance.PlayDash();
    }

    private void PowerUp(InputAction.CallbackContext context)
    {
        if (PlayerStatsAndUIPanel.Instance.GetPowerUp()
            < PlayerStatsAndUIPanel.Instance.GetMaxPowerUp())
            return;

        if (PlayerStatsAndUIPanel.Instance.GetCurrentLifePoints()
            == PlayerStatsAndUIPanel.Instance.GetMaxLifePoints())
            return;

        PlayerStatsAndUIPanel.Instance.ActivatePowerUp();

        PlayerAudio.Instance.PlayHeal();
    }
}
