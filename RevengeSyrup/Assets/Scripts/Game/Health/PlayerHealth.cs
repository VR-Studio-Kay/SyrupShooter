using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;

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
    public AudioClip deathSound;
    public GameObject deathVFXPrefab;
    private AudioSource audioSource;

    [Header("Health UI")]
    public Slider healthBarSlider;

    [Header("Game Over UI")]
    public GameObject gameOverCanvas;
    public GameObject youDiedText; // For Dark Souls effect
    public Button resetCheckpointButton;
    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [SerializeField] private float fadeSpeed = 1f;

    [Header("XR Controller Feedback")]
    [SerializeField] private XRBaseController leftController;
    [SerializeField] private XRBaseController rightController;

    [Header("Disable on Death")]
    [SerializeField] private GameObject vrRig;
    [SerializeField] private GameObject locomotionSystem;

    [Header("Animator (Optional)")]
    public Animator animator;

    [Header("Events")]
    public UnityEvent<int> onHealthChanged;
    public UnityEvent onDeath;

    public int CurrentHealth => currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
        audioSource = GetComponent<AudioSource>();
        audioSource.spatialBlend = 1f;

        if (healthBarSlider != null)
            healthBarSlider.value = CalculateHealthPercentage();

        if (bloodScreenPrefab != null)
        {
            bloodScreenCanvasGroup = bloodScreenPrefab.GetComponent<CanvasGroup>();
            if (bloodScreenCanvasGroup == null)
                bloodScreenCanvasGroup = bloodScreenPrefab.AddComponent<CanvasGroup>();

            bloodScreenCanvasGroup.alpha = 0f;
        }

        if (fadeCanvasGroup != null)
            fadeCanvasGroup.alpha = 0f;

        if (resetCheckpointButton != null)
            resetCheckpointButton.onClick.AddListener(RestartScene);

        if (youDiedText != null)
            youDiedText.SetActive(false);
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

        if (healthBarSlider != null)
            healthBarSlider.value = CalculateHealthPercentage();

        onHealthChanged?.Invoke(currentHealth);

        if (healSound != null && audioSource != null)
            PlaySoundWithPitchVariation(healSound, 1.0f, 1.2f);
    }

    private void TriggerFeedback()
    {
        // Blood screen effect
        if (bloodScreenPrefab != null)
        {
            bloodScreenCanvasGroup.alpha = 1f;
            StartCoroutine(FadeBloodScreen());
        }

        // Play random pitch damage sound
        if (damageSound != null && audioSource != null)
            PlaySoundWithPitchVariation(damageSound, 0.9f, 1.1f);

        // Haptic feedback
        TriggerHaptics();
    }

    private void TriggerHaptics(float intensity = 0.5f, float duration = 0.2f)
    {
        leftController?.SendHapticImpulse(intensity, duration);
        rightController?.SendHapticImpulse(intensity, duration);
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
        onDeath?.Invoke();

        if (animator != null)
            animator.SetTrigger("Die");

        // Death sound with random pitch
        if (deathSound != null && audioSource != null)
            PlaySoundWithPitchVariation(deathSound, 0.85f, 1.0f);

        if (deathVFXPrefab != null)
            Instantiate(deathVFXPrefab, transform.position, Quaternion.identity);

        TriggerHaptics(1.0f, 1.0f);

        Time.timeScale = 0.5f;
        Invoke(nameof(ResetTimeScale), 2f);

        if (vrRig != null) vrRig.SetActive(false);
        if (locomotionSystem != null) locomotionSystem.SetActive(false);

        StartCoroutine(FadeToBlackThenGameOver());
    }

    private void ResetTimeScale()
    {
        Time.timeScale = 1f;
    }

    private IEnumerator FadeToBlackThenGameOver()
    {
        if (fadeCanvasGroup != null)
        {
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * fadeSpeed;
                fadeCanvasGroup.alpha = t;
                yield return null;
            }
        }

        yield return new WaitForSecondsRealtime(1f);

        if (youDiedText != null)
            youDiedText.SetActive(true);

        yield return new WaitForSecondsRealtime(2f);

        if (gameOverCanvas != null)
            gameOverCanvas.SetActive(true);
    }

    public void RestartScene()
    {
        Time.timeScale = 1f;
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    // NEW: Sound with random pitch variation
    private void PlaySoundWithPitchVariation(AudioClip clip, float minPitch = 0.95f, float maxPitch = 1.05f)
    {
        if (clip == null || audioSource == null) return;

        audioSource.pitch = Random.Range(minPitch, maxPitch);
        audioSource.PlayOneShot(clip);
        audioSource.pitch = 1f; // Reset back to normal after playing
    }
}
