using UnityEngine;
using UnityEngine.InputSystem;

public class HeadBobbing : MonoBehaviour
{
    [Header("References")]
    public InputActionReference moveAction;
    public Transform cameraTransform;

    [Header("Walk Bob Settings")]
    public float bobSpeed = 10f;
    public float bobAmount = 0.05f;

    [Header("Idle Bob Settings")]
    public float idleBobSpeed = 1.5f;
    public float idleBobAmount = 0.005f;

    [Header("Smooth")]
    public float smooth = 12f;

    private Vector3 baseLocalPos;
    private Vector3 currentOffset;
    private float bobTimer;
    private float idleBobTimer;

    private void Awake()
    {
        baseLocalPos = transform.localPosition;
        moveAction.action.Enable();
    }

    private void Update()
    {
        Vector2 move = moveAction.action.ReadValue<Vector2>();
        float speed = move.magnitude;
        bool grounded = PlayerMovement.Instance != null && PlayerMovement.Instance.IsGrounded();
        bool walking = grounded && speed > 0.1f;

        if (walking)
        {
            bobTimer += Time.deltaTime * bobSpeed * speed;
            bobTimer = Mathf.Repeat(bobTimer, Mathf.PI * 2f);
        }
        else
        {
            bobTimer = Mathf.Lerp(bobTimer, Mathf.Round(bobTimer / (Mathf.PI * 2f)) * (Mathf.PI * 2f), Time.deltaTime * smooth);
        }

        idleBobTimer += Time.deltaTime * idleBobSpeed;
        idleBobTimer = Mathf.Repeat(idleBobTimer, Mathf.PI * 2f);

        Vector3 targetOffset = Vector3.zero;

        if (walking)
        {
            float bobSide = Mathf.Sin(bobTimer) * bobAmount * speed;
            float bobUp = Mathf.Cos(bobTimer * 2f) * bobAmount * 0.5f * speed;

            // Offset in Camera Space berechnen, dann in Local Space des Camera Root umrechnen
            Vector3 worldOffset = cameraTransform.right * bobSide + cameraTransform.up * bobUp;
            targetOffset = transform.parent.InverseTransformDirection(worldOffset);
        }
        else if (grounded)
        {
            float idleBobY = Mathf.Sin(idleBobTimer) * idleBobAmount;
            targetOffset = new Vector3(0f, idleBobY, 0f);
        }

        currentOffset = Vector3.Lerp(currentOffset, targetOffset, Time.deltaTime * smooth);
        transform.localPosition = baseLocalPos + currentOffset;
    }
}