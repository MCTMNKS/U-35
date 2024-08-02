using UnityEngine;

public class DisableOnTrigger : MonoBehaviour
{
    public string playerTag = "ExampleCharacter"; // Oyuncunun tagý
    public GameObject objectToDisable; // Atlas olcak obje

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object that entered the trigger has the player tag
        if (other.CompareTag(playerTag))
        {
            // Disable the Ghost Atlas GameObject
            if (objectToDisable != null)
            {
                objectToDisable.SetActive(false);
            }
        }
    }
}
