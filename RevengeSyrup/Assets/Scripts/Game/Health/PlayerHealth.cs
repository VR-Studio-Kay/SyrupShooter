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
    private AudioSource audioSource;

    [Header("Health UI")]
    public Slider healthBarSlider;

    [Header("Game Over UI")]
    public GameObject gameOverCanvas;
    public Button resetCheckpointButton;
    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [SerializeField] private float fadeSpeed = 1f;

    [Header("XR Controller Feedback")]
    [SerializeField] private XRBaseController leftController;
    [SerializeField] private XRBaseController rightController;

    [Header("Disable on Death")]
    [SerializeField] private GameObject vrRig;
    [SerializeField] private GameObject locomotionSystem;

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
    }

    private void TriggerFeedback()
    {
        // Blood screen
        if (bloodScreenPrefab != null)
        {
            bloodScreenCanvasGroup.alpha = 1f;
            StartCoroutine(FadeBloodScreen());
        }

        // Audio
        if (damageSound != null && audioSource != null)
            audioSource.PlayOneShot(damageSound);

        // Haptics
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

        // Disable movement/input
        if (vrRig != null) vrRig.SetActive(false);
        if (locomotionSystem != null) locomotionSystem.SetActive(false);

        // Fade to black + show game over
        StartCoroutine(FadeToBlackThenGameOver());
    }

    private IEnumerator FadeToBlackThenGameOver()
    {
        if (fadeCanvasGroup == null)
        {
            if (gameOverCanvas != null)
                gameOverCanvas.SetActive(true);
            yield break;
        }

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * fadeSpeed;
            fadeCanvasGroup.alpha = t;
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        if (gameOverCanvas != null)
            gameOverCanvas.SetActive(true);
    }

    public void RestartScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}
