using UnityEngine;
using UnityEngine.InputSystem;

public class HeadBobbing : MonoBehaviour
{
    [Header("References")]
    public InputActionReference moveAction;

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
        bool grounded = PlayerMovement.Instance != null && PlayerMovement.Instance.isGrounded;
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
            float bobX = Mathf.Sin(bobTimer) * bobAmount * speed;
            float bobY = Mathf.Cos(bobTimer * 2f) * bobAmount * 0.5f * speed;
            targetOffset = new Vector3(bobX, bobY, 0f);
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