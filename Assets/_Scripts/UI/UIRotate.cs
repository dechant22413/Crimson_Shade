using UnityEngine;

public class UIRotate : MonoBehaviour
{
    public enum RotationDirection
    {
        Clockwise,
        CounterClockwise
    }

    [Header("Rotation")]
    [SerializeField] private RotationDirection direction = RotationDirection.Clockwise;
    [SerializeField] private float speed = 180f;

    private void Update()
    {
        float dir = direction == RotationDirection.Clockwise ? -1f : 1f;

        transform.Rotate(0f, 0f, speed * dir * Time.deltaTime);
    }
}