using UnityEngine;

public class TrapTrigger : MonoBehaviour
{
    public Rigidbody sphere; // Assign your sphere's Rigidbody in the inspector
    public Transform movingObject; // Assign the object to be moved in the inspector
    public float moveSpeed = 10f; // Speed at which the object moves along +Z axis

    private bool hasMoved = false; // Add this line

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object that triggered the event is the player
        if (other.CompareTag("ExampleCharacter"))
        {
            // Make the sphere non-kinematic, so it starts falling
            if (sphere != null)
            {
                sphere.isKinematic = false;
            }

            // Move the specified object towards the +Z direction very fast
            if (movingObject != null && !hasMoved) // Check if the object has already moved
            {
                StartCoroutine(MoveObject(movingObject, moveSpeed));
                hasMoved = true; // Set the flag to true after starting the coroutine
            }
        }
    }

    private System.Collections.IEnumerator MoveObject(Transform obj, float speed)
    {
        Vector3 targetPosition = obj.position + new Vector3(0, 0, 10f);

        // Use a coroutine to move the object to the target position
        while (Vector3.Distance(obj.position, targetPosition) > 0.01f)
        {
            obj.position = Vector3.MoveTowards(obj.position, targetPosition, speed * Time.deltaTime);
            yield return null;
        }
    }
}


