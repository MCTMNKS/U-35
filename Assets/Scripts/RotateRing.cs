using UnityEngine;

public class RotateRing : MonoBehaviour
{
    public float rotationSpeed = 50f; // Rotation speed in degrees per second

    void Update()
    {
        // Rotate the game object around its Y-axis at the defined rotation speed
        transform.Rotate(new Vector3(0, rotationSpeed, 0) * Time.deltaTime);
    }
}
