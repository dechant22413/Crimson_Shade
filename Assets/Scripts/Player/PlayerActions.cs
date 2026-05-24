using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class PlayerActions : MonoBehaviour
{
    #region Singleton Initialization
    //Singleton
    public static PlayerActions Instance;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    #endregion

    #region Settings
    [Header("References")]
    public InputActionReference jumpAction;
    public InputActionReference shootAction;
    public InputActionReference dashAction;
    public InputActionReference hitAction;
    public InputActionReference powerUpAction;
    public InputActionReference reloadAction;

    [Header("Dash Settings")]
    [SerializeField] private float dashForce = 20f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashStrain = 10;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce;
    #endregion

    private bool isAttacking = false;

    #region Actions
    private void OnEnable()
    {
        //Enabled alle Actions
        jumpAction.action.Enable();
        shootAction.action.Enable();
        dashAction.action.Enable();
        hitAction.action.Enable();
        powerUpAction.action.Enable();
        reloadAction.action.Enable();
        

        jumpAction.action.performed += Jump;
        shootAction.action.performed += Shoot;
        dashAction.action.performed += Dash;

        hitAction.action.performed += Hit;
        hitAction.action.canceled += HitCanceled;

        powerUpAction.action.performed += PowerUp;
        reloadAction.action.performed += Reload;

    }

    private void OnDisable()
    {
        //Disabled alle Actions
        jumpAction.action.performed -= Jump;
        shootAction.action.performed -= Shoot;
        dashAction.action.performed -= Dash;

        hitAction.action.performed -= Hit;
        hitAction.action.canceled -= HitCanceled;

        powerUpAction.action.performed -= PowerUp;
        reloadAction.action.performed -= Reload;

        jumpAction.action.Disable();
        shootAction.action.Disable();
        dashAction.action.Disable();
        hitAction.action.Disable();
        powerUpAction.action.Disable();
        reloadAction.action.Disable();
    }
    #endregion 

    private void Shoot(InputAction.CallbackContext context)
    {
        //Kein Input geben, wenn bereits eine Animation läuft
        if (PlayerAnimations.Instance.IsRightArmPlaying) return;

        //Spielt Schuss Animation ab
        PlayerAnimations.Instance.PlayShoot();
    }

    private void Reload(InputAction.CallbackContext context)
    {
        //Kein Input geben, wenn bereits eine Animation läuft
        if (PlayerAnimations.Instance.IsRightArmPlaying) return;

        //Spielt Reload Animation ab
        PlayerAnimations.Instance.PlayReload();
    }


    private void PowerUp(InputAction.CallbackContext context)
    {
        //PowerUp kann nicht aktiviert werden, wenn nicht vollständig aufgeladen
        if (PlayerStatsAndUIPanel.Instance.GetPowerUp() < PlayerStatsAndUIPanel.Instance.GetMaxPowerUp())
        {
            Debug.Log("Not Enough PowerUp");
            return;
        }

        //PowerUp kann nicht aktiviert werden, wenn Leben bereits voll
        if(PlayerStatsAndUIPanel.Instance.GetMaxLifePoints() == PlayerStatsAndUIPanel.Instance.GetCurrentLifePoints())
        {
            Debug.Log("Already full Health");
            return;
        }

        //Aktiviert PowerUp
        PlayerStatsAndUIPanel.Instance.ActivatePowerUp();
    }

    private void Dash(InputAction.CallbackContext context)
    {
        //Dash kann nur aktiviert werden, wenn genug Stamina vorhanden
        if(PlayerStatsAndUIPanel.Instance.GetStamina() < dashStrain)
        {
            Debug.Log("Not Enough Stamina");
            return;
        }

        Vector2 input = PlayerMovement.Instance.GetMoveInput();

        Transform cam = PlayerMovement.Instance.cameraTransform;

        Vector3 forward = cam.forward;
        Vector3 right = cam.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        Vector3 moveDir = forward * input.y + right * input.x;

        if (moveDir == Vector3.zero)
            moveDir = - forward;

        moveDir.Normalize();

        //Startet einen Dash in BLickrichtung des Spielers mit angegebener Duration und Force
        PlayerMovement.Instance.StartDash(moveDir, dashForce, dashDuration);

        //Verbraucht Stamina
        PlayerStatsAndUIPanel.Instance.UseStamina(dashStrain);

        Debug.Log("Dash");
    }

    private void Jump(InputAction.CallbackContext context)
    {
        //Springe nur, wenn Grounded
        if (PlayerMovement.Instance.IsGrounded())
        {
            PlayerMovement.Instance.SetVerticalVelocity(jumpForce);
            Debug.Log("Jump");
        }
    }

    private void Hit(InputAction.CallbackContext context)
    {
        isAttacking = true;
    }

    private void HitCanceled(InputAction.CallbackContext context)
    {
        isAttacking = false;
    }

    private void Update()
    {
        //Kein Input geben, wenn bereits eine Animation läuft
        if (PlayerAnimations.Instance.IsLeftArmPlaying) return;

        //Spielt Hit Animation ab, solange der Input gehalten wird
        if (isAttacking)
            PlayerAnimations.Instance.PlayHit();
    }
}
