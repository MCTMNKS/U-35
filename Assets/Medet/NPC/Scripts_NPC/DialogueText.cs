using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Dialouge / New Dialouge Contanier")]

public class DialogueText : ScriptableObject
{
    public string speakerName;

    public Sprite npcPortrait; //chatgpt kısmı

    public AudioClip dialougueSound; //chatgpt kısmı

    [TextArea(5,10)]
    public string[] paragpraphs;
}
