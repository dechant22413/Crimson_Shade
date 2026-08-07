using UnityEngine;

public class TextInteraction : MonoBehaviour, IInteractable
{
    #region  Settings
    [Header("Text")]
    public string interactionText;
    #endregion

    public string GetInteractionLabel()
    {
        return interactionText;
    }

    public void Interact()
    {

    }
}
