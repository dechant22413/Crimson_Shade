using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class InteractionSystem : MonoBehaviour
{
    #region Settings

    [Header("References")]
    [SerializeField] private InputActionReference interactAction;
    [SerializeField] private Camera playerCam;
    [SerializeField] private TextMeshProUGUI interactionLabel;

    [Header("Settings")]
    [SerializeField] private float interactionRange = 3f;

    #endregion

    #region Actions
    private void OnEnable()
    {
        interactAction.action.Enable();

        interactAction.action.performed += Interact;
    }
    private void OnDisable()
    {
        interactAction.action.Disable();

        interactAction.action.performed -= Interact;
    }
    #endregion
    private Interactable currentInteractable;

    private void Update()
    {
        HandleInteractionCheck();
    }

    private void HandleInteractionCheck()
    {
        currentInteractable = null;

        interactionLabel.gameObject.SetActive(false);

        if (Physics.Raycast(playerCam.transform.position,playerCam.transform.forward,out RaycastHit hit,interactionRange))
        {
            Interactable interactable = hit.collider.GetComponentInParent<Interactable>();

            if (interactable != null)
            {
                currentInteractable = interactable;

                interactionLabel.text =
                    interactable.GetInteractionLabel();

                interactionLabel.gameObject.SetActive(true);
            }
        }
    }

    private void Interact(InputAction.CallbackContext context)
    {
        if (currentInteractable == null)
            return;

        currentInteractable.Interact();
    }
}