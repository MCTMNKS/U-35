using System.Collections;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    public float shakeDuration = 2f;
    public float shakeMagnitude = 0.1f;
    public float fallDelay = 2f;
    public float resetDelay = 5f;
    public Color steppedOnColor = Color.red;
    public Color steppedOnEmissionColor = Color.black; // No emission when stepped on

    private Vector3 originalPosition;
    private Rigidbody rb;
    private Material material;
    private Color originalEmissionColor; // Original emission color

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        material = GetComponent<Renderer>().material;
        originalPosition = transform.position;
        originalEmissionColor = material.GetColor("_EmissionColor"); // Store the original emission color
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Character"))
        {
            material.SetColor("_BaseColor", steppedOnColor);
            material.SetColor("_EmissionColor", steppedOnEmissionColor); // Change the emission color
            StartCoroutine(Shake());
        }
    }

    IEnumerator Shake()
    {
        float elapsed = 0.0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1, 1) * shakeMagnitude;
            float y = Random.Range(-1, 1) * shakeMagnitude;

            transform.position = new Vector3(originalPosition.x + x, originalPosition.y + y, originalPosition.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.position = originalPosition;

        yield return new WaitForSeconds(fallDelay);

        rb.isKinematic = false;

        yield return new WaitForSeconds(resetDelay);

        rb.isKinematic = true;
        transform.position = originalPosition;
        material.SetColor("_BaseColor", Color.blue);
        material.SetColor("_EmissionColor", originalEmissionColor); // Reset the emission color
    }
}

