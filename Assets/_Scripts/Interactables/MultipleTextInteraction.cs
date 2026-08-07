using UnityEngine;
using System.Collections.Generic;

public class MultipleTextInteraction : MonoBehaviour, IInteractable
{
    public enum InteractionMode
    {
        Book,
        NPC
    }

    #region Settings

    [Header("Interaction")]
    [SerializeField] private InteractionMode interactionMode = InteractionMode.NPC;

    [Header("Dialogue")]
    [SerializeField] private List<string> dialogueLines = new List<string>();

    #endregion

    private int currentDialogueIndex = 0;
    private bool isTalking = false;

    public string GetInteractionLabel()
    {
        // Keine Dialogzeilen vorhanden
        if (dialogueLines.Count == 0)
        {
            return interactionMode == InteractionMode.Book
                ? "READ [E]"
                : "TALK [E]";
        }

        // Dialog läuft
        if (isTalking)
        {
            return dialogueLines[currentDialogueIndex] + "\n[E]";
        }

        // Dialog noch nicht gestartet
        return interactionMode == InteractionMode.Book
            ? "READ [E]"
            : "TALK [E]";
    }

    public void Interact()
    {
        if (dialogueLines.Count == 0)
            return;

        // Dialog starten
        if (!isTalking)
        {
            isTalking = true;
            currentDialogueIndex = 0;
            return;
        }

        // Zur nächsten Dialogzeile
        currentDialogueIndex++;

        // Dialog beendet
        if (currentDialogueIndex >= dialogueLines.Count)
        {
            currentDialogueIndex = 0;
            isTalking = false;
        }
    }
}