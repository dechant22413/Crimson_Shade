using UnityEngine;
using Unity.Cinemachine;

public class FollowCameraRotation : MonoBehaviour
{
    public Transform cameraTransform;
    [Header("Smoothing")]
    public float smooth = 12f;

    void OnEnable()
    {
        CinemachineCore.CameraUpdatedEvent.AddListener(OnCameraUpdated);
    }

    void OnDisable()
    {
        CinemachineCore.CameraUpdatedEvent.RemoveListener(OnCameraUpdated);
    }

    void OnCameraUpdated(CinemachineBrain brain)
    {
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            cameraTransform.rotation,
            Time.deltaTime * smooth
        );
    }
}