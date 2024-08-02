using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SpikeTrapMovement : MonoBehaviour
{
    public float speedUp = 10f;
    public float speedDown = 1f;
    public float pauseTime = 1f; // Time to pause at the top and bottom

    // Reference to the HeartLossTrigger script
    public HeartLossTrigger heartLossTrigger;

    private Vector3 startPosition;
    private Vector3 endPosition;
    private bool goingUp = true;
    private AudioSource audioSource;
    private bool hasPlayedSound = false; // To track if the sound has been played during upward movement

    void Start()
    {
        startPosition = transform.position;
        endPosition = startPosition + new Vector3(0, 1.5f, 0); // Moves up a fixed distance of 1.5 units
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (goingUp)
        {
            transform.position = Vector3.MoveTowards(transform.position, endPosition, speedUp * Time.deltaTime);
            if (Vector3.Distance(transform.position, endPosition) < 0.001f)
            {
                StartCoroutine(PauseBeforeGoingDown());
            }

            if (!hasPlayedSound)
            {
                audioSource.Play(); // Play the sound when the spike trap starts moving up
                hasPlayedSound = true; // Ensure the sound plays only once per upward movement
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, startPosition, speedDown * Time.deltaTime);
            if (Vector3.Distance(transform.position, startPosition) < 0.001f)
            {
                StartCoroutine(PauseBeforeGoingUp());
            }
        }
    }

    IEnumerator PauseBeforeGoingDown()
    {
        yield return new WaitForSeconds(pauseTime);
        goingUp = false;
        hasPlayedSound = false; // Reset sound flag for the next cycle

        // Disable the HeartLossTrigger when moving down
        heartLossTrigger.enabled = false;
    }

    IEnumerator PauseBeforeGoingUp()
    {
        yield return new WaitForSeconds(pauseTime);
        goingUp = true;

        // Enable the HeartLossTrigger when moving up
        heartLossTrigger.enabled = true;
    }
}

