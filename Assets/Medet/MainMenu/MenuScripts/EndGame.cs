using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EndGame : MonoBehaviour
{
    public GameObject blocker,npcCanvas,ingameCanvas,quitButton;
    public UnityEngine.UI.Image EndScreen;
    public TMP_Text endText; // Add a Text UI element to display "THE END"

    public float fadeDuration = 60f;
    private float fadeTimer = 0f;
    private bool startFade = false;

    void Start()
    {
        if (EndScreen != null)
        {
            // Set the initial color with alpha 0
            Color color = EndScreen.color;
            color.a = 0f;
            EndScreen.color = color;
        }

        if (endText != null)
        {
            // Set the initial alpha of the text to 0
            Color color = endText.color;
            color.a = 0f;
            endText.color = color;
        }
    }

    void Update()
    {
        if (startFade)
        {
            if (EndScreen != null && fadeTimer <= fadeDuration)
            {
                fadeTimer += Time.deltaTime;
                float alpha = Mathf.Clamp01(fadeTimer / fadeDuration);
                Color color = EndScreen.color;
                color.a = alpha;
                EndScreen.color = color;
                if(fadeTimer > 30f)
            {
                npcCanvas.SetActive(false);
                ingameCanvas.SetActive(false);

            }


                if (endText != null)
                {
                    Color textColor = endText.color;
                    textColor.a = alpha;
                    endText.color = textColor;
                }

                if(fadeTimer>34f)
                {
                    quitButton.SetActive(true);
                }
            }
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "ExampleCharacter")
        {
            activateBlockers();
            EndScreen.gameObject.SetActive(true);
            startFade = true;
            

        }
    }

    public void activateBlockers()
    {
        blocker.SetActive(true);
    }

    
}
