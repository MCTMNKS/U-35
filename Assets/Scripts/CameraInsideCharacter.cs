using UnityEngine;

public class CameraInsideCharacter : MonoBehaviour
{
    public SkinnedMeshRenderer[] characterMeshes; // Assign your character's SkinnedMeshRenderers here

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MainCamera")) // replace "Player" with your camera's tag
        {
            foreach (var mesh in characterMeshes)
            {
                mesh.enabled = false;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MainCamera")) // replace "Player" with your camera's tag
        {
            foreach (var mesh in characterMeshes)
            {
                mesh.enabled = true;
            }
        }
    }
}
