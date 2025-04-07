using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    public Image fillImage;
    public Transform cam;

    private int maxHealth;

    public void SetMaxHealth(int max)
    {
        maxHealth = max;
        fillImage.fillAmount = 1f;
    }

    public void SetHealth(int currentHealth)
    {
        fillImage.fillAmount = (float)currentHealth / maxHealth;
    }

    void LateUpdate()
    {
        if (cam != null)
        {
            transform.LookAt(transform.position + cam.forward);
        }
    }
}
