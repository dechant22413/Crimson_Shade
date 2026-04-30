using UnityEngine;
using UnityEngine.InputSystem;

public class HeadBobbing : MonoBehaviour
{
    [Header("Input")]
    public InputActionReference moveAction;

    [Header("Bob Settings")]
    public float bobSpeed = 10f;
    public float bobAmount = 0.05f;

    [Header("Smooth")]
    public float smooth = 12f;

    private Vector3 baseLocalPos;
    private Vector3 currentOffset;
    private float bobTimer;

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


        if (grounded && speed > 0.1f)
        {
            bobTimer += Time.deltaTime * bobSpeed * speed;
        }

        Vector3 targetOffset = Vector3.zero;

        if (grounded && speed > 0.1f)
        {
            float bobX = Mathf.Sin(bobTimer) * bobAmount * speed;
            float bobY = Mathf.Cos(bobTimer * 2f) * bobAmount * 0.5f * speed;

            targetOffset = new Vector3(bobX, bobY, 0f);
        }

        currentOffset = Vector3.Lerp(currentOffset, targetOffset, Time.deltaTime * smooth);

        transform.localPosition = baseLocalPos + currentOffset;
    }
}