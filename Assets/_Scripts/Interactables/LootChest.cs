using UnityEngine;

public class LootChest : MonoBehaviour, IInteractable
{
    [Header("References")]
    [SerializeField] private Animator chestAnimator;

    [SerializeField] private bool isOpened = false;

    #region Animator Variables
    private static readonly int OpenHash =
        Animator.StringToHash("Open");
    #endregion

    public string GetInteractionLabel()
    {
        return isOpened ? "" : "OPEN [E]";
    }

    public void Interact()
    {
        if (isOpened)
            return;

        Open();
    }

    private void Open()
    {
        isOpened = true;

        chestAnimator.SetBool(OpenHash, true);

        Debug.Log("Chest opened");
    }
}