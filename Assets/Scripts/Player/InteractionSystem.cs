using UnityEngine;
using TMPro;

public class InteractionSystem : MonoBehaviour
{
    #region Settings
    [Header("References")]
    public Camera playerCam;
    public TextMeshProUGUI interactionLabel;

    [Header("Settings")]
    public float interactionRange = 3f;
    public KeyCode interactKey = KeyCode.E;
    #endregion

    private IInteractable currentInteractable;

    private void Update()
    {
        if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out RaycastHit hit, interactionRange))
        {
            IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();

            if (interactable != null)
            {
                currentInteractable = interactable;
                interactionLabel.text = interactable.GetInteractionLabel();
                interactionLabel.gameObject.SetActive(true);

                if (Input.GetKeyDown(interactKey))
                    interactable.Interact();

                return;
            }
        }

        currentInteractable = null;
        interactionLabel.gameObject.SetActive(false);
    }
}
