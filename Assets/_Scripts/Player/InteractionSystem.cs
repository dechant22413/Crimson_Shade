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

    private IInteractable currentInteractable;
    private CharacterController characterController;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        HandleInteractionCheck();
    }

    private void HandleInteractionCheck()
    {
        currentInteractable = null;
        interactionLabel.gameObject.SetActive(false);

        Ray ray = new Ray(
            playerCam.transform.position,
            playerCam.transform.forward
        );

        RaycastHit[] hits = Physics.RaycastAll(
            ray,
            interactionRange
        );

        // Wichtig: RaycastAll ist nicht garantiert nach Entfernung sortiert
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (RaycastHit hit in hits)
        {
            // Eigenen CharacterController ignorieren
            if (characterController != null &&
                hit.collider == characterController)
            {
                continue;
            }

            IInteractable interactable =
                hit.collider.GetComponentInParent<IInteractable>();

            if (interactable != null)
            {
                currentInteractable = interactable;

                interactionLabel.text =
                    interactable.GetInteractionLabel();

                interactionLabel.gameObject.SetActive(true);

                break;
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