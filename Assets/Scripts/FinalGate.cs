using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateController : MonoBehaviour
{
    public List<GameObject> objectsToGoDown;
    public List<GameObject> objectsToGoUp;
    public AudioSource gateOpenSound; // AudioSource to play the gate opening sound
    private bool[] artifactsPlaced = new bool[2]; // Array to track placed artifacts

    // This method is called by ArtifactLock when an artifact is placed
    public void ArtifactPlaced(int artifactID)
    {
        if (artifactID >= 0 && artifactID < artifactsPlaced.Length)
        {
            artifactsPlaced[artifactID] = true;
        }

        // Check if both artifacts are placed before starting to open the gate
        if (artifactsPlaced[0] && artifactsPlaced[1])
        {
            StartCoroutine(OpenGate());
        }
    }

    IEnumerator OpenGate()
    {
        // Play the gate opening sound
        if (gateOpenSound != null)
        {
            gateOpenSound.Play();
        }

        float totalDistance = 40f; // Total distance to move
        float speed = 1f; // Speed of movement
        float delay = 0.1f; // Delay before starting upward movement

        for (int i = 0; i < objectsToGoDown.Count; i++)
        {
            // Start the downward movement
            StartCoroutine(MoveObject(objectsToGoDown[i], new Vector3(0, -totalDistance, 0), speed));

            // Wait for the delay, then start the upward movement
            yield return new WaitForSeconds(delay);
            StartCoroutine(MoveObject(objectsToGoUp[i], new Vector3(0, totalDistance, 0), speed));

            // Wait for the full interval before starting the next pair of movements
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator MoveObject(GameObject obj, Vector3 targetPosition, float speed)
    {
        Vector3 startPosition = obj.transform.position;
        Vector3 endPosition = startPosition + targetPosition;
        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime * speed;
            obj.transform.position = Vector3.Lerp(startPosition, endPosition, t);
            yield return null;
        }
    }
}





