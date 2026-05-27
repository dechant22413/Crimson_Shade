using UnityEngine;

public class LootChest : MonoBehaviour, IInteractable
{
    [Header("References")]
    [SerializeField] private Animator chestAnimator;

    public string GetInteractionLabel() => "OPEN [E]";
    public void Interact() => Open();

    private bool isOpened = false;

    #region Animator Variables
    private static readonly int open = Animator.StringToHash("Open");
    #endregion

    private void Open()
    {
        isOpened = true;

        chestAnimator.SetBool(open, isOpened);
    }


}
