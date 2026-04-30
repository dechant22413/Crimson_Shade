using UnityEngine;

public class FollowCameraRotation : MonoBehaviour
{
    public Transform cameraTransform;

    [Header("Smoothing")]
    public float smooth = 12f;

    private bool initialized;

    void LateUpdate()
    {
        if (!initialized)
        {
            transform.rotation = cameraTransform.rotation;
            initialized = true;
            return;
        }

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            cameraTransform.rotation,
            Time.deltaTime * smooth
        );
    }
}