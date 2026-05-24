using UnityEngine;

public class WeaponLag : MonoBehaviour
{
    #region Settings
    [Header("References")]
    public Transform cameraTransform;

    [Header("Lag Settings")]
    public float positionLag = 0.05f;
    public float returnSpeed = 8f;

    [Header("Movement Influence")]
    public float velocityMultiplier = 0.02f;
    public float maxOffset = 0.2f;
    #endregion

    private Vector3 startLocalPos;
    private Vector3 currentOffset;

    private void Awake()
    {
        startLocalPos = transform.localPosition;
    }

    private void LateUpdate()
    {
        //Verzögertes Hinterherziehen der Waffen nach der Player Kamera
        if (PlayerMovement.Instance == null) return;

        Vector3 velocity = PlayerMovement.Instance.CurrentVelocity;

        //  Kamera-relative Bewegung ignorieren Y
        velocity.y = 0;

        //  Ziel-Offset aus Bewegung
        Vector3 targetOffset = -velocity * velocityMultiplier;
        targetOffset = Vector3.ClampMagnitude(targetOffset, maxOffset);

        //  Smooth Lag
        currentOffset = Vector3.Lerp(currentOffset, targetOffset, Time.deltaTime * returnSpeed);
        transform.localPosition = startLocalPos + currentOffset;
    }
}
