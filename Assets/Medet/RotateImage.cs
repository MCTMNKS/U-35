using UnityEngine;

public class RotateImage : MonoBehaviour
{
    public float rotationSpeed = 10f; // Degrees per second

    void Update()
    {
        // Calculate the rotation for this frame
        float rotationAmount = rotationSpeed * Time.deltaTime;

        // Apply the rotation around the Z-axis
        transform.Rotate(0, 0, rotationAmount);
    }
}
