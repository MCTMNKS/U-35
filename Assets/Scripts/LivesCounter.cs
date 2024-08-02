using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using XEntity.InventoryItemSystem;
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
    public float volume = 1.0f; // Volume control (0.0 to 1.0)
    public ParticleSystem loseLifeParticles; //spawn particles when player loses a life

    private Coroutine[] heartbeats; // Array to store heartbeat Coroutines

    public GameObject gameOverScreen;

    void Start()
    {
        // Initialize the hearts display
        UpdateHearts();
        // Start the heartbeat effect
        StartHeartbeat();
        // Subscribe to the onAppleConsumed event
        ItemManager.Instance.onAppleConsumed.AddListener(Heal);
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
            StartCoroutine(TransitionHeart(hearts[lives])); // Start the transition for the lost life heart

            // Activate the particle system
            loseLifeParticles.Play();

            // Stop the particle system after it has finished playing
            StartCoroutine(StopParticles(loseLifeParticles.main.duration));

            // Create a new AudioSource at the player's position
            AudioSource audioSource = playerSpawnPoint.gameObject.AddComponent<AudioSource>();
            audioSource.clip = loseLifeSound;
            audioSource.volume = volume; // Set the volume
            audioSource.Play();

            // Destroy the AudioSource after the clip has finished playing
            Destroy(audioSource, loseLifeSound.length);
        }
        else
        {
            gameOverScreen.SetActive(true);

            Debug.Log("Game Over");
        }
    }

    IEnumerator StopParticles(float delay)
    {
        yield return new WaitForSeconds(delay);
        loseLifeParticles.Stop();
    }
    public void Heal()
    {
        if (lives < 5)
        {
            lives++; // Increase the number of lives
            hearts[lives - 1].sprite = fullHeart; // Update the heart image

            // Restart the heartbeat Coroutine for the healed heart
            if (heartbeats[lives - 1] != null)
            {
                StopCoroutine(heartbeats[lives - 1]);
            }
            heartbeats[lives - 1] = StartCoroutine(Heartbeat(hearts[lives - 1]));
        }
    }
    void OnDestroy()
    {
        ItemManager.Instance.onAppleConsumed.RemoveListener(Heal);
    }

    private IEnumerator TransitionHeart(Image heart)
    {
        // Stop the heartbeat coroutine for the lost life
        StopCoroutine(heartbeats[lives]);

        // Fade out the heart
        float fadeDuration = 0.5f;
        Color originalColor = heart.color;
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float normalizedTime = t / fadeDuration;
            heart.color = new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Lerp(1, 0, normalizedTime));
            yield return null;
        }
        heart.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0);

        // Change to the broken heart sprite
        heart.sprite = brokenHeart;

        // Fade in the broken heart
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float normalizedTime = t / fadeDuration;
            heart.color = new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Lerp(0, 1, normalizedTime));
            yield return null;
        }
        heart.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1);
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

    public void quitToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");

    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("I quit the game");
    }
}