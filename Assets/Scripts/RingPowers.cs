using KinematicCharacterController.Examples;
using UnityEngine;

public class DoubleJumpPowerUp : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object is the player character
        if (other.gameObject.CompareTag("ExampleCharacter"))
        {
            // Get the ExampleCharacterController component
            ExampleCharacterController controller = other.gameObject.GetComponent<ExampleCharacterController>();

            // Enable double jumping
            controller.EnableDoubleJump();

            // Enable dashing
            controller.EnableDash();

            // Find the ExampleCharacterCamera component
            ExampleCharacterCamera camera = FindObjectOfType<ExampleCharacterCamera>();

            // Enable dash power in the camera for screen shake
            if (camera != null)
            {
                camera.EnableDashPower();
            }

            // Destroy the power-up object
            Destroy(gameObject);
        }
    }
}

