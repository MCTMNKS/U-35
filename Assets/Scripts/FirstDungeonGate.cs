using UnityEngine;
using System.Collections;

public class DoorScript : MonoBehaviour
{
    public GameObject DoorObject; // The door object
    public float speed = 10f; // Speed of the door movement
    public string triggeringTag = "ExampleCharacter"; // Tag of the character that triggers the door
    public AudioClip stopSound; // Sound to play when the door stops moving

    private bool hasTriggered = false; // Flag to ensure the door is only triggered once
    private AudioSource audioSource; // AudioSource component to play the sound

    private void Start()
    {
        audioSource = DoorObject.AddComponent<AudioSource>();
        audioSource.clip = stopSound;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(triggeringTag) && !hasTriggered)
        {
            hasTriggered = true;
            StartCoroutine(MoveDoor());
        }
    }

    IEnumerator MoveDoor()
    {
        Vector3 startPosition = DoorObject.transform.position;
        Vector3 endPosition = startPosition + new Vector3(0, -15.5f, 0);
        Vector3 halfwayPosition = startPosition + (2.1f / 3f) * (endPosition - startPosition);


        while (DoorObject.transform.position.y > endPosition.y)
        {
            DoorObject.transform.position = Vector3.MoveTowards(DoorObject.transform.position, endPosition, Time.deltaTime * speed);

            // Play the sound when the door is halfway through its movement
            if (DoorObject.transform.position.y <= halfwayPosition.y && !audioSource.isPlaying)
            {
                audioSource.Play();
            }

            yield return null;
        }
    }
}


