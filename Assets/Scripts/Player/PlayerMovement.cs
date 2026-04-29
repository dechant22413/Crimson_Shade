using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        playerController = GetComponent<CharacterController>();
    }

    [Header("References")]
    public Transform cameraTransform;
    public InputActionReference moveAction;

    [Header("Basic Movement Settings")]
    public float speed;

    [Header("Phsyics")]
    [SerializeField] private float gravity = 12f;
    [SerializeField] private float initialFallVelocity = -2f;
    public float verticalVelocity;

    [Header("Ground Check Settings")]
    public bool isGrounded;

    [Header("Dash Indication")]
    public bool isDashing;
    private float dashTimer;
    private Vector3 dashDirection;
    private float dashForce;


    private CharacterController playerController;
    private Vector2 moveInput;

    private void OnEnable()
    {
        moveAction.action.Enable();

        moveAction.action.performed += StoreMovementInput;
        moveAction.action.canceled += StoreMovementInput;
    }

    private void OnDisable()
    {
        moveAction.action.performed -= StoreMovementInput;
        moveAction.action.canceled -= StoreMovementInput;

        moveAction.action.Disable();
    }

    public void StartDash(Vector3 direction, float force, float duration)
    {
        isDashing = true;
        dashDirection = direction;
        dashForce = force;
        dashTimer = duration;
    }

    public Vector2 GetMoveInput()
    {
        return moveInput;
    }

    private void Update()
    {
        isGrounded = playerController.isGrounded;

        HandleGravity();
        HandleMovement();

    }

    private void HandleMovement()
    {
        if (isDashing)
        {
            playerController.Move(dashDirection * dashForce * Time.deltaTime);

            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0)
            {
                isDashing = false;
            }

            return;
        }

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        Vector3 move = forward * moveInput.y + right * moveInput.x;

        Vector3 finalMove = move * speed;
        finalMove.y = verticalVelocity;

        playerController.Move(finalMove * Time.deltaTime);
    }

    private void HandleGravity()
    {
        if(isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = initialFallVelocity;
        }

        verticalVelocity += gravity * Time.deltaTime;
    }

    private void StoreMovementInput(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }
}
