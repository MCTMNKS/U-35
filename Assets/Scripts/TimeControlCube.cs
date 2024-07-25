using UnityEngine;

public class TimeControlCube : MonoBehaviour
{
    // Define the slow down factor
    public float slowDownFactor = 0.5f;
    private float originalTimeScale;

    private void Start()
    {
        // Store the original time scale
        originalTimeScale = Time.timeScale;
    }

    private void OnTriggerEnter(Collider other)
    {
        // When the player enters the cube, slow down time for the entire game
        if (other.gameObject.tag == "ExampleCharacter")
        {
            Time.timeScale = slowDownFactor;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // When the player leaves the cube, reset time to normal for the entire game
        if (other.gameObject.tag == "ExampleCharacter")
        {
            Time.timeScale = originalTimeScale;
        }
    }
}


