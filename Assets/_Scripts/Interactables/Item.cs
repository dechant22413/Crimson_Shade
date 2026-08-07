using UnityEngine;

public class Item : MonoBehaviour, IInteractable
{
    #region  Settings
    [Header("Text")]
    public string interactionText;

    private ItemAudio itemAudio;
    #endregion
    private void Start()
    {
        itemAudio = GetComponent<ItemAudio>();
    }

    public string GetInteractionLabel()
    {
        return interactionText;
    }

    public void Interact()
    {
        itemAudio.PlayPickUp();
        Destroy(gameObject);
    }
}
