using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerActions : MonoBehaviour
{
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

    [Header("References")]
    public InputActionReference jumpAction;
    public InputActionReference shootAction;
    public InputActionReference dashAction;
    public InputActionReference hitAction;
    public InputActionReference powerUpAction;

    [Header("Gun Settings")]
    public int damage;
    public float reloadTime;
    public int shotCount;

    [Header("Dash Settings")]
    public float dashForce = 20f;
    public float dashDuration = 0.2f;
    public int dashCount;

    [Header("Jump Settings")]
    public float jumpForce;

    private void OnEnable()
    {
        jumpAction.action.Enable();
        shootAction.action.Enable();
        dashAction.action.Enable();
        hitAction.action.Enable();
        powerUpAction.action.Enable();

        jumpAction.action.performed += Jump;
        shootAction.action.performed += Shoot;
        dashAction.action.performed += Dash;
        hitAction.action.performed += Hit;
        powerUpAction.action.performed += PowerUp;

    }

    private void OnDisable()
    {
        jumpAction.action.performed -= Jump;
        shootAction.action.performed -= Shoot;
        dashAction.action.performed -= Dash;
        hitAction.action.performed -= Hit;
        powerUpAction.action.performed -= PowerUp;

        jumpAction.action.Disable();
        shootAction.action.Disable();
        dashAction.action.Disable();
        hitAction.action.Disable();
        powerUpAction.action.Disable();
    }

    private void Shoot(InputAction.CallbackContext context)
    {

    }

    private void Hit(InputAction.CallbackContext context)
    {
        
    }

    private void PowerUp(InputAction.CallbackContext context)
    {

    }

    private void Dash(InputAction.CallbackContext context)
    {
        Debug.Log("Dash");

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
            moveDir = forward;

        moveDir.Normalize();

        PlayerMovement.Instance.StartDash(moveDir, dashForce, dashDuration);
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (PlayerMovement.Instance.isGrounded)
        {
            PlayerMovement.Instance.verticalVelocity = jumpForce;
            Debug.Log("Jump");
        }
    }

    private void Reload()
    {

    }
}
