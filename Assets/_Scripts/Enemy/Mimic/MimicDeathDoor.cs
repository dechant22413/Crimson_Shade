using UnityEngine;
using System.Collections;

public class MimicDeathDoor : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField] private Animator animator;
    [SerializeField] private float openDelay = 3f;

    private static readonly int openHash = Animator.StringToHash("Open");

    private bool doorOpening;

    public void OpenDoor()
    {
        if (doorOpening)
            return;

        StartCoroutine(OpenDoorRoutine());
    }

    private IEnumerator OpenDoorRoutine()
    {
        doorOpening = true;

        yield return new WaitForSeconds(openDelay);

        if (animator != null)
            animator.SetTrigger(openHash);
    }
}