using UnityEngine;

public class TriggerExample : MonoBehaviour
{
    public ActionRecorder actionRecorder; // Assign in inspector
    public GhostPlayer ghostPlayer; // Assign in inspector
    public AudioSource audioSource; // Assign in inspector

    private void Start()
    {
        // Disable the ActionRecorder and GhostPlayer scripts at the start
        actionRecorder.enabled = false;
        ghostPlayer.enabled = false;

        // Ensure the AudioSource is disabled initially
        if (audioSource != null)
        {
            audioSource.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("ExampleCharacter")) // Assuming the player object has a tag "ExampleCharacter"
        {
            // Enable the ActionRecorder script immediately when the trigger is activated
            actionRecorder.enabled = true;

            // Enable the GhostPlayer script with a delay of 3 seconds when the trigger is activated
            Invoke("EnableGhostPlayer", 3f);

            // Enable the AudioSource to play sound
            if (audioSource != null)
            {
                audioSource.enabled = true;
                audioSource.Play(); // Play the audio clip
            }
        }
    }

    private void EnableGhostPlayer()
    {
        ghostPlayer.enabled = true;
    }
}

