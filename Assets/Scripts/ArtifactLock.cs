using UnityEngine;

public class ArtifactLock : MonoBehaviour
{
    private Rigidbody rb;

    // These need to be assigned via the Inspector or by other means
    public int artifactID; // Unique ID for this artifact
    public int expectedAltarID; // The altar ID this artifact belongs to

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnTriggerEnter(Collider other)
    {
        Altar altar = other.gameObject.GetComponent<Altar>();
        if (altar != null && altar.altarID == expectedAltarID)
        {
            rb.isKinematic = true;
            Vector3 altarCenter = other.bounds.center;
            transform.position = altarCenter;

            // Assuming GameManager.Instance.FinalGateController is set up correctly
            GameManager.Instance.FinalGateController.ArtifactPlaced(artifactID);
        }
    }
}








