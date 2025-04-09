using System.Collections; // Add this line
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    public int currentHealth;

    [Header("Optional Feedback")]
    public GameObject bloodScreenPrefab; // UI overlay prefab (e.g., red flash)
    private CanvasGroup bloodScreenCanvasGroup; // CanvasGroup to control transparency of blood screen
    public float bloodScreenDuration = 0.5f; // Time for blood screen to stay visible
    public float fadeDuration = 1f; // Time to fade out the blood screen
    public AudioClip damageSound;
    private AudioSource audioSource;

    [Header("Health UI")]
    public Slider healthBarSlider; // Drag a UI Slider here to show health

    [Header("Events")]
    public UnityEvent<int> onHealthChanged; // Can be used for updating health bar
    public UnityEvent onDeath;

    public int CurrentHealth => currentHealth; // Public read-only property to easily access current health

    void Start()
    {
        currentHealth = maxHealth;
        audioSource = GetComponent<AudioSource>();

        Debug.Log($"[PlayerHealth] Player starts with {currentHealth} HP.");

        // Update health bar at the start
        if (healthBarSlider != null)
        {
            healthBarSlider.value = CalculateHealthPercentage();
        }

        // Initialize the blood screen CanvasGroup if a blood screen is set
        if (bloodScreenPrefab != null)
        {
            bloodScreenCanvasGroup = bloodScreenPrefab.GetComponent<CanvasGroup>();
            if (bloodScreenCanvasGroup == null)
            {
                bloodScreenCanvasGroup = bloodScreenPrefab.AddComponent<CanvasGroup>(); // Add CanvasGroup if it doesn't exist
            }
            bloodScreenCanvasGroup.alpha = 0f; // Initially invisible
        }
    }

    public void TakeDamage(int amount)
    {
        if (currentHealth <= 0) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log($"[PlayerHealth] Took {amount} damage. Current HP: {currentHealth}");

        // Trigger feedback
        TriggerFeedback();

        // Update health bar
        if (healthBarSlider != null)
        {
            healthBarSlider.value = CalculateHealthPercentage();
        }

        // Trigger health change event
        onHealthChanged?.Invoke(currentHealth);

        // Check for death
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void TriggerFeedback()
    {
        // Trigger blood screen effect (only visible for a brief moment)
        if (bloodScreenPrefab != null)
        {
            bloodScreenCanvasGroup.alpha = 1f; // Make the blood screen visible

            // Start fading the blood screen effect
            StartCoroutine(FadeBloodScreen());
        }

        // Trigger damage sound
        if (damageSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(damageSound);
        }
    }

    private IEnumerator FadeBloodScreen()
    {
        // Fade the blood screen in
        float elapsedTime = 0f;

        // Wait for the blood screen to stay visible for the duration
        while (elapsedTime < bloodScreenDuration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Fade the blood screen out over the specified duration
        float fadeElapsedTime = 0f;
        while (fadeElapsedTime < fadeDuration)
        {
            bloodScreenCanvasGroup.alpha = 1f - (fadeElapsedTime / fadeDuration); // Fade out
            fadeElapsedTime += Time.deltaTime;
            yield return null;
        }

        bloodScreenCanvasGroup.alpha = 0f; // Ensure it's fully faded out
    }

    private float CalculateHealthPercentage()
    {
        return (float)currentHealth / maxHealth;
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    private void Die()
    {
        Debug.Log("[PlayerHealth] Player has died.");
        onDeath?.Invoke();

        // Example action: disable player controller
        // GetComponent<PlayerController>().enabled = false;

        // Optionally: play death animation, show game over screen, etc.
    }
}
