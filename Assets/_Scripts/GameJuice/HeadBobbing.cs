using UnityEngine;
using UnityEngine.InputSystem;

public class HeadBobbing : MonoBehaviour
{
    #region Settings
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

    [Header("Audio")]
    [SerializeField] private PlayerAudio playerAudio;
    #endregion

    private Vector3 baseLocalPos;
    private Vector3 currentOffset;
    private float bobTimer;
    private float idleBobTimer;
    private float lastCosValue;


    private bool wasInFootstepZone = false;

    private void Awake()
    {
        baseLocalPos = transform.localPosition;
        moveAction.action.Enable();
    }

    private void Update()
    {
        Vector2 move = moveAction.action.ReadValue<Vector2>();
        float speed = move.magnitude;

        bool grounded = PlayerMovement.Instance != null &&
                        PlayerMovement.Instance.IsGrounded();

        bool walking = grounded && speed > 0.1f;

        if (walking)
        {
            bobTimer += Time.deltaTime * bobSpeed * speed;
            bobTimer = Mathf.Repeat(bobTimer, Mathf.PI * 2f);
        }
        else
        {
            bobTimer = Mathf.Lerp(
                bobTimer,
                Mathf.Round(bobTimer / (Mathf.PI * 2f)) * (Mathf.PI * 2f),
                Time.deltaTime * smooth
            );

            wasInFootstepZone = false;
        }

        idleBobTimer += Time.deltaTime * idleBobSpeed;
        idleBobTimer = Mathf.Repeat(idleBobTimer, Mathf.PI * 2f);

        Vector3 targetOffset = Vector3.zero;

        if (walking)
        {
            float bobSide = Mathf.Sin(bobTimer) * bobAmount * speed;
            float bobUp = Mathf.Cos(bobTimer * 2f) * bobAmount * 0.5f * speed;

            // Footstep auslösen, wenn der Headbob unten ist
            CheckFootstep(bobTimer);

            Vector3 worldOffset =
                cameraTransform.right * bobSide +
                cameraTransform.up * bobUp;

            targetOffset =
                transform.parent.InverseTransformDirection(worldOffset);
        }
        else if (grounded)
        {
            float idleBobY = Mathf.Sin(idleBobTimer) * idleBobAmount;
            targetOffset = new Vector3(0f, idleBobY, 0f);
        }

        currentOffset = Vector3.Lerp(
            currentOffset,
            targetOffset,
            Time.deltaTime * smooth
        );

        transform.localPosition = baseLocalPos + currentOffset;
    }

    private void CheckFootstep(float timer)
    {
        float cosValue = Mathf.Cos(timer * 2f);

        // Nulldurchgang von positiv nach negativ = tiefster Punkt kommt
        // Nulldurchgang von negativ nach positiv = höchster Punkt kommt
        // Wir wollen wenn cos von positiv zu negativ wechselt (absteigende Flanke)
        if (lastCosValue > 0f && cosValue <= 0f)
        {
            Footstep();
        }

        lastCosValue = cosValue;
    }

    private void Footstep()
    {
        if (playerAudio == null)
            return;
        playerAudio.PlayFootStep();
        Debug.Log("Footstep");
    }
}
