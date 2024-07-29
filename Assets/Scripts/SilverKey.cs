using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    public AudioClip doorSound; // Assign your door sound here in the inspector
    private AudioSource audioSource;
    private bool used = false; // Add this line

    void Start()
    {
        // Get the AudioSource component attached to the key
        audioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the key has collided with the door and if it hasn't been used yet
        if (other.gameObject.CompareTag("Door") && !used) // Replace "Door" with the tag you assigned to your door
        {
            // Start the rotation coroutine
            StartCoroutine(RotateDoor(other.transform));

            // Play the door sound
            audioSource.PlayOneShot(doorSound);

            // Mark the key as used
            used = true;

            // Start the fade out coroutine
            StartCoroutine(FadeOutAndDestroy());
        }
    }

    IEnumerator RotateDoor(Transform doorTransform)
    {
        Quaternion startRotation = doorTransform.rotation;
        Quaternion endRotation = Quaternion.Euler(0, 0, 0);
        float duration = 1.0f; // Duration of the rotation in seconds
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            doorTransform.rotation = Quaternion.Slerp(startRotation, endRotation, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        doorTransform.rotation = endRotation;
    }

    IEnumerator FadeOutAndDestroy()
    {
        float duration = 1.0f; // Duration of the fade out in seconds
        float elapsed = 0.0f;
        Renderer renderer = GetComponent<Renderer>();
        Color originalColor = renderer.material.color;

        // Get the original emission color
        Color originalEmissionColor = renderer.material.GetColor("_EmissionColor");

        // Disable emission
        renderer.material.SetColor("_EmissionColor", Color.black);

        while (elapsed < duration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            renderer.material.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Restore the original emission color
        renderer.material.SetColor("_EmissionColor", originalEmissionColor);

        // Destroy the key object
        Destroy(gameObject);
    }

}




