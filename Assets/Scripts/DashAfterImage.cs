using System.Collections;
using UnityEngine;

public class DashAfterImage : MonoBehaviour
{
    public GameObject afterimagePrefab; // Assign a prefab of your afterimage
    public KeyCode dashKey = KeyCode.R;
    public float cooldown = 2f; // Cooldown time in seconds
    public int afterImageCount = 7; // Number of afterimages to spawn
    public float afterImageDuration = 0.25f; // Duration that each afterimage lasts
    public float afterImageSpawnInterval = 0.05f; // Time between afterimages

    private float lastDashTime;

    void Update()
    {
        if (Input.GetKeyDown(dashKey) && Time.time >= lastDashTime + cooldown)
        {
            lastDashTime = Time.time;
            StartCoroutine(SpawnAfterImages());
        }
    }

    IEnumerator SpawnAfterImages()
    {
        for (int i = 0; i < afterImageCount; i++)
        {
            // Only spawn afterimages if a movement key is being pressed
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
            {
                Vector3 position = transform.position;
                Quaternion rotation = transform.rotation;
                GameObject afterimage = Instantiate(afterimagePrefab, position, rotation);

                // Get all Skinned Mesh Renderers from the afterimage
                SkinnedMeshRenderer[] renderers = afterimage.GetComponentsInChildren<SkinnedMeshRenderer>();
                foreach (var renderer in renderers)
                {
                    StartCoroutine(FadeOutAfterImage(renderer, afterImageDuration));
                }
            }
            yield return new WaitForSeconds(afterImageSpawnInterval);
        }
    }

    IEnumerator FadeOutAfterImage(SkinnedMeshRenderer renderer, float duration)
    {
        Material[] materials = renderer.materials;
        float elapsed = 0f;

        // Fade out
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            SetMaterialAlpha(materials, alpha);
            yield return null;
        }

        Destroy(renderer.transform.root.gameObject); // Destroy the parent game object
    }


    void SetMaterialAlpha(Material[] materials, float alpha)
    {
        foreach (var material in materials)
        {
            Color color = material.color;
            color.a = alpha;
            material.color = color;
        }
    }
}





