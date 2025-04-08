using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    public int currentHealth;

    [Header("Optional Feedback")]
    public GameObject bloodScreenPrefab; // UI overlay prefab (e.g., red flash)
    public AudioClip damageSound;
    private AudioSource audioSource;

    [Header("Health UI")]
    public UnityEngine.UI.Slider healthBarSlider; // Drag a UI Slider here to show health

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
            Instantiate(bloodScreenPrefab, transform.position, Quaternion.identity);
        }

        // Trigger damage sound
        if (damageSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(damageSound);
        }
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
