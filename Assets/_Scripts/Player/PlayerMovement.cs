using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    #region Singelton Initialization
    //Singleton
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
    #endregion

    #region Settings
    [Header("References")]
    public Transform cameraTransform;
    public InputActionReference moveAction;

    [Header("Basic Movement Settings")]
    [SerializeField] private float speed;

    [Header("Phsyics")]
    [SerializeField] private float gravity = 12f;
    [SerializeField] private float initialFallVelocity = -2f;
    [SerializeField] private float verticalVelocity;

    [Header("Dash Indication")]
    [SerializeField] private bool isDashing;
    #endregion

    private float dashTimer;
    private Vector3 dashDirection;
    private float dashForce;

    public Vector3 CurrentVelocity;

    private CharacterController playerController;
    private Vector2 moveInput;

    #region Move Action
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
    #endregion

    public void StartDash(Vector3 direction, float force, float duration)
    {
        //Startet einen Dash mit angegebenen Werten
        isDashing = true;
        dashDirection = direction;
        dashForce = force;
        dashTimer = duration; 
    }

    private void Update()
    {
        HandleGravity();
        HandleMovement();
    }

    private void HandleMovement()
    {
        if (isDashing)
        {
            //Überschreibt Player Bewegung während Dash
            playerController.Move(dashDirection * dashForce * Time.deltaTime);

            dashTimer -= Time.deltaTime; 

            if (dashTimer <= 0)
            {
                isDashing = false;
                verticalVelocity = 0f;
            }
            return;
        }

        // normale Bewegung des Spielers
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

        CurrentVelocity = playerController.velocity;
    }

    private void HandleGravity()
    {
        //Künstliche Schwerkraft ohne Rigidbody
        if(playerController.isGrounded && verticalVelocity < 0)
        {
            //Spieler hat einen automatischen Anpressdruck an den Boden, um floaty Bewegungen zu vermeiden
            verticalVelocity = initialFallVelocity;
        }

        verticalVelocity += gravity * Time.deltaTime;
    }

    private void StoreMovementInput(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    #region Zugriffsfunktionen für andere Skripte
    public bool IsGrounded()
    {
        return playerController.isGrounded;
    }

    public void SetVerticalVelocity(float input)
    {
        verticalVelocity = input;
    }

    public Vector2 GetMoveInput()
    {
        return moveInput;
    }
    #endregion
}
