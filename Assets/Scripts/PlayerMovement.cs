using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public Transform cameraTransform;
    public InputActionReference moveAction;
    public InputActionReference jumpAction;

    [Header("Basic Movement Settings")]
    public float speed;

    [Header("Phsyics")]
    [SerializeField] private float gravity = 12f;
    [SerializeField] private float initialFallVelocity = -2f;
    private float verticalVelocity;

    [Header("Jump Settings")]
    public float jumpForce;

    [Header("Ground Check Settings")]
    [SerializeField] private bool isGrounded;


    private CharacterController playerController;
    private Vector2 moveInput;

    private void Awake()
    {
        playerController = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        moveAction.action.Enable();
        jumpAction.action.Enable();

        moveAction.action.performed += StoreMovementInput;
        moveAction.action.canceled += StoreMovementInput;
        jumpAction.action.performed += Jump;
    }

    private void OnDisable()
    {
        moveAction.action.performed -= StoreMovementInput;
        moveAction.action.canceled -= StoreMovementInput;
        jumpAction.action.performed += Jump;

        moveAction.action.Disable();
        jumpAction.action.Disable();
    }


    private void Update()
    {
        isGrounded = playerController.isGrounded;

        HandleGravity();
        HandleMovement();

    }

    private void HandleMovement()
    {
        var move = cameraTransform.TransformDirection(new Vector3(moveInput.x, 0, moveInput.y)).normalized;
        var currentSpeed = speed;
        var finalMove = move * currentSpeed;
        finalMove.y = verticalVelocity;

        playerController.Move(finalMove * Time.deltaTime);
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if(isGrounded)
        {
            verticalVelocity = jumpForce;
            Debug.Log("Jump");
        }
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
