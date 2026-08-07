using UnityEngine;
using System.Collections.Generic;

public class NPCInteraction : MonoBehaviour, IInteractable
{
    #region Settings
    [Header("Dialogue")]
    [SerializeField] private List<string> dialogueLines = new List<string>();
    #endregion

    private int currentDialogueIndex = 0;
    private bool isTalking = false;

    public string GetInteractionLabel()
    {
        if (dialogueLines.Count == 0)
            return "TALK [E]";

        // Wenn gerade ein Dialog läuft, zeige die aktuelle Zeile an
        if (isTalking)
            return dialogueLines[currentDialogueIndex];

        return "TALK [E]";
    }

    public void Interact()
    {
        if (dialogueLines.Count == 0)
            return;

        isTalking = true;

        // Zur nächsten Zeile wechseln
        currentDialogueIndex++;

        // Wenn das Ende erreicht wurde, wieder von vorne beginnen
        if (currentDialogueIndex >= dialogueLines.Count)
        {
            currentDialogueIndex = 0;
            isTalking = false;
        }
    }
}