using UnityEngine;

public class PlaySoundOnCollision : MonoBehaviour
{
    public AudioClip[] audioClips; // Assign your audio clips in the Inspector
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if the object collided with has the "Terrain" tag
        if (collision.gameObject.tag == "Terrain")
        {
            // Play a random sound from the array
            int randomIndex = Random.Range(0, audioClips.Length);
            audioSource.clip = audioClips[randomIndex];
            audioSource.Play();
        }
    }

}

