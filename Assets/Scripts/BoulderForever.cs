using UnityEngine;
using System.Collections;

public class BoulderReset : MonoBehaviour
{
    public Rigidbody[] spheres; // Array of Rigidbodies
    public float resetTime = 30f; // Time after which the spheres reset

    private Vector3[] originalPositions; // Array to store the original positions

    void Start()
    {
        // Store the original positions of the spheres
        originalPositions = new Vector3[spheres.Length];
        for (int i = 0; i < spheres.Length; i++)
        {
            if (spheres[i] != null)
            {
                originalPositions[i] = spheres[i].transform.position;
            }
        }

        // Start the reset coroutine
        StartCoroutine(ResetBoulders());
    }

    IEnumerator ResetBoulders()
    {
        while (true)
        {
            // Wait for the specified reset time
            yield return new WaitForSeconds(resetTime);

            // Reset the positions of the spheres
            for (int i = 0; i < spheres.Length; i++)
            {
                if (spheres[i] != null)
                {
                    spheres[i].transform.position = originalPositions[i];
                    spheres[i].velocity = Vector3.zero; // Also reset the velocity
                }
            }
        }
    }
}
