using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeSpawner : MonoBehaviour
{
    public GameObject cubePrefab;
    public int poolSize = 100;
    public float spawnRate = 1f;

    private float nextSpawn = 0f;
    private Queue<GameObject> cubePool;

    void Start()
    {
        cubePool = new Queue<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject cube = Instantiate(cubePrefab);
            cube.SetActive(false);
            cubePool.Enqueue(cube);
        }
    }

    void Update()
    {
        if (Time.time > nextSpawn)
        {
            nextSpawn = Time.time + spawnRate;
            SpawnCube();
        }
    }

    void SpawnCube()
    {
        if (cubePool.Count > 0)
        {
            GameObject cube = cubePool.Dequeue();

            // Get the bounds of the object
            Bounds bounds = GetComponent<Collider>().bounds;

            // Generate a random position within the bounds
            float x = Random.Range(bounds.min.x, bounds.max.x);
            float y = Random.Range(bounds.min.y, bounds.max.y);
            float z = Random.Range(bounds.min.z, bounds.max.z);
            Vector3 randomPosition = new Vector3(x, y, z);

            // Set the position of the cube to the random position
            cube.transform.position = randomPosition;

            // Set the scale of the cube to a random value between 5 and 10
            float scale = Random.Range(5, 10);
            cube.transform.localScale = new Vector3(scale, scale, scale);

            cube.SetActive(true);

            if (!cube.GetComponent<Rigidbody>())
            {
                cube.AddComponent<Rigidbody>();
            }

            cube.GetComponent<Rigidbody>().useGravity = true;

            // Add the CubeFade script if not already attached
            if (!cube.GetComponent<CubeFade>())
            {
                cube.AddComponent<CubeFade>().SetSpawner(this);
            }
        }
    }

    public void ReturnCube(GameObject cube)
    {
        cube.SetActive(false);
        if (cube.GetComponent<CubeFade>())
        {
            cube.GetComponent<CubeFade>().ResetColor();
        }
        cubePool.Enqueue(cube);
    }
}

// This script should be attached to the cubePrefab
public class CubeFade : MonoBehaviour
{
    private Renderer cubeRenderer;
    private CubeSpawner spawner;
    private bool isFading;
    private Color initialColor;

    void Start()
    {
        cubeRenderer = GetComponent<Renderer>();
        initialColor = cubeRenderer.material.color;
    }

    public void SetSpawner(CubeSpawner spawner)
    {
        this.spawner = spawner;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!isFading && collision.gameObject.GetComponent<TerrainCollider>())
        {
            StartCoroutine(FadeOut());
        }
    }

    public void ResetColor()
    {
        cubeRenderer.material.color = initialColor;
        isFading = false;
    }

    private IEnumerator FadeOut()
    {
        isFading = true;
        float fadeDuration = 1.5f;
        float fadeSpeed = 1.5f / fadeDuration;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float blend = Mathf.Clamp01(t * fadeSpeed);
            cubeRenderer.material.color = new Color(initialColor.r, initialColor.g, initialColor.b, 1 - blend);
            yield return null;
        }

        // Return cube to pool
        spawner.ReturnCube(gameObject);
    }
}
