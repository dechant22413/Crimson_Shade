using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponSway : MonoBehaviour
{
    #region Settings
    [Header("References")]
    public InputActionReference lookAction;

    [Header("Sway Settings")]
    [SerializeField] private float smooth = 10f;
    [SerializeField] private float swayMultiplier = 0.5f;
    [SerializeField] private float maxSway = 5f;
    #endregion 

    private Vector2 smoothedLook;

    private void OnEnable()
    {
        lookAction.action.Enable();
    }

    void Update()
    {
        //leichtes Rotieren der Waffe in BLickrichtung des Spielers
        Vector2 look = lookAction.action.ReadValue<Vector2>();

        smoothedLook = Vector2.Lerp(smoothedLook, look, Time.deltaTime * 10f);

        float mouseX = Mathf.Clamp(smoothedLook.x * swayMultiplier, -maxSway, maxSway);
        float mouseY = Mathf.Clamp(smoothedLook.y * swayMultiplier, -maxSway, maxSway);

        Quaternion rotationX = Quaternion.AngleAxis(-mouseY, Vector3.right);
        Quaternion rotationY = Quaternion.AngleAxis(mouseX, Vector3.up);

        Quaternion targetRotation = rotationX * rotationY;

        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * smooth);
    }
}