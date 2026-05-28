using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public abstract string GetInteractionLabel();
    public abstract void Interact();
}