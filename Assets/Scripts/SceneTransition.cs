using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
    public float fadeSpeed = 1f; // Speed of the fade (higher value = faster fade)

    private Image image; // The image component

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private void Start()
    {
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        while (image.color.a > 0)
        {
            float newAlpha = image.color.a - (fadeSpeed * Time.deltaTime);
            image.color = new Color(image.color.r, image.color.g, image.color.b, newAlpha);
            yield return null;
        }
        gameObject.SetActive(false); // Disable the GameObject after the fade-in completes
    }
}
