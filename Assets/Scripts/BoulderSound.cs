using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class BoulderTrap : MonoBehaviour
{
    private Rigidbody rb;
    private AudioSource audioSource;
    private float movementThreshold = 0.1f;
    private float maxVolume = 1.0f;
    private float minVolume = 0.1f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

        if (audioSource.clip == null)
        {
            Debug.LogError("No audio clip assigned to the AudioSource component.");
        }

        audioSource.loop = true;
        audioSource.volume = minVolume; // Start with minimum volume
    }

    void FixedUpdate()
    {
        // Check if the Rigidbody is not kinematic and if the boulder is moving
        if (!rb.isKinematic && rb.velocity.magnitude > movementThreshold)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }

            // Adjust volume based on speed
            audioSource.volume = Mathf.Lerp(minVolume, maxVolume, rb.velocity.magnitude / 10f);
        }
        else
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }
}



