using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueUIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI NPCnameText;
    [SerializeField] private TextMeshProUGUI NPCdialogueText;
    [SerializeField] private Image NPCportraitImage; //chatgpt kısmı

    [SerializeField] private AudioSource NPCaudio;

    [SerializeField] private float typeSpeed = 10f;


    private Queue<string> paragraphs = new Queue<string>();

    private bool conversationEnded,isTyping;

    private string p;


    private Coroutine typeDiaologueCO;




    public void displayNextParagraph(DialogueText dialogueText)
    {
        

        //if there is nothing in the queue
        if (paragraphs.Count == 0)
        {
            if (!conversationEnded)
            {
                //start a convo
                startConversation(dialogueText);
                //chatgpt ses kısmı
                if (dialogueText.dialougueSound != null)
                {
                    NPCaudio.PlayOneShot(dialogueText.dialougueSound);

                }
                //chatgpt ses kısmı
            }
            else if(conversationEnded && !isTyping)
            {
                //end a convo
                endConversation();
                return;
            }
        }


        //if there is something in the queue
        if (!isTyping)
        {
            p = paragraphs.Dequeue();
            typeDiaologueCO = StartCoroutine(TypeDialogueText(p));
        }
        else
        {
            finishParagraphEarly();
        }


        //update the conversation text
        //NPCdialogueText.text = p;


        if(paragraphs.Count == 0)
        {
            conversationEnded = true;
        }



    }



    private void startConversation(DialogueText dialogueText)
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
        NPCnameText.text = dialogueText.speakerName;

        for (int i = 0; i < dialogueText.paragpraphs.Length; i++)
        {
            paragraphs.Enqueue(dialogueText.paragpraphs[i]);

        }
        NPCportraitImage.sprite = dialogueText.npcPortrait; //chatgpt kısmı
    }

    private void endConversation()
    {
        //clear the queue
        paragraphs.Clear();

        conversationEnded = false;

        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }

    }


    private IEnumerator TypeDialogueText(string p)
    {
        float elapsedTime = 0f;
        int charIndex = 0;
        charIndex = Mathf.Clamp(charIndex, 0, p.Length);
        while(charIndex < p.Length)
        {
            elapsedTime += Time.deltaTime * typeSpeed;
            charIndex = Mathf.FloorToInt(elapsedTime);


            NPCdialogueText.text = p.Substring(0, charIndex);

            yield return null;
        }

        NPCdialogueText.text = p;
    }


    private void finishParagraphEarly()
    {
        //stop the coroutine
        StopCoroutine(typeDiaologueCO);



        //finish displaying text
        NPCdialogueText.text = p;



        //update istyping bool
        isTyping = false;

    }


}

