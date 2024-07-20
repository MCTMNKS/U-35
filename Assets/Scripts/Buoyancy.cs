using UnityEngine;

public class Buoyancy : MonoBehaviour
{
    public float UpwardForce = 9.81f; // Opposite of gravity
    private bool isInWater = false;

    void FixedUpdate()
    {
        if (isInWater)
        {
            GetComponent<Rigidbody>().AddForce(new Vector3(0, UpwardForce, 0));
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("RespawnTrigger"))
        {
            isInWater = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("RespawnTrigger"))
        {
            isInWater = false;
        }
    }
}
