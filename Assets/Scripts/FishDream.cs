using UnityEngine;

public class MoveInCircle : MonoBehaviour
{
    public float radius = 5f; // Radius of the circle
    public float speed = 2f;  // Speed of the movement

    private float angle = 0f; // Current angle

    void Update()
    {
        // Calculate the new position
        float x = Mathf.Cos(angle) * radius;
        float y = Mathf.Sin(angle) * radius;

        // Update the position of the GameObject
        transform.position = new Vector3(x, y, transform.position.z);

        // Increment the angle based on the speed and time
        angle += speed * Time.deltaTime;

        // Keep the angle within 0 to 2*PI range
        if (angle > 2 * Mathf.PI)
        {
            angle -= 2 * Mathf.PI;
        }
    }
}

