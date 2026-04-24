using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public CharacterController playerController;

    [Header("Basic Movement Settings")]
    public float speed;

    [Header("JumpSettings")]
    public float jumpForce;
    public int jumpCount;
    private float yVelocity;
    private int jumpsRemaining;

    [Header("DodgeSettings")]
    public float dodgeForce;
    public float dodgeCoolDown;
    public int dodgeCount;

    [Header("Gravity Settings")]
    public float gravity = -9.81f;

    [Header("Ground Check Settings")]
    public Transform groundCheck;
    public float groundDistance = 0.3f;
    public LayerMask groundMask;

    private PlayerInputAction inputActions;

    private Vector2 moveInput;

    private void Awake()
    {
        inputActions = new PlayerInputAction();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Jump.performed += OnJump;
    }

    private void OnDisable()
    {
        inputActions.Player.Jump.performed -= OnJump;
        inputActions.Player.Disable();
    }

    private void Update()
    {
        //dauerhafter Boden Check
        bool isGrounded = IsGrounded();

        if (isGrounded && yVelocity < 0)
        {
            yVelocity = -2f;
            jumpsRemaining = jumpCount;
        }

        moveInput = inputActions.Player.Move.ReadValue<Vector2>();

        float x = moveInput.x;
        float y = moveInput.y;

        Vector3 move = transform.right * x + transform.forward * y;

        move = move.normalized * speed;

        yVelocity += gravity * Time.deltaTime;
        move.y = yVelocity;

        playerController.Move(move * Time.deltaTime);
    }

    private void Jump()
    {
        if (jumpsRemaining > 0)
        {
            yVelocity = Mathf.Sqrt(jumpForce * -2f * gravity);
            jumpsRemaining--;
        }
    }

    private void Dodge()
    {

    }

    //Checkt ob Spieler gerade am Boden ist
    private bool IsGrounded()
    {
        return Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }

    private void OnJump(InputAction.CallbackContext ctx)
    {
        Jump();
    }
}
