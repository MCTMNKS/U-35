using UnityEngine;
using System.Collections;

public class EnableNPC : MonoBehaviour
{
    public Transform player;
    public float enableDistance = 10f;
    private SkinnedMeshRenderer[] npcRenderers;
    public ParticleSystem particleEffect;

    void Start()
    {
        // Get all SkinnedMeshRenderer components in the children of this game object.
        npcRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
    }

    void Update()
    {
        float distance = Vector3.Distance(player.position, transform.position);

        if (distance < enableDistance)
        {
            // If the player is close enough, enable the SkinnedMeshRenderers and play the particle effect.
            foreach (SkinnedMeshRenderer npcRenderer in npcRenderers)
            {
                if (!npcRenderer.enabled)
                {
                    npcRenderer.enabled = true;
                    StartCoroutine(PlayParticleEffect());
                }
            }
        }
        else
        {
            // If the player is too far away, disable the SkinnedMeshRenderers and stop the particle effect.
            foreach (SkinnedMeshRenderer npcRenderer in npcRenderers)
            {
                if (npcRenderer.enabled)
                {
                    npcRenderer.enabled = false;
                    particleEffect.Stop();
                }
            }
        }
    }

    IEnumerator PlayParticleEffect()
    {
        particleEffect.Play();
        yield return new WaitForSeconds(2);
        particleEffect.Stop();
    }
}


