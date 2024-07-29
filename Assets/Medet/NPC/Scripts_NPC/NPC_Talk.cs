using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Talk : NPC, Italkable
{
    [SerializeField] private DialogueText DialogueText;
    [SerializeField] private DialogueUIController dialogueController;



    public override void Interact()
    {
        isInteracting = true; // This will now access the variable from the NPC class
        Talk(DialogueText);

    }

    public override void StopInteract()
    {
        isInteracting = false; // This will now access the variable from the NPC class
        // Reset the dialogue
        dialogueController.ResetDialogue();

        // Deactivate the UI canvas
        dialogueController.gameObject.SetActive(false);
    }

    public void Talk(DialogueText dialogueText)
    {
        //start conversation
        dialogueController.displayNextParagraph(dialogueText);


    }
}
