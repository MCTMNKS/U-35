using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Talk: NPC,Italkable
{
    [SerializeField] private DialogueText DialogueText;
    [SerializeField] private DialogueUIController dialogueController;



    public override void Interact()
    {
        Talk(DialogueText);

    }

    public void Talk(DialogueText dialogueText)
    {
        //start conversation
        dialogueController.displayNextParagraph(dialogueText);


    }
}
