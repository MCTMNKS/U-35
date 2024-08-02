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

            // Get the DashAfterImage component
            DashAfterImage dashAfterImage = other.gameObject.GetComponent<DashAfterImage>();

            // Enable the DashAfterImage script
            if (dashAfterImage != null)
            {
                dashAfterImage.enabled = true;
            }

            // Destroy the power-up object
            Destroy(gameObject);
        }
    }
}

