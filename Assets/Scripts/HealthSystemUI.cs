using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LivesCounter : MonoBehaviour
{
    public int lives = 5; // Set the initial number of lives
    public Image[] hearts; // Array of heart images
    public Sprite fullHeart; // Full heart sprite
    public Sprite brokenHeart; // Broken heart sprite
    public float beatSpeed = 0.5f; // Speed of the heartbeat effect
    public float beatScale = 1.2f; // How much the heart grows during the heartbeat effect
    public AudioClip loseLifeSound; // Sound to play when a life is lost
    public Transform playerSpawnPoint; // Player's spawn point

    private Coroutine[] heartbeats; // Array to store heartbeat Coroutines

    void Start()
    {
        // Initialize the hearts display
        UpdateHearts();
        // Start the heartbeat effect
        StartHeartbeat();
    }

    public void StartHeartbeat()
    {
        heartbeats = new Coroutine[hearts.Length];
        for (int i = 0; i < hearts.Length; i++)
        {
            heartbeats[i] = StartCoroutine(Heartbeat(hearts[i]));
        }
    }

    public void LoseLife()
    {
        if (lives > 0)
        {
            lives--; // Decrease the number of lives
            UpdateHearts();
            StopCoroutine(heartbeats[lives]); // Stop the heartbeat for the lost life

            // Create a new AudioSource at the player's position
            AudioSource audioSource = playerSpawnPoint.gameObject.AddComponent<AudioSource>();
            audioSource.clip = loseLifeSound;
            audioSource.Play();

            // Destroy the AudioSource after the clip has finished playing
            Destroy(audioSource, loseLifeSound.length);
        }
        else
        {
            // Game Over
            Debug.Log("Game Over");
        }
    }


        void UpdateHearts()
    {
        // Update the hearts display
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < lives)
            {
                hearts[i].sprite = fullHeart;
            }
            else
            {
                hearts[i].sprite = brokenHeart;
            }
        }
    }

    IEnumerator Heartbeat(Image heart)
    {
        while (true)
        {
            // Scale up
            for (float t = 0; t < beatSpeed; t += Time.deltaTime)
            {
                float scale = Mathf.Lerp(1, beatScale, t / beatSpeed);
                heart.transform.localScale = new Vector3(scale, scale, 1);
                yield return null;
            }

            // Scale down
            for (float t = 0; t < beatSpeed; t += Time.deltaTime)
            {
                float scale = Mathf.Lerp(beatScale, 1, t / beatSpeed);
                heart.transform.localScale = new Vector3(scale, scale, 1);
                yield return null;
            }
        }
    }
}

