using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class IdleMovement : MonoBehaviour
{
    [Header("References")]
    public CinemachineCamera cam;
    public InputActionReference moveAction;

    [Header("Idle Settings")]
    public float idleAmplitude = 0f;
    public float idleFrequency = 0f;

    private CinemachineBasicMultiChannelPerlin noise;

    private void Start()
    {
        noise = cam.GetComponent<CinemachineBasicMultiChannelPerlin>();
    }

    void Update()
    {
        Vector2 move = moveAction.action.ReadValue<Vector2>();
        float speed = move.magnitude;

        if (speed > 0.1f)
        {
            noise.AmplitudeGain = 0;
            noise.FrequencyGain = 0;
        }
        else if(speed < 0.1f && PlayerMovement.Instance.IsGrounded())
        {
            noise.AmplitudeGain = idleAmplitude;
            noise.FrequencyGain = idleFrequency;
        }
    }
}
