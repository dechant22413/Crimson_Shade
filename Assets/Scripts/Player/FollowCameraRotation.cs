using UnityEngine;
using Unity.Cinemachine;

public class FollowCameraRotation : MonoBehaviour
{
    public Transform cameraTransform;
    [Header("Smoothing")]
    public float smooth = 12f;
    [Header("Lag Limit")]
    public float maxAngleOffset = 15f;

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
        Quaternion target = cameraTransform.rotation;

        // Wenn der Winkelunterschied zu groß wird, hart klemmen
        if (Quaternion.Angle(transform.rotation, target) > maxAngleOffset)
        {
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                target,
                Quaternion.Angle(transform.rotation, target) - maxAngleOffset
            );
        }

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            target,
            Time.deltaTime * smooth
        );
    }
}