using UnityEngine;

public class EnemyVisuals : MonoBehaviour
{
    private Renderer enemyRenderer;
    private Color originalColor;

    [Header("Color Settings")]
    [SerializeField] private float colorChangeDuration = 0.2f;
    [SerializeField] private Color damageColor = Color.yellow;
    [SerializeField] private Color defaultColor = Color.red;

    private void Awake()
    {
        enemyRenderer = GetComponent<Renderer>();
        originalColor = enemyRenderer.material.color;  // Store the original color
    }

    public void ChangeColor(Color color)
    {
        if (enemyRenderer == null) return;

        enemyRenderer.material.color = color;
        Invoke(nameof(ResetColor), colorChangeDuration);
    }

    private void ResetColor()
    {
        if (enemyRenderer != null)
        {
            enemyRenderer.material.color = originalColor; // Reset to original color
        }
    }
}
