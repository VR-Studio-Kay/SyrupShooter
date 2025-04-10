using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    public int currentHealth;

    [Header("Optional Feedback")]
    public GameObject bloodScreenPrefab;
    private CanvasGroup bloodScreenCanvasGroup;
    public float bloodScreenDuration = 0.5f;
    public float fadeDuration = 1f;
    public AudioClip damageSound;
    public AudioClip healSound;
    private AudioSource audioSource;

    [Header("Health UI")]
    public Slider healthBarSlider;

    [Header("Events")]
    public UnityEvent<int> onHealthChanged;
    public UnityEvent onDeath;

    public int CurrentHealth => currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
        audioSource = GetComponent<AudioSource>();

        Debug.Log($"[PlayerHealth] Player starts with {currentHealth} HP.");

        if (healthBarSlider != null)
            healthBarSlider.value = CalculateHealthPercentage();

        if (bloodScreenPrefab != null)
        {
            bloodScreenCanvasGroup = bloodScreenPrefab.GetComponent<CanvasGroup>();
            if (bloodScreenCanvasGroup == null)
                bloodScreenCanvasGroup = bloodScreenPrefab.AddComponent<CanvasGroup>();

            bloodScreenCanvasGroup.alpha = 0f;
        }
    }

    public void TakeDamage(int amount)
    {
        if (currentHealth <= 0) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log($"[PlayerHealth] Took {amount} damage. Current HP: {currentHealth}");

        TriggerFeedback();

        if (healthBarSlider != null)
            healthBarSlider.value = CalculateHealthPercentage();

        onHealthChanged?.Invoke(currentHealth);

        if (currentHealth <= 0)
            Die();
    }

    public void Heal(int amount)
    {
        if (currentHealth <= 0) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log($"[PlayerHealth] Healed {amount}. Current HP: {currentHealth}");

        if (healSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(healSound);
        }

        if (healthBarSlider != null)
            healthBarSlider.value = CalculateHealthPercentage();

        onHealthChanged?.Invoke(currentHealth);
    }

    private void TriggerFeedback()
    {
        if (bloodScreenPrefab != null)
        {
            bloodScreenCanvasGroup.alpha = 1f;
            StartCoroutine(FadeBloodScreen());
        }

        if (damageSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(damageSound);
        }
    }

    private IEnumerator FadeBloodScreen()
    {
        float elapsedTime = 0f;
        while (elapsedTime < bloodScreenDuration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        float fadeElapsedTime = 0f;
        while (fadeElapsedTime < fadeDuration)
        {
            bloodScreenCanvasGroup.alpha = 1f - (fadeElapsedTime / fadeDuration);
            fadeElapsedTime += Time.deltaTime;
            yield return null;
        }

        bloodScreenCanvasGroup.alpha = 0f;
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

        // Trigger the death event
        onDeath?.Invoke();

        // Restart the scene when player dies
        RestartScene();
    }

    public void RestartScene()
    {
        // Get the current active scene and reload it
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

}
