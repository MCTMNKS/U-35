using UnityEngine;

public class CloudEffect : MonoBehaviour
{
    public ParticleSystem cloudEffect;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("ExampleCharacter"))
        {
            cloudEffect.Play();
            Invoke("StopCloudEffect", 0.5f); // stops the effect after 2 seconds
        }
    }

    void StopCloudEffect()
    {
        cloudEffect.Stop();
    }
}
