using UnityEngine;

public class HeartLossTrigger : MonoBehaviour
{
    // Reference to the LivesCounter script
    public LivesCounter livesCounter;

    // Number of lives to lose
    public int livesToLose = 1;

    // Whether the trap should reset after being triggered
    public bool resetTrap = false;

    // Velocity threshold below which the trap won't trigger
    public float velocityThreshold = 1f;

    // Whether to check the trap's speed
    public bool checkTrapSpeed = false;

    // The trap's Rigidbody component
    private Rigidbody trapRigidbody;

    void Start()
    {
        // Get the trap's Rigidbody component
        trapRigidbody = GetComponent<Rigidbody>();
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the collided object is the player and the trap is active
        if (other.gameObject.CompareTag("ExampleCharacter") && this.enabled)
        {
            // Only trigger the trap if we're not checking the trap's speed, or if the trap's speed is above the threshold
            if (!checkTrapSpeed || trapRigidbody.velocity.magnitude > velocityThreshold)
            {
                // Call the LoseLife method
                for (int i = 0; i < livesToLose; i++)
                {
                    livesCounter.LoseLife();
                }

                // Disable the trap if it should not reset
                if (!resetTrap)
                {
                    this.enabled = false;
                }
            }
        }
    }
}

