using UnityEngine;

public class EnemyVisuals : MonoBehaviour
{
    private Renderer enemyRenderer;

    private void Awake()
    {
        enemyRenderer = GetComponent<Renderer>();
    }

    public void ChangeColor(Color color)
    {
        if (enemyRenderer == null) return;

        enemyRenderer.material.color = color;
        Invoke(nameof(ResetColor), 0.2f);
    }

    private void ResetColor()
    {
        if (enemyRenderer != null)
        {
            enemyRenderer.material.color = Color.red;
        }
    }
}
