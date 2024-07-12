using UnityEngine;

public class FishMovement : MonoBehaviour
{
    public float speed = 1f; // Speed of the fish
    public float radius = 5f; // Radius of the circular path
    public Vector3 center = Vector3.zero; // Center of the circular path

    private float angle = 0f; // Current angle on the circular path
    private Animator animator; // Animator component

    void Start()
    {
        // Get the Animator component
        animator = GetComponent<Animator>();

        // Start the swim animation
        animator.Play("Swim");
    }

    void Update()
    {
        // Calculate the new position
        float x = Mathf.Cos(angle) * radius;
        float z = Mathf.Sin(angle) * radius;
        Vector3 newPos = center + new Vector3(x, 0, z);

        // Rotate to face the direction of movement
        transform.LookAt(newPos);

        // Move to the new position
        transform.position = newPos;

        // Update the angle for the next frame
        angle += speed * Time.deltaTime;
    }
}


